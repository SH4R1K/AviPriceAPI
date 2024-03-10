using System;
using System.Collections.Generic;

namespace AviPriceUI.Models;

public partial class Location
{
    public int IdLocation { get; set; }

    public string Name { get; set; } = null!;

    public int? IdParentLocation { get; set; }

    public virtual ICollection<CellMatrix> CellMatrices { get; set; } = new List<CellMatrix>();

    public virtual Location? IdParentLocationNavigation { get; set; }

    public virtual ICollection<Location> InverseIdParentLocationNavigation { get; set; } = new List<Location>();
}
