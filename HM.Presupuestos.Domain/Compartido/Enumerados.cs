
using System.ComponentModel;

namespace HM.Presupuestos.Domain.Compartido
{
    public enum TiposCambiosdeDatos
    {
        SinCambios,
        Modificados,
        Añadidos,
        Eliminados
    }

    public enum TiposCambiosdeDatosPrevisiones
    {
        SinCambios,
        Eliminados,
        Modificados,
        Añadidos
    }

    public enum CodigosMenu
    {
        Home = 0,
        Versiones = 1,
        CrearCargarVersiones = 2,
        PlanificacionCondiciones = 5,
        Indicadores = 12,
        ImportacionCondiciones = 13,
        Sobreprimas = 14,
        ImportacionSobreprimas = 16,
        CopiarInversiones = 19,
        Administracion = 20,
        Avisos = 21,
        Impersonacion = 22,
        MesesBloqueados = 23,
        AnioDiario = 25,
    }

    /// <summary>
    /// Origenes de las versiones. No cambiar nombres ya que se utilizarn en los Pl's de carga
    /// </summary>
    public enum OrigenVersion
    {
        Presupuestos = 1,
        /// <summary>
        /// Tambien denominada Real
        /// </summary>
        Inges = 2,
        PonerA0 = 3
    }

    public enum ModoOperacion
    {
        Ninguna = 0,
        Consultar = 1,
        Insertar = 2,
        Modificar = 3,
        Eliminar = 4
    }

    public enum EstadoEntidad
    {
        SinCambios = 0,
        Modificado = 1,
        Nuevo = 2,
        Eliminado = 3
    }

    public enum CampoErrorValidacion
    {
        Descripcion,
        Orden,
        BitAnd
    }



    public enum ConceptosSobreprimas
    {
        Sobreprima = 1,
        SLA = 2,
        HVP = 3,
    }

    /// <summary>
    /// Conceptos para las condiciones de clientes
    /// Los nombres y valor tienen que coincidir con los del Json -> PlanificacionCondiciones:ListaConceptos
    /// </summary>
    public enum ConceptosCondiciones
    {
        Sag = 1,
        Manpower = 2,
        Devolucion = 3,
        BaseCalculoDevolucion = 4
    }


    public enum ConceptosCondicionesNMD
    {
        Alcance = 1,
        Disciplina = 2,
        Diversified = 3,
        Objetivo = 4,
        TipoCompra = 5,
        TipoDisciplina = 6,
        DisciplinaGrupo = 7
    }

    public enum ModoEdicion
    {
        Alta = 1,
        Edicion = 2
    }

    public enum TiposDeAviso
    {
        Warning = 1,
        Info = 2,
        Success = 3,
        Error = 4
    }


    public enum ReglasUsuario
    {
        Administrador = 47,
        VerAjuste = 48
    }

    public enum AccionesLog
    {
        [Description("Impersonación de Usuario")]
        ImpersonacionUsuario,
        [Description("Impersonación de Usuario inválida")]
        ImpersonacionUsuarioInvalida,
        [Description("Bloquear meses del año")]
        BloquearMesesAño,
        [Description("Modificar Vigencia")]
        ModificarVigencia,
        [Description("Eliminar Vigencia")]
        EliminarVigencia,
        AccederAplicacion,
        [Description("Lanzar proceso copia Inversiones (PKG_COPIAR_PREVISIONES.SET_COPIAR_PREVISIONES)")]
        CopiarInversiones,
        [Description("Poceso copia Inversiones finalizado (PKG_COPIAR_PREVISIONES.SET_COPIAR_PREVISIONES)")]
        CopiarInversionesFinalizado,
        [Description("Lanzar proceso importar Condiciones de MMS (PKG_CARGA_DATOS_CONDICIONES.SET_CARGA_CONDICIONES_MMS)")]
        ImportarCondicionesMMS,
        [Description("Proceso importar Condiciones de MMS finalizado (PKG_CARGA_DATOS_CONDICIONES.SET_CARGA_CONDICIONES_MMS) ")]
        ImportarCondicionesMMSFinalizado,
        [Description("Lanzar proceso copiar Versiones (PKG_CARGA_DATOS_VERSIONES.SET_COPIA)")]
        CopiarVersiones,
        [Description("Proceso copiar Versiones finalizado (PKG_CARGA_DATOS_VERSIONES.SET_COPIA)")]
        CopiarVersionesFinalizado,
        [Description("Entrar en la aplicacion Presupuestos Web con SSO")]
        EntrarEnPresupuestosWebSSO,
        [Description("Desimpersonación de Usuario")]
        DesimpersonacionUsuario,
        [Description("Actualizacion de Usuario")]
        ActualizacionUsuario,
        [Description("Actualización de Sobreprimas")]
        ActualizarSobreprimas,
        [Description("Eliminar Sobreprima")]
        EliminarSobreprima,
        [Description("Lanzar proceso importar sobreprimas de MMS (PKG_CARGA_DATOS_SOBREPRIMAS.SET_CARGA_SOBREPRIMAS_MMS)")]
        ImportarSobreprimasMMS,
        [Description("Recuperar sesión después de F5 con SSO")]
        RecuperarSesionDespuesDeF5SSO,
        [Description("Intento de acceso no autorizado a la página -> (mirar campo Parámetros)")]
        IntentoAccesoNoAutorizado,
        [Description("Entrar en la aplicacion Presupuestos Web con Impersonación")]
        EntrarEnPresupuestosWebImpersonacion,
        [Description("Recuperar sesión después de F5 con Impersonación")]
        RecuperarSesionDespuesDeF5Impersonacion,
        [Description("Cerrar sesión de usuario Impersonado")]
        CerrarSesionUsuarioLogin,
        [Description("Volver a la aplicacion Presupuestos Web con SSO")]
        VolverEntrarEnPresupuestosWebSSO,
        [Description("Eliminar Indicador")]
        EliminarIndicador,
        [Description("Grabar Indicador")]
        GrabarIndicador,

    }

    public enum OrigenValidacionUsuario
    {
        SSO = 1,
        Login = 2
    }

}
