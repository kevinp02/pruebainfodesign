namespace PruebaInfodesign.Application.DTOs.Response
{
    public class HistoricoConsumoResponse
    {
        public string CodigoLinea { get; set; } = string.Empty;
        public List<DetalleConsumo> Historico { get; set; } = new();
    }

    public class DetalleConsumo
    {
        public DateTime Fecha { get; set; }
        public string TipoCliente { get; set; } = string.Empty;
        public decimal ConsumoWh { get; set; }
        public decimal PerdidasWh { get; set; }
        public decimal CostoTotal { get; set; }
    }
}
