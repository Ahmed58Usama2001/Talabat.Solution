using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Specs;

public class OrderWithPaymentIntentSpecifications : BaseSpecifications<Order>
{

    public OrderWithPaymentIntentSpecifications(string paymentIntentId)
        : base(O => O.PaymentIntentId == paymentIntentId)
    {
        Includes.Add(O => O.DeliveryMethod);
        Includes.Add(O => O.Items);

        AddOrderByDesc(O => O.OrderDate);
    }
}
