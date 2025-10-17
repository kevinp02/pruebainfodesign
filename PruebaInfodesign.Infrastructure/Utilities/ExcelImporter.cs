using OfficeOpenXml;
using PruebaInfodesign.Domain.Entities;
using PruebaInfodesign.Infrastructure.Data;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace PruebaInfodesign.Infrastructure.Utilities
{
    public class ExcelImporter
    {
        private readonly Context _context;

        public ExcelImporter(Context context)
        {
            _context = context;
            ExcelPackage.License.SetNonCommercialOrganization("My Noncommercial organization");
        }

        public async Task<ImportResult> ImportarDesdeExcel(string rutaArchivo)
        {
            var resultado = new ImportResult();

            try
            {
                using var package = new ExcelPackage(new FileInfo(rutaArchivo));

                var hojaConsumo = package.Workbook.Worksheets["CONSUMO_POR_TRAMO"]
                    ?? throw new Exception("No se encontro la pestaña 'Consumo'");
                var hojaCostos = package.Workbook.Worksheets["COSTOS_POR_TRAMO"]
                    ?? throw new Exception("No se encontro la pestaña 'Costos'");
                var hojaPerdidas = package.Workbook.Worksheets["PERDIDAS_POR_TRAMO"]
                    ?? throw new Exception("No se encontro la pestaña 'Perdidas'");

                var datosConsumo = LeerHojaConsumo(hojaConsumo);
                var datosCostos = LeerHojaCostos(hojaCostos);
                var datosPerdidas = LeerHojaPerdidas(hojaPerdidas);

                var lineasUnicas = datosConsumo.Keys
                    .Union(datosCostos.Keys)
                    .Union(datosPerdidas.Keys)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                await CrearLineas(lineasUnicas);

                var registros = await ConsolidarDatos(datosConsumo, datosCostos, datosPerdidas);

                await InsertarEnLotes(registros, 1000);

                resultado.Exitoso = true;
                resultado.RegistrosImportados = registros.Count;
                resultado.Mensaje = $"Importacion exitosa";
            }
            catch (Exception ex)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = $"Error: {ex.Message}";
            }

            return resultado;
        }

        private Dictionary<string, List<DatoConsumo>> LeerHojaConsumo(ExcelWorksheet hoja)
        {
            var datos = new Dictionary<string, List<DatoConsumo>>();
            int filaInicio = 2;
            int filaFin = hoja.Dimension.End.Row;

            for (int fila = filaInicio; fila <= filaFin; fila++)
            {
                var linea = hoja.Cells[fila, 1].Text;
                var fechaTexto = hoja.Cells[fila, 2].Text;

                if (string.IsNullOrWhiteSpace(linea) || string.IsNullOrWhiteSpace(fechaTexto))
                    continue;

                var fecha = ParsearFecha(fechaTexto);
                var residencial = ParsearDecimal(hoja.Cells[fila, 3].Text);
                var comercial = ParsearDecimal(hoja.Cells[fila, 4].Text);
                var industrial = ParsearDecimal(hoja.Cells[fila, 5].Text);

                if (!datos.ContainsKey(linea))
                    datos[linea] = new List<DatoConsumo>();

                datos[linea].Add(new DatoConsumo
                {
                    Fecha = fecha,
                    Residencial = residencial,
                    Comercial = comercial,
                    Industrial = industrial
                });
            }

            return datos;
        }

        private Dictionary<string, List<DatoCosto>> LeerHojaCostos(ExcelWorksheet hoja)
        {
            var datos = new Dictionary<string, List<DatoCosto>>();
            int filaInicio = 2;
            int filaFin = hoja.Dimension.End.Row;

            for (int fila = filaInicio; fila <= filaFin; fila++)
            {
                var linea = hoja.Cells[fila, 1].Text;
                var fechaTexto = hoja.Cells[fila, 2].Text;

                if (string.IsNullOrWhiteSpace(linea) || string.IsNullOrWhiteSpace(fechaTexto))
                    continue;

                var fecha = ParsearFecha(fechaTexto);
                var residencial = ParsearDecimal(hoja.Cells[fila, 3].Text);
                var comercial = ParsearDecimal(hoja.Cells[fila, 4].Text);
                var industrial = ParsearDecimal(hoja.Cells[fila, 5].Text);

                if (!datos.ContainsKey(linea))
                    datos[linea] = new List<DatoCosto>();

                datos[linea].Add(new DatoCosto
                {
                    Fecha = fecha,
                    Residencial = residencial,
                    Comercial = comercial,
                    Industrial = industrial
                });
            }

            return datos;
        }

        private Dictionary<string, List<DatoPerdida>> LeerHojaPerdidas(ExcelWorksheet hoja)
        {
            var datos = new Dictionary<string, List<DatoPerdida>>();
            int filaInicio = 2;
            int filaFin = hoja.Dimension.End.Row;

            for (int fila = filaInicio; fila <= filaFin; fila++)
            {
                var linea = hoja.Cells[fila, 1].Text;
                var fechaTexto = hoja.Cells[fila, 2].Text;

                if (string.IsNullOrWhiteSpace(linea) || string.IsNullOrWhiteSpace(fechaTexto))
                    continue;

                var fecha = ParsearFecha(fechaTexto);
                var residencial = ParsearDecimal(hoja.Cells[fila, 3].Text);
                var comercial = ParsearDecimal(hoja.Cells[fila, 4].Text);
                var industrial = ParsearDecimal(hoja.Cells[fila, 5].Text);

                if (!datos.ContainsKey(linea))
                    datos[linea] = new List<DatoPerdida>();

                datos[linea].Add(new DatoPerdida
                {
                    Fecha = fecha,
                    Residencial = residencial,
                    Comercial = comercial,
                    Industrial = industrial
                });
            }

            return datos;
        }

        private async Task CrearLineas(List<string> codigosLinea)
        {
            var lineasExistentes = await _context.Lineas
                .Where(l => codigosLinea.Contains(l.CodigoLinea))
                .Select(l => l.CodigoLinea)
                .ToListAsync();

            var lineasNuevas = codigosLinea
                .Except(lineasExistentes)
                .Select(codigo => new Linea
                {
                    CodigoLinea = codigo,
                    Descripcion = $"Linea {codigo}",
                    Activo = true
                })
                .ToList();

            if (lineasNuevas.Any())
            {
                await _context.Lineas.AddRangeAsync(lineasNuevas);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<List<RegistroConsumo>> ConsolidarDatos(
            Dictionary<string, List<DatoConsumo>> consumos,
            Dictionary<string, List<DatoCosto>> costos,
            Dictionary<string, List<DatoPerdida>> perdidas)
        {
            var registros = new List<RegistroConsumo>();

            var lineas = await _context.Lineas.ToDictionaryAsync(l => l.CodigoLinea, l => l.LineaId);
            var tiposCliente = await _context.TiposCliente.ToDictionaryAsync(t => t.NombreTipo, t => t.TipoClienteId);

            foreach (var lineaCodigo in consumos.Keys)
            {
                if (!lineas.ContainsKey(lineaCodigo))
                    continue;

                var lineaId = lineas[lineaCodigo];
                var consumosLinea = consumos[lineaCodigo];
                var costosLinea = costos.ContainsKey(lineaCodigo) ? costos[lineaCodigo] : new List<DatoCosto>();
                var perdidasLinea = perdidas.ContainsKey(lineaCodigo) ? perdidas[lineaCodigo] : new List<DatoPerdida>();

                foreach (var consumo in consumosLinea)
                {
                    var costo = costosLinea.FirstOrDefault(c => c.Fecha.Date == consumo.Fecha.Date);
                    var perdida = perdidasLinea.FirstOrDefault(p => p.Fecha.Date == consumo.Fecha.Date);

                    if (consumo.Residencial > 0)
                    {
                        registros.Add(new RegistroConsumo
                        {
                            LineaId = lineaId,
                            TipoClienteId = tiposCliente["Residencial"],
                            Fecha = consumo.Fecha.Date,
                            ConsumoWh = consumo.Residencial,
                            CostoUnitario = costo?.Residencial ?? 0,
                            PorcentajePerdida = perdida?.Residencial ?? 0
                        });
                    }

                    if (consumo.Comercial > 0)
                    {
                        registros.Add(new RegistroConsumo
                        {
                            LineaId = lineaId,
                            TipoClienteId = tiposCliente["Comercial"],
                            Fecha = consumo.Fecha.Date,
                            ConsumoWh = consumo.Comercial,
                            CostoUnitario = costo?.Comercial ?? 0,
                            PorcentajePerdida = perdida?.Comercial ?? 0
                        });
                    }

                    if (consumo.Industrial > 0)
                    {
                        registros.Add(new RegistroConsumo
                        {
                            LineaId = lineaId,
                            TipoClienteId = tiposCliente["Industrial"],
                            Fecha = consumo.Fecha.Date,
                            ConsumoWh = consumo.Industrial,
                            CostoUnitario = costo?.Industrial ?? 0,
                            PorcentajePerdida = perdida?.Industrial ?? 0
                        });
                    }
                }
            }

            return registros;
        }

        private async Task InsertarEnLotes(List<RegistroConsumo> registros, int tamañoLote)
        {
            for (int i = 0; i < registros.Count; i += tamañoLote)
            {
                var lote = registros.Skip(i).Take(tamañoLote).ToList();
                await _context.RegistrosConsumo.AddRangeAsync(lote);
                await _context.SaveChangesAsync();
            }
        }

        private DateTime ParsearFecha(string fecha)
        {
            string[] formatos = { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy" };

            foreach (var formato in formatos)
            {
                if (DateTime.TryParseExact(fecha, formato, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime resultado))
                {
                    return resultado;
                }
            }

            return DateTime.Parse(fecha);
        }

        private decimal ParsearDecimal(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            valor = valor.Replace(",", ".");
            return decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado)
                ? resultado
                : 0;
        }
    }
    public class DatoConsumo
    {
        public DateTime Fecha { get; set; }
        public decimal Residencial { get; set; }
        public decimal Comercial { get; set; }
        public decimal Industrial { get; set; }
    }

    public class DatoCosto
    {
        public DateTime Fecha { get; set; }
        public decimal Residencial { get; set; }
        public decimal Comercial { get; set; }
        public decimal Industrial { get; set; }
    }

    public class DatoPerdida
    {
        public DateTime Fecha { get; set; }
        public decimal Residencial { get; set; }
        public decimal Comercial { get; set; }
        public decimal Industrial { get; set; }
    }

    public class ImportResult
    {
        public bool Exitoso { get; set; }
        public int RegistrosImportados { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
