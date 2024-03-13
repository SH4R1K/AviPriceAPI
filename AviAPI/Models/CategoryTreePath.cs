using ProtoBuf;
using System;
using System.Collections.Generic;

namespace AviAPI.Models;

[ProtoContract]
public partial class CategoryTreePath
{
    [ProtoMember(1)]
    public int Ancestor { get; set; }

    [ProtoMember(2)]
    public int Descendant { get; set; }

    [ProtoMember(3)]
    public int Depth { get; set; }

    public virtual Category AncestorNavigation { get; set; } = null!;

    public virtual Category DescendantNavigation { get; set; } = null!;
}
