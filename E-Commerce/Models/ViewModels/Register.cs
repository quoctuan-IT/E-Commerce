using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.ViewModels
{
    public class Register
	{
		[Required(ErrorMessage = "Name empty!")]
		[MaxLength(10, ErrorMessage = "Tối đa 10 kí tự!")]
		public required string HoTen { get; set; }

		[Required(ErrorMessage = "Address empty!")]
		[MaxLength(10, ErrorMessage = "Tối đa 10 kí tự!")]
		public required string DiaChi { get; set; }

		[Display(Name = "Phone")]
		[Required(ErrorMessage = "Phone empty!")]
		[MinLength(10, ErrorMessage = "10 number required!"), MaxLength(10, ErrorMessage = "10 number required!")]
		public required string DienThoai { get; set; }

		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Password empty!")]
		[MaxLength(10, ErrorMessage = "Tối đa 10 kí tự!")]
		public required string MatKhau { get; set; }
    }
}
