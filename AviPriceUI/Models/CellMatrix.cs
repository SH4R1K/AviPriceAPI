using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AviPriceUI.Models;

public partial class CellMatrix
{
    public int IdCellMatrix { get; set; }

    [DisplayName("Цена")]
    public decimal? Price { get; set; }

    public int IdLocation { get; set; }

    [DisplayName("Категория")]
    public int IdCategory { get; set; }

    [DisplayName("Локация")]
    public int IdMatrix { get; set; }

    [DisplayName("Категория")]
    public virtual Category IdCategoryNavigation { get; set; } = null!;

    [DisplayName("Локация")]
    public virtual Location IdLocationNavigation { get; set; } = null!;

    public virtual Matrix IdMatrixNavigation { get; set; } = null!;
}
