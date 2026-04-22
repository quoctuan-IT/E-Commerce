using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.DTOs
{
    public class CategoryResponseDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class ProductResponseDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public double UnitPrice { get; set; }
        public string Image { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class DirectCheckoutItemDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }
    }

    public class DirectCheckoutRequestDTO
    {
        [Required(ErrorMessage = "Payment Method is required.")]
        public int PaymentMethodId { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(50, ErrorMessage = "Address cannot exceed 50 characters.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = null!;

        [MinLength(1, ErrorMessage = "At least one product is required")]
        public List<DirectCheckoutItemDTO> Items { get; set; } = [];
    }

    public class OrderDetailResponseDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string Image { get; set; } = null!;
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public double LineTotal { get; set; }
    }

    public class OrderResponseDTO
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int OrderStatusId { get; set; }
        public string OrderStatusName { get; set; } = null!;
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; } = null!;
        public double TotalAmount { get; set; }
        public List<OrderDetailResponseDTO> Items { get; set; } = [];
    }
}
