namespace AviPriceUI.Models
{
    public class MatricesViewModel
    {
        private string searchNameText;
        private string searchUserSegmentText;

        public IEnumerable<Matrix> Matrices { get; set; }

        public string MatricesType { get; set; }

        public string SearchNameText
        {
            get
            {
                return searchNameText ?? "";
            }

            set => searchNameText = value;
        }

        public string SearchUserSegmentText
        {
            get
            {
                return searchUserSegmentText ?? "";
            }

            set => searchUserSegmentText = value;
        }

        public string Message { get; set; }
    }
}
