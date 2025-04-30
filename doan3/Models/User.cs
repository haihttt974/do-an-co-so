using System;
using System.Collections.Generic;

namespace doan3.Models;

public partial class User
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? Referenceld { get; set; }

    public string? Email { get; set; }

    public string? Diachi { get; set; }

    public string? Sdt { get; set; }

    public DateTime? Createat { get; set; }

    public DateTime? Updateat { get; set; }

    public bool? Isactive { get; set; }

    public virtual Role? Role { get; set; }
}
