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
    }
}
