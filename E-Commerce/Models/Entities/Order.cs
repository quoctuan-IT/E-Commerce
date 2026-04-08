namespace E_Commerce.Models.Entities;
public partial class Order
{
    public int OrderId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int OrderStatusId { get; set; }

    public int PaymentMethodId { get; set; }

    public double TotalAmount { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual OrderStatus OrderStatus { get; set; } = null!;

    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
