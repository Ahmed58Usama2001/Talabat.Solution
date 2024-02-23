using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Specifications.Order_Specs;

public class OrderSpecifications:BaseSpecifications<Order>
{
    public OrderSpecifications( string buyerEmail)
        :base(O=>O.BuyerEmail== buyerEmail)
    {
        Includes.Add(O => O.DeliveryMethod);
        Includes.Add(O => O.Items);

        AddOrderByDesc(O => O.OrderDate);
    }

    public OrderSpecifications(int orderId,string buyerEmail)
       : base(O=>O.Id==orderId && O.BuyerEmail == buyerEmail)
    {
        Includes.Add(O => O.DeliveryMethod);
        Includes.Add(O => O.Items);

        AddOrderByDesc(O => O.OrderDate);
    }
}
