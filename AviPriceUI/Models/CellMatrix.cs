using System;
using System.Collections.Generic;

namespace AviPriceUI.Models;

public partial class CellMatrix
{
    public int IdCellMatrix { get; set; }

    public decimal? Price { get; set; }

    public int IdLocation { get; set; }

    public int IdCategory { get; set; }

    public int IdMatrix { get; set; }

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual Location IdLocationNavigation { get; set; } = null!;

    public virtual Matrix IdMatrixNavigation { get; set; } = null!;
}
