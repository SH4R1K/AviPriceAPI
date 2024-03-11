namespace AviPriceUI.Models
{
    public class MatricesViewModel
    {
        public IEnumerable<Matrix> Matrices { get; set; }

        public string MatricesType { get; set; }

        public string SearchNameText { get; set; }
        public string SearchUserSegmentText { get; set; }
    }
}
