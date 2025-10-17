using System.ComponentModel.DataAnnotations;

namespace PruebaInfodesign.Application.DTOs.Request
{
    public class FechaRangoRequest
    {
        [Required(ErrorMessage = "La fecha inicial es requerida")]
        public DateTime FechaInicial { get; set; }

        [Required(ErrorMessage = "La fecha final es requerida")]
        public DateTime FechaFinal { get; set; }

        public void Validate()
        {
            if (FechaInicial > FechaFinal)
            {
                throw new ArgumentException("La fecha inicial no puede ser mayor que la fecha final");
            }

            if (FechaFinal > DateTime.Now)
            {
                throw new ArgumentException("La fecha final no puede ser mayor que la fecha actual");
            }
        }
    }
}
