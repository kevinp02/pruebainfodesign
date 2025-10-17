namespace PruebaInfodesign.Domain.Entities
{
    public class RegistroConsumo
    {
        public long RegistroId { get; set; }
        public int LineaId { get; set; }
        public int TipoClienteId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal ConsumoWh { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal PorcentajePerdida { get; set; }
        public decimal CostoTotal => ConsumoWh * CostoUnitario;
        public decimal PerdidasWh => ConsumoWh * (PorcentajePerdida / 100m);
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public virtual Linea Linea { get; set; } = null!;
        public virtual TipoCliente TipoCliente { get; set; } = null!;
    }
}