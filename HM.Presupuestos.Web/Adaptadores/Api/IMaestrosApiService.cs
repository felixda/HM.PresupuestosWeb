using HM.Presupuestos.Web.Adaptadores.Api.Modelos;

namespace HM.Presupuestos.Web.Adaptadores.Api;

public interface IMaestrosApiService
{
    Task<List<MaestroApiItem>> ObtenerTipologias();
    Task<List<MaestroApiItem>> ObtenerNetworks();
    Task<List<MaestroApiItem>> ObtenerMedios();
    Task<List<MaestroApiItem>> ObtenerGruposClientes();
}
