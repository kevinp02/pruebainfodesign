using PruebaInfodesign.Application.DTOs.Request;
using PruebaInfodesign.Application.DTOs.Response;

namespace PruebaInfodesign.Application.Interfaces
{
    public interface IConsumoService
    {
        Task<List<HistoricoConsumoResponse>> ObtenerHistoricoConsumosPorTramos(FechaRangoRequest request);

        Task<List<HistoricoClienteResponse>> ObtenerHistoricoConsumosPorCliente(FechaRangoRequest request);

        Task<List<Top20Response>> ObtenerTop20PeoresTramos(FechaRangoRequest request);
    }
}
