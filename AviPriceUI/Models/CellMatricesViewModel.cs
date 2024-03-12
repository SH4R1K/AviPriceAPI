using System.ComponentModel;

namespace AviPriceUI.Models
{
    public class CellMatricesViewModel
    {
        public IEnumerable<CellMatrix> CellMatrices { get; set; }

        public string ErrorMessage { get; set; }

        [DisplayName("Сегмент пользователей")]
        public int? IdUserSegment { get; set; } = null;

        public string MatrixName { get; set; }

        public string SearchLocationText { get; set; }

        public string SearchCategoryText { get; set; }

        public int PageCount { get; set; }

        public int PageIndex { get; set; }
    }
}
