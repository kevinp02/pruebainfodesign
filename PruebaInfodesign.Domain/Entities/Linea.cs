
namespace PruebaInfodesign.Domain.Entities
{
    public class Linea
    {
        public int LineaId { get; set; }
        public string CodigoLinea { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public virtual ICollection<RegistroConsumo> Registros { get; set; } = new List<RegistroConsumo>();
    }
}