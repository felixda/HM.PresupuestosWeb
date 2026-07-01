using HM.Presupuestos.Domain.Entidades.LogAcciones;

namespace HM.Presupuestos.UnitTest.Builders;

internal sealed class FiltroLogsTecnicosBuilder
{
    private readonly FiltroLogsTecnicos _f = new();

    public FiltroLogsTecnicosBuilder WithNivel(string nivel) { _f.Nivel = nivel; return this; }
    public FiltroLogsTecnicosBuilder WithUsuario(string usuario) { _f.Usuario = usuario; return this; }
    public FiltroLogsTecnicosBuilder WithCategoria(string categoria) { _f.Categoria = categoria; return this; }
    public FiltroLogsTecnicosBuilder WithFechaDesde(System.DateTime fecha) { _f.FechaDesde = fecha; return this; }
    public FiltroLogsTecnicosBuilder WithFechaHasta(System.DateTime fecha) { _f.FechaHasta = fecha; return this; }

    public FiltroLogsTecnicos Build() => _f;
}
