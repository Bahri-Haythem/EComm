using Domain.Entities.OrderAggregate;

namespace Domain.Specifications;

public class OrderByPaymentIntentIdSpecification : BaseSpecification<Order>
{
    public OrderByPaymentIntentIdSpecification(string paymentIntentId)
        : base(b => b.PaymentIntentId == paymentIntentId)
    {
    }
}
