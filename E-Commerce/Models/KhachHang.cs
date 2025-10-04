namespace E_Commerce.Models;
public partial class KhachHang
{
    public int MaKh { get; set; }

    public string HoTen { get; set; } = null!;

    public string DienThoai { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public int VaiTro { get; set; }

    public bool HieuLuc { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
