namespace E_Commerce.Models;
public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public string FullName { get; set; }  = null!;

    public string Address { get; set; }  = null!;

    public string PaymentMethod { get; set; }  = null!;

    public string ShippingMethod { get; set; }  = null!;

    public string Phone { get; set; }  = null!;

    public int OrderStatusId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];

    public virtual AppUser AppUser { get; set; } = null!;

    public virtual OrderStatus OrderStatus { get; set; } = null!;
}
