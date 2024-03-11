namespace AviPriceUI.Models
{
    public class CellMatricesViewModel
    {
        public IEnumerable<CellMatrix> CellMatrices { get; set; }

        public string ErrorMessage { get; set; }

        public int? IdUserSegment { get; set; } = null;

        public string MatrixName { get; set; }
    }
}
