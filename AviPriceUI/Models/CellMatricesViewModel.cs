using System.ComponentModel;

namespace AviPriceUI.Models
{
    public class CellMatricesViewModel
    {
        private string searchLocationText;
        private string searchCategoryText;

        public IEnumerable<CellMatrix> CellMatrices { get; set; }

        public string Message { get; set; }

        [DisplayName("Сегмент пользователей")]
        public int? IdUserSegment { get; set; } = null;

        public string MatrixName { get; set; }

        public string SearchLocationText 
        {
            get
            {
                return searchLocationText ?? "";
            }
            set => searchLocationText = value; 
        }

        public string SearchCategoryText 
        {
            get 
            { 
                return searchCategoryText ?? ""; 
            } 
            set => searchCategoryText = value; 
        }

        public int PageCount { get; set; }

        public int PageIndex { get; set; }
    }
}
