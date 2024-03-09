using System;
using System.Collections.Generic;

namespace AviAPI.Models;

public partial class UserSegment
{
    public int IdUserSegment { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Matrix> Matrices { get; set; } = new List<Matrix>();
}
