using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class TaiLieu
{
    public int TailieuId { get; set; }

    public int? HangId { get; set; }

    public string Tentl { get; set; } = null!;

    public int? Sl { get; set; }

    public DateTime? Thoigiancapnhat { get; set; }

    public virtual HangGplx? Hang { get; set; }
}
