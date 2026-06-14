using HM.Core.Comun.v6.Loggers.Interfaces;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.Domain.Entidades;


namespace HM.Presupuestos.Application.CasosDeUso
{
    public class ConfiguracionService(ILogger logger, IConfiguracionRepository configuracionRepository) : IConfiguracionService
    {

        #region Mantenimiento

        public async Task<CodigoDescripcion> ObtenerAnioDiario()
        {
            logger.Trace($"Llamando método ObtenerAnioDiario");

            return await configuracionRepository.ObtenerAnioDiario();
        }

        public async Task ActualizarAnioDiario(int anio)
        {
            logger.Trace($"Llamando método ActualizarAnioDiario");
            await configuracionRepository.ActualizarAnioDiario(anio);
        }

        #endregion
    }
}
