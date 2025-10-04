using System;
using System.Collections.Generic;

namespace E_Commerce.Models;
public partial class HoaDon
{
    public int MaHd { get; set; }

    public int MaKh { get; set; }

    public DateTime NgayDat { get; set; }

    public string? HoTen { get; set; }

    public string DiaChi { get; set; } = null!;

    public string CachThanhToan { get; set; } = null!;

    public string CachVanChuyen { get; set; } = null!;

    public string? DienThoai { get; set; }

    public int MaTrangThai { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual KhachHang MaKhNavigation { get; set; } = null!;

    public virtual TrangThai MaTrangThaiNavigation { get; set; } = null!;
}
