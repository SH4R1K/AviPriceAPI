using System;
using System.Collections.Generic;

namespace AviAPI.Models;

public partial class Category
{
    public int IdCategory { get; set; }

    public string Name { get; set; } = null!;

    public int? IdParentCategory { get; set; }

    public virtual ICollection<CellMatrix> CellMatrices { get; set; } = new List<CellMatrix>();

    public virtual Category? IdParentCategoryNavigation { get; set; }

    public virtual ICollection<Category> InverseIdParentCategoryNavigation { get; set; } = new List<Category>();
}
