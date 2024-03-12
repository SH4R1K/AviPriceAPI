using System;
using System.Collections.Generic;

namespace AviAPI.Models;

public partial class CategoryTreePath
{
    public int Ancestor { get; set; }

    public int Descendant { get; set; }

    public int Depth { get; set; }

    public virtual Category AncestorNavigation { get; set; } = null!;

    public virtual Category DescendantNavigation { get; set; } = null!;
}
