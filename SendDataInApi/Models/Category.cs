using System;
using System.Collections.Generic;

namespace SendDataInApi.Models;

public partial class Category
{
    public int IdCategory { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<CategoryTreePath> CategoryTreePathAncestorNavigations { get; set; } = new List<CategoryTreePath>();

    public virtual ICollection<CategoryTreePath> CategoryTreePathDescendantNavigations { get; set; } = new List<CategoryTreePath>();

    public virtual ICollection<CellMatrix> CellMatrices { get; set; } = new List<CellMatrix>();
}
