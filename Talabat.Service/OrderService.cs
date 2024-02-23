using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Order_Specs;

namespace Talabat.Service;

public class OrderService : IOrderService
{
    private readonly IBasketRepository _basketRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(
        IBasketRepository basketRepository,
        IUnitOfWork unitOfWork)
    {
        _basketRepository = basketRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryMethodId, Address shippingAddress)
    {
        //1. get basket from baskets repo
        var basket = await _basketRepository.GetBasketAsync(basketId);

        //2. Get Selected Items at basket from products repo
        var orderItems = new List<OrderItem>();

        if(basket?.Items?.Count>0)
        {
            var productRepository = _unitOfWork.Repository<Product>();
            foreach (var item in basket.Items)
            {
                var product = await productRepository.GetByIdAsync(item.Id);

                var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                orderItems.Add(orderItem);
            }
        }

        //3. Calculate SubTotal
        var subTotal = orderItems.Sum(O => O.Quantity * O.Price);

        //4. Get DeliveryMethod From DeliveryMethod Repository
        var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

        //5. Create Order 
        var order = new Order(buyerEmail, shippingAddress, deliveryMethod, orderItems, subTotal);

        await _unitOfWork.Repository<Order>().AddAsync(order);

        //6. Save to Database
        var result=await _unitOfWork.CompleteAsync();

        if (result <= 0) return null;

        return order;
       
    }

    public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
    {
        var deliveryMethodRepo = _unitOfWork.Repository<DeliveryMethod>();

        var deliveryMethods=await deliveryMethodRepo.GetAllAsync();

        return deliveryMethods;
    }

    public Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
    {
        var orderRepo = _unitOfWork.Repository<Order>();

        var spec = new OrderSpecifications(orderId,buyerEmail);

        var order = orderRepo.GetByIdWithSpecAsync(spec);

        return order;
    }

    public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
    {
        var orderRepo = _unitOfWork.Repository<Order>();

        var spec = new OrderSpecifications(buyerEmail);

        var orders = orderRepo.GetAllWithSpecAsync(spec);

        return orders;
    }
}
