namespace PruebaInfodesign.Application.DTOs.Response
{
    public class Top20Response
    {
        public int Ranking { get; set; }
        public string CodigoLinea { get; set; } = string.Empty;
        public string TipoCliente { get; set; } = string.Empty;
        public decimal TotalPerdidasWh { get; set; }
        public decimal TotalConsumoWh { get; set; }
        public decimal PorcentajePerdidasPromedio { get; set; }
        public decimal CostoTotalPerdidas { get; set; }
    }
}
