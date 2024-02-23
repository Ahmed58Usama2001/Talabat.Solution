using System.Runtime.Serialization;

namespace Talabat.Core.Entities.Order_Aggregate;

public enum OrderStatus
{
    [EnumMember(Value ="Pendeng")]
    Pending,

    [EnumMember(Value = "Payment Received")]
    PaymentSucceded,

    [EnumMember(Value = "Payment Failed")]
    PaymentFailed
}
