using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AviPriceUI.Models;

[ProtoContract]
public partial class CellMatrix
{
    [ProtoMember(1)]
    public int IdCellMatrix { get; set; }

    [ProtoMember(2)]
    [DisplayName("Цена")]
    [Required(ErrorMessage = "Это поле обязательное")]
    public decimal? Price { get; set; }

    [ProtoMember(3)]
    [DisplayName("Локация")]
    public int IdLocation { get; set; }

    [ProtoMember(4)]
    [DisplayName("Категория")]
    public int IdCategory { get; set; }

    [ProtoMember(5)]
    public int IdMatrix { get; set; }

    [DisplayName("Категория")]
    public virtual Category IdCategoryNavigation { get; set; } = null!;

    [DisplayName("Локация")]
    public virtual Location IdLocationNavigation { get; set; } = null!;

    public virtual Matrix IdMatrixNavigation { get; set; } = null!;

    [NotMapped]
    public string ErrorMessage { get; set; }
}
