using HM.Presupuestos.Domain.Entidades.LogAcciones;

namespace HM.Presupuestos.UnitTest.LogAcciones;

internal sealed class LogTecnicoSeedBuilder
{
    private DateTime _fecha = new(2026, 6, 1, 10, 0, 0);
    private string _nivel = "Info";
    private string _categoria = "Metodo";
    private string _usuario = "usuario";
    private string _mensaje = "mensaje";
    private string _logger = "logger";
    private string _excepcion = string.Empty;
    private string _stackTrace = string.Empty;
    private string _comentarios = string.Empty;

    public LogTecnicoSeedBuilder WithFecha(DateTime fecha)
    {
        _fecha = fecha;
        return this;
    }

    public LogTecnicoSeedBuilder WithNivel(string nivel)
    {
        _nivel = nivel;
        return this;
    }

    public LogTecnicoSeedBuilder WithCategoria(string categoria)
    {
        _categoria = categoria;
        return this;
    }

    public LogTecnicoSeedBuilder WithUsuario(string usuario)
    {
        _usuario = usuario;
        return this;
    }

    public LogTecnico Build()
    {
        return new LogTecnico
        {
            Fecha = _fecha,
            Nivel = _nivel,
            Categoria = _categoria,
            Usuario = _usuario,
            Mensaje = _mensaje,
            Logger = _logger,
            Excepcion = _excepcion,
            StackTrace = _stackTrace,
            Comentarios = _comentarios
        };
    }
}