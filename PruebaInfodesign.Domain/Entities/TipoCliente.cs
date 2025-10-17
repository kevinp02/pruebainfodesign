
namespace PruebaInfodesign.Domain.Entities
{
    public class TipoCliente
    {
        public int TipoClienteId { get; set; }
        public string NombreTipo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public virtual ICollection<RegistroConsumo> Registros { get; set; } = new List<RegistroConsumo>();
    }
}