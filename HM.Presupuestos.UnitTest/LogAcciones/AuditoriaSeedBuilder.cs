using HM.Presupuestos.Domain.Compartido;

namespace HM.Presupuestos.UnitTest.LogAcciones;

internal sealed class AuditoriaSeedBuilder
{
    private AccionesLog _tipo = AccionesLog.EntrarEnPresupuestosWebSSO;
    private DateTime _fechaInicio = new(2026, 6, 1);
    private string _usuario = "usuario";
    private string _descripcion = "descripcion";
    private string _parametros = string.Empty;
    private int? _codigoPagina;

    public AuditoriaSeedBuilder WithTipo(AccionesLog tipo)
    {
        _tipo = tipo;
        return this;
    }

    public AuditoriaSeedBuilder WithFechaInicio(DateTime fechaInicio)
    {
        _fechaInicio = fechaInicio;
        return this;
    }

    public AuditoriaSeedBuilder WithUsuario(string usuario)
    {
        _usuario = usuario;
        return this;
    }

    public AuditoriaSeedBuilder WithDescripcion(string descripcion)
    {
        _descripcion = descripcion;
        return this;
    }

    public AuditoriaSeedBuilder WithParametros(string parametros)
    {
        _parametros = parametros;
        return this;
    }

    public AuditoriaSeedBuilder WithCodigoPagina(int? codigoPagina)
    {
        _codigoPagina = codigoPagina;
        return this;
    }

    public HM.Presupuestos.UnitTest.Fakes.AuditoriaSeed Build()
    {
        return new HM.Presupuestos.UnitTest.Fakes.AuditoriaSeed(
            _tipo,
            _fechaInicio,
            _usuario,
            _descripcion,
            _parametros,
            _codigoPagina);
    }
}
