using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AviPriceUI.Models;

public partial class CellMatrix
{
    public int IdCellMatrix { get; set; }

    [DisplayName("Цена")]
    [Required(ErrorMessage = "Это поле обязательное")]
    public decimal? Price { get; set; }

    [DisplayName("Локация")]
    public int IdLocation { get; set; }

    [DisplayName("Категория")]
    public int IdCategory { get; set; }

    public int IdMatrix { get; set; }

    [DisplayName("Категория")]
    public virtual Category IdCategoryNavigation { get; set; } = null!;

    [DisplayName("Локация")]
    public virtual Location IdLocationNavigation { get; set; } = null!;

    public virtual Matrix IdMatrixNavigation { get; set; } = null!;

    [NotMapped]
    public string ErrorMessage { get; set; }
}
