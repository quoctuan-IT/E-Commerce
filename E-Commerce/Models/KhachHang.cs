using System;
using System.Collections.Generic;

namespace E_Commerce.Models;

public partial class KhachHang
{
    public int MaKh { get; set; }

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string DiaChi { get; set; } = null!;

    public string DienThoai { get; set; } = null!;

    public bool HieuLuc { get; set; }

    public int VaiTro { get; set; }

    public string? RandomKey { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
