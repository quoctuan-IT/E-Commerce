namespace E_Commerce.Models.Entities;
public partial class PaymentMethod
{
    public int PaymentMethodId { get; set; }

    public string PaymentMethodName { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = [];
}
