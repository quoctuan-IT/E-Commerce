using AutoMapper;

using E_Commerce.Models;
using E_Commerce.Models.ViewModels;

namespace E_Commerce.Helpers
{
	public class AutoMapperProfile : Profile
	{
		public AutoMapperProfile()
		{
			CreateMap<Register, KhachHang>();
		}
	}
}
