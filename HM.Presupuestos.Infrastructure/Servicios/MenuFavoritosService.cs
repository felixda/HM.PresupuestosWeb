using HM.Core.Comun.v6.Entidades.Configuracion;
using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Domain.Compartido;

namespace HM.Presupuestos.Infrastructure.Servicios
{
    /// <inheritdoc />
    public sealed class MenuFavoritosService(IClienteApiCore clienteApiCore) : IMenuFavoritosService
    {
        private readonly IClienteApiCore _clienteApiCore = clienteApiCore;

        /// <inheritdoc />
        public async Task<HashSet<string>> ObtenerFavoritos()
        {
            var favoritosTexto = await _clienteApiCore.ObtenerCodigosDeMenusFavoritos();

            if (string.IsNullOrWhiteSpace(favoritosTexto))
            {
                return [];
            }

            return favoritosTexto
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToHashSet(StringComparer.Ordinal);
        }

        /// <inheritdoc />
        public async Task GuardarFavoritos(HashSet<string> favoritos)
        {
            var configuracion = new ElementoConfiguracion
            {
                Nombre = Constantes.UserConfiguration.MENU_FAVORITES,
                Valor = string.Join(",", favoritos)
            };

            await _clienteApiCore.GuardarCodigosDeMenusFavoritos(configuracion);
        }
    }
}
