using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SendDataInApi.Models;

[ProtoContract]
public partial class Matrix
{
    [ProtoMember(1)]
    public int IdMatrix { get; set; }

    [DisplayName("Имя матрицы")]
    public string Name { get; set; } = null!;

    [ProtoMember(2)]
    [DisplayName("Сегмент пользователей")]
    public int? IdUserSegment { get; set; }

    [ProtoMember(3)]
    public virtual ICollection<CellMatrix> CellMatrices { get; set; } = new List<CellMatrix>();

    [DisplayName("Сегмент пользователей")]
    public virtual UserSegment? IdUserSegmentNavigation { get; set; }

    [NotMapped]
    public bool IsSelected { get; set; } = false;
}
