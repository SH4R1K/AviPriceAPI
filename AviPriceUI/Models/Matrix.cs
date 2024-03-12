using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AviPriceUI.Models;

public partial class Matrix
{
    public int IdMatrix { get; set; }

    [DisplayName("Имя матрицы")]
    public string Name { get; set; } = null!;

    [DisplayName("Сегмент пользователей")]
    public int? IdUserSegment { get; set; }

    public virtual ICollection<CellMatrix> CellMatrices { get; set; } = new List<CellMatrix>();

    [DisplayName("Сегмент пользователей")]
    public virtual UserSegment? IdUserSegmentNavigation { get; set; }

    [NotMapped]
    public bool IsSelected { get; set; } = false;
}
