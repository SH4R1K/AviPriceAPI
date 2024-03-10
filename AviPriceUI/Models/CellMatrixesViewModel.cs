namespace AviPriceUI.Models
{
    public class CellMatrixesViewModel
    {
        public IEnumerable<CellMatrix> CellMatrices { get; set; }

        public string ErrorMessage { get; set; }

        public int? IdUserSegment { get; set; } = null;
    }
}
