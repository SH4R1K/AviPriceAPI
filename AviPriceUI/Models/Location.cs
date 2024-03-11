using System;
using System.Collections.Generic;

namespace AviPriceUI.Models;

public partial class Location
{
    public int IdLocation { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<CellMatrix> CellMatrices { get; set; } = new List<CellMatrix>();

    public virtual ICollection<LocationTreePath> LocationTreePathAncestorNavigations { get; set; } = new List<LocationTreePath>();

    public virtual ICollection<LocationTreePath> LocationTreePathDescendantNavigations { get; set; } = new List<LocationTreePath>();
}
