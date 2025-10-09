using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.DTOs
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string FullName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
        public string ShippingMethod { get; set; } = null!;
        public int OrderStatusId { get; set; }
        public string? OrderStatusName { get; set; }
        public double TotalAmount { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = [];
    }

    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = null!;

        [Required(ErrorMessage = "Shipping method is required")]
        public string ShippingMethod { get; set; } = null!;

        public bool DefaultAddress { get; set; } = false;

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreateOrderItemDto> Items { get; set; } = [];
    }

    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public double UnitPrice { get; set; }
    }

    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductImage { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
    }

    public class UpdateOrderDto
    {
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        public string? PaymentMethod { get; set; }
        public string? ShippingMethod { get; set; }
        public int? OrderStatusId { get; set; }
    }

    public class OrderFilterDto
    {
        public string? UserId { get; set; }
        public int? OrderStatusId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class OrderSummaryDto
    {
        public int OrderId { get; set; }
        public string UserName { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public string OrderStatusName { get; set; } = null!;
        public double TotalAmount { get; set; }
        public int ItemCount { get; set; }
    }
}
