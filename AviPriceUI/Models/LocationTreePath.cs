using System;
using System.Collections.Generic;

namespace AviPriceUI.Models;

public partial class LocationTreePath
{
    public int Ancestor { get; set; }

    public int Descendant { get; set; }

    public int Depth { get; set; }

    public virtual Location AncestorNavigation { get; set; } = null!;

    public virtual Location DescendantNavigation { get; set; } = null!;
}
