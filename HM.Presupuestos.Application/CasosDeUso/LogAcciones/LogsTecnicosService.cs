using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.Application.CasosDeUso.LogAcciones
{
    public class LogsTecnicosService(ILogsTecnicosRepository logsTecnicosRepository) : ILogsTecnicosService
    {
        private readonly ILogsTecnicosRepository _logsTecnicosRepository = logsTecnicosRepository;

        public async Task<List<LogTecnico>> ObtenerLogs(FiltroLogsTecnicos filtro)
        {
            return await _logsTecnicosRepository.ObtenerLogs(filtro);
        }

        public async Task<List<CodigoDescripcion>> ObtenerNivelesDisponibles()
        {
            return await _logsTecnicosRepository.ObtenerNivelesDisponibles();
        }

        public async Task<List<CodigoDescripcion>> ObtenerUsuariosDisponibles()
        {
            var logs = await _logsTecnicosRepository.ObtenerLogs(new FiltroLogsTecnicos());

            return logs
                .Select(log => NormalizarUsuario(log.Usuario))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(usuario => usuario, StringComparer.OrdinalIgnoreCase)
                .Select((usuario, indice) => new CodigoDescripcion
                {
                    Codigo = indice + 1,
                    Descripcion = usuario
                })
                .ToList();
        }

        private static string NormalizarUsuario(string? usuario)
        {
            return string.IsNullOrWhiteSpace(usuario)
                ? LogsTecnicosConstantes.UsuarioSinUsuario
                : usuario.Trim();
        }
    }
}