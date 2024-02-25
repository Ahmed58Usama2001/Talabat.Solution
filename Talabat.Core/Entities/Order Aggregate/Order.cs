using System.ComponentModel.DataAnnotations.Schema;

namespace Talabat.Core.Entities.Order_Aggregate;

public class Order:BaseEntity
{
    public Order()
    {
        
    }
    public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal,string paymentIntentId)
    {
        BuyerEmail = buyerEmail;
        ShippingAddress = shippingAddress;
        DeliveryMethod = deliveryMethod;
        Items = items;
        SubTotal = subTotal;
        PaymentIntentId = paymentIntentId;  
    }

    public string BuyerEmail { get; set; }
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public Address ShippingAddress { get; set; } //not a navigational property, so its properties will be included in order table.
                                                 //That will happen because Address doesn't have a primary key

    //public int DeliveryMethodId { get; set; } //Foreign Key [1]
    public DeliveryMethod? DeliveryMethod { get; set; }  //Navigational Property [One]

    public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>(); //Navigational Property [Many]

    public decimal SubTotal { get; set; }

    //[NotMapped]
    //public decimal Total=>  SubTotal + DeliveryMethod.Cost;

    public decimal GetTotal()
        => SubTotal + DeliveryMethod.Cost;

    public string PaymentIntentId { get; set; } = string.Empty;
}
