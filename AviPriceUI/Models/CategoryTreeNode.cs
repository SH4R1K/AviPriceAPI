namespace AviPriceUI.Models
{
    public class CategoryTreeNode
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<CategoryTreeNode> Children { get; set; }

        public Category Category { get; set; }
    }
}
