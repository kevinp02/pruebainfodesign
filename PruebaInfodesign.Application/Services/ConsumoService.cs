using PruebaInfodesign.Application.DTOs.Request;
using PruebaInfodesign.Application.DTOs.Response;
using PruebaInfodesign.Application.Interfaces;
using PruebaInfodesign.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PruebaInfodesign.Application.Services
{
    public class ConsumoService : IConsumoService
    {
        private readonly DbContext _context;

        public ConsumoService(DbContext context)
        {
            _context = context;
        }

        public async Task<List<HistoricoConsumoResponse>> ObtenerHistoricoConsumosPorTramos(FechaRangoRequest request)
        {
            request.Validate();

            var registros = await _context.Set<RegistroConsumo>()
                .Include(r => r.Linea)
                .Include(r => r.TipoCliente)
                .Where(r => r.Fecha >= request.FechaInicial && r.Fecha <= request.FechaFinal)
                .OrderBy(r => r.Linea.CodigoLinea)
                .ThenBy(r => r.Fecha)
                .ToListAsync();

            var resultado = registros
                .GroupBy(r => r.Linea.CodigoLinea)
                .Select(g => new HistoricoConsumoResponse
                {
                    CodigoLinea = g.Key,
                    Historico = g.Select(r => new DetalleConsumo
                    {
                        Fecha = r.Fecha,
                        TipoCliente = r.TipoCliente.NombreTipo,
                        ConsumoWh = r.ConsumoWh,
                        PerdidasWh = r.PerdidasWh,
                        CostoTotal = r.CostoTotal
                    }).ToList()
                })
                .ToList();

            return resultado;
        }

        public async Task<List<HistoricoClienteResponse>> ObtenerHistoricoConsumosPorCliente(FechaRangoRequest request)
        {
            request.Validate();

            var registros = await _context.Set<RegistroConsumo>()
                .Include(r => r.Linea)
                .Include(r => r.TipoCliente)
                .Where(r => r.Fecha >= request.FechaInicial && r.Fecha <= request.FechaFinal)
                .OrderBy(r => r.TipoCliente.NombreTipo)
                .ThenBy(r => r.Fecha)
                .ToListAsync();

            var resultado = registros
                .GroupBy(r => r.TipoCliente.NombreTipo)
                .Select(g => new HistoricoClienteResponse
                {
                    TipoCliente = g.Key,
                    Historico = g.Select(r => new DetalleTramo
                    {
                        Fecha = r.Fecha,
                        CodigoLinea = r.Linea.CodigoLinea,
                        ConsumoWh = r.ConsumoWh,
                        PerdidasWh = r.PerdidasWh,
                        CostoTotal = r.CostoTotal
                    }).ToList()
                })
                .ToList();

            return resultado;
        }

        public async Task<List<Top20Response>> ObtenerTop20PeoresTramos(FechaRangoRequest request)
        {
            request.Validate();

            var resultado = await _context.Set<RegistroConsumo>()
                .Include(r => r.Linea)
                .Include(r => r.TipoCliente)
                .Where(r => r.Fecha >= request.FechaInicial && r.Fecha <= request.FechaFinal)
                .GroupBy(r => new { r.Linea.CodigoLinea, r.TipoCliente.NombreTipo })
                .Select(g => new
                {
                    g.Key.CodigoLinea,
                    g.Key.NombreTipo,
                    TotalPerdidasWh = g.Sum(r => r.ConsumoWh * (r.PorcentajePerdida / 100m)),
                    TotalConsumoWh = g.Sum(r => r.ConsumoWh),
                    PorcentajePerdidasPromedio = g.Average(r => r.PorcentajePerdida),
                    CostoTotalPerdidas = g.Sum(r => (r.ConsumoWh * (r.PorcentajePerdida / 100m)) * r.CostoUnitario)
                })
                .OrderByDescending(x => x.TotalPerdidasWh)
                .Take(20)
                .ToListAsync();

            var top20 = resultado
                .Select((item, index) => new Top20Response
                {
                    Ranking = index + 1,
                    CodigoLinea = item.CodigoLinea,
                    TipoCliente = item.NombreTipo,
                    TotalPerdidasWh = item.TotalPerdidasWh,
                    TotalConsumoWh = item.TotalConsumoWh,
                    PorcentajePerdidasPromedio = item.PorcentajePerdidasPromedio,
                    CostoTotalPerdidas = item.CostoTotalPerdidas
                })
                .ToList();

            return top20;
        }
    }
}
