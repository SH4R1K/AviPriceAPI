using ProtoBuf;
using System;
using System.Collections.Generic;

namespace AviAPI.Models;

[ProtoContract]
public partial class CellMatrix
{
    [ProtoMember(1)]
    public int IdCellMatrix { get; set; }

    [ProtoMember(2)]
    public decimal? Price { get; set; }

    [ProtoMember(3)]
    public int IdLocation { get; set; }

    [ProtoMember(4)]
    public int IdCategory { get; set; }

    [ProtoMember(5)]
    public int IdMatrix { get; set; }

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual Location IdLocationNavigation { get; set; } = null!;

    public virtual Matrix IdMatrixNavigation { get; set; } = null!;
}
