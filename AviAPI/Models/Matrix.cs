using System;
using System.Collections.Generic;

namespace AviAPI.Models;

public partial class Matrix
{
    public int IdMatrix { get; set; }

    public string Name { get; set; } = null!;

    public int? IdUserSegment { get; set; }

    public virtual ICollection<CellMatrix> CellMatrices { get; set; } = new List<CellMatrix>();

    public virtual UserSegment? IdUserSegmentNavigation { get; set; }
}
