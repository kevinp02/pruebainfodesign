namespace PruebaInfodesign.Application.DTOs.Response
{
    public class HistoricoClienteResponse
    {
        public string TipoCliente { get; set; } = string.Empty;
        public List<DetalleTramo> Historico { get; set; } = new();
    }

    public class DetalleTramo
    {
        public DateTime Fecha { get; set; }
        public string CodigoLinea { get; set; } = string.Empty;
        public decimal ConsumoWh { get; set; }
        public decimal PerdidasWh { get; set; }
        public decimal CostoTotal { get; set; }
    }
}
