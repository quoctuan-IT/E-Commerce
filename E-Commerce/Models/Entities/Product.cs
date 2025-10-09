namespace E_Commerce.Models.Entities;
public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int CategoryId { get; set; }

    public double UnitPrice { get; set; }

    public string Image { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];

    public virtual Category Category { get; set; } = null!;
}
