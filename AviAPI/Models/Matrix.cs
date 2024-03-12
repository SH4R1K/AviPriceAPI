using ProtoBuf;
using System;
using System.Collections.Generic;

namespace AviAPI.Models;

[ProtoContract]
public partial class Matrix
{
    [ProtoMember(1)]
    public int IdMatrix { get; set; }

    public string Name { get; set; } = null!;

    [ProtoMember(2)]
    public int? IdUserSegment { get; set; }

    [ProtoMember(3)]
    public virtual ICollection<CellMatrix> CellMatrices { get; set; } = new List<CellMatrix>();

    public virtual UserSegment? IdUserSegmentNavigation { get; set; }
}
