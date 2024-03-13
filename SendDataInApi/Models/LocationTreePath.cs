using ProtoBuf;
using System;
using System.Collections.Generic;

namespace SendDataInApi.Models;

[ProtoContract]
public partial class LocationTreePath
{
    [ProtoMember(1)]
    public int Ancestor { get; set; }

    [ProtoMember(2)]
    public int Descendant { get; set; }

    [ProtoMember(3)]
    public int Depth { get; set; }

    public virtual Location AncestorNavigation { get; set; } = null!;

    public virtual Location DescendantNavigation { get; set; } = null!;
}
