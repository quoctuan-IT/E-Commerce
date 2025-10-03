using System;
using System.Collections.Generic;

namespace E_Commerce.Models;

public partial class VChiTietHoaDon
{
    public int MaCt { get; set; }

    public int MaHd { get; set; }

    public int MaHh { get; set; }

    public double DonGia { get; set; }

    public int SoLuong { get; set; }

    public string TenHh { get; set; } = null!;
}
