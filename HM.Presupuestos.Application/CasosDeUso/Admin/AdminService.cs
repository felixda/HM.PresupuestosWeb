using HM.Core.Comun.v6.Loggers.Interfaces;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.Application.CasosDeUso
{
    public class AdminService(ILogger logger, IAdminRepository adminRepository) : IAdminService
    {

        #region Meses bloqueados

        public async Task<List<int>> ObtenerMesesBloqueados(int anio)
        {
            logger.Trace($"Llamando método ObtenerVigencias");

            return await adminRepository.ObtenerMesesBloqueados(anio);
        }


      
        public async Task InsertarMesesBloqueado(int anio, List<int> meses)
        {
            var transaction = adminRepository.ObtenerTransaccion();
            try
            {
                logger.Trace($"Llamando método InsertarMesBloqueado");

                await adminRepository.EliminarMesesBloqueados(anio);

                foreach (int mes in meses)
                {
                    await adminRepository.InsertarMesBloqueado(anio, mes);
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

       

        #endregion

    }
}
