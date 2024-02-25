using Microsoft.Extensions.Configuration;
using Stripe;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Order_Specs;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IConfiguration configuration,IBasketRepository basketRepository,IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        _basketRepository = basketRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
    {
        StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

        var basket= await _basketRepository.GetBasketAsync(basketId);

        if(basket is null)
            return null;

        var shippingPrice = 0m;

        if(basket.DeliveryMethodId.HasValue)
        {
            var deliveryMethod=await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId.Value);
            basket.ShippingPrice = deliveryMethod.Cost;
            shippingPrice = deliveryMethod.Cost;
        }

        if(basket?.Items.Count>0)
        {
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                if(item.Price!=product.Price)
                    item.Price= product.Price;
            }
        }

        PaymentIntentService paymentIntentService= new PaymentIntentService();

        PaymentIntent paymentIntent;

        if(string.IsNullOrEmpty(basket.PaymentIntentId)) //create new payment intent
        {
            var createOptions = new PaymentIntentCreateOptions()
            {
                Amount = (long?)(basket.Items.Sum(I => I.Price * I.Quantity*100)+shippingPrice*100),
                Currency="usd",
                PaymentMethodTypes=new List<string>() { "card"}
            };

            paymentIntent = await paymentIntentService.CreateAsync(createOptions); //integrate with stripe

            basket.PaymentIntentId = paymentIntent.Id;
            basket.ClientSecret= paymentIntent.ClientSecret;
        }
        else//update
        {
            var updateOptions = new PaymentIntentUpdateOptions()
            {
                Amount = (long?)(basket.Items.Sum(I => I.Price * I.Quantity * 100) + shippingPrice * 100),
            };
            await paymentIntentService.UpdateAsync(basket.PaymentIntentId,updateOptions);
        }

       return await _basketRepository.UpdateBasketAsync(basket);

    }

    public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string paymentIntentId, bool isSucceeded)
    {
        var spec = new OrderWithPaymentIntentSpecifications(paymentIntentId);

        var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

        if (isSucceeded)
            order.Status = OrderStatus.PaymentSucceded;
        else
            order.Status = OrderStatus.PaymentFailed;

        _unitOfWork.Repository<Order>().Update(order);

        await _unitOfWork.CompleteAsync();

        return order;
    }
}
