using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Core.Servidor.v6.DAL;
using HM.Core.Servidor.v6.DAL.Interfaces;

namespace HM.Presupuestos.Infrastructure.Persistencia
{
    /// <summary>
    /// Clase base para repositorios de Presupuestos que proporciona funcionalidad común
    /// </summary>
    /// <remarks>
    /// Constructor base que inicializa las dependencias comunes
    /// </remarks>
    /// <param name="dah">Data Access Helper Secure</param>
    /// <param name="jwt">Servicio JWT para obtener información del usuario autenticado</param>

    public abstract class BasePresupuestosRepository(
        IDataAccessHelperSecure dah,
        IJwt jwt) : BaseDAL(dah)
    {
        protected readonly IJwt Jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));

        // Propiedades calculadas en lugar de campos
        // Se evalúan solo cuando se acceden, no en la construcción

        //        Timeline correcto:
        //1.	Startup ? Se registran servicios
        //2.	Constructor ejecuta ? Solo guarda referencia a Jwt ? ? OK
        //3.	Usuario navega y se autentica ? JWT se llena
        //4.	Método usa CodigoAplicacion ? Se evalúa con usuario autenticado ? ? OK
        //---
        //?? Ventajas de las propiedades:
        //Aspecto           Campo(readonly)     Propiedad(=>)               Resultado
        //Evaluación        En constructor      Al acceder	                ? Propiedad
        //Usuario null	    ?? Falla siempre	? Solo si se usa sin auth	? Propiedad
        //Performance	    ? 1 evaluación	    ?? Cada acceso	            ?? Campo
        //Flexibilidad	    ? Fijo	            ? Dinámico	                ? Propiedad
        //Blazor compatible	? Falla	        ? Funciona	                ? Propiedad




        /// <summary>
        /// Código de aplicación del usuario autenticado actual
        /// </summary>
        /// <exception cref="InvalidOperationException">Si no hay un usuario autenticado válido</exception>
        protected int CodigoAplicacion => 
            Jwt.Usuario?.CodigoAplicacion 
            ?? throw new InvalidOperationException("No se puede obtener el código de aplicación: no hay un usuario autenticado válido.");

        /// <summary>
        /// Código del usuario autenticado actual
        /// </summary>
        /// <exception cref="InvalidOperationException">Si no hay un usuario autenticado válido</exception>
        protected int CodigoUsuario => 
            Jwt.Usuario?.CodigoUsuario 
            ?? throw new InvalidOperationException("No se puede obtener el código de usuario: no hay un usuario autenticado válido.");

        /// <summary>
        /// Código del país del usuario autenticado actual
        /// </summary>
        /// <exception cref="InvalidOperationException">Si no hay un usuario autenticado válido</exception>
        protected int CodigoPais => 
            Jwt.Usuario?.CodigoPais 
            ?? throw new InvalidOperationException("No se puede obtener el código de país: no hay un usuario autenticado válido.");

        /// <summary>
        /// Añade el parámetro multicompañía basado en el usuario actual
        /// </summary>
        /// <param name="dahHelper">Data Access Helper al que se le añadirá el parámetro</param>
        /// <exception cref="InvalidOperationException">Si no hay un usuario autenticado</exception>
        protected async Task AñadirParametroMulticompania(IDataAccessHelperBase dahHelper)
        {
            ArgumentNullException.ThrowIfNull(dahHelper);

            var usuario = Jwt.Usuario; 

            if (usuario == null || usuario.CodigoUsuario <= 0)
            {
                throw new InvalidOperationException(
                    "No se puede añadir el parámetro multicompañía: no hay un usuario autenticado válido.");
            }

            dahHelper.AddParameterMulticompania(usuario.Companias);
        }
    }
}
