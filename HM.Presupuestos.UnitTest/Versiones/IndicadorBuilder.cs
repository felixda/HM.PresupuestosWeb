using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;

namespace HM.Presupuestos.UnitTest.Versiones;

internal sealed class IndicadorBuilder
{
    private int? _codigo;
    private string _descripcion = "Indicador";
    private int _bitAnd = 1;
    private int _orden = 10;
    private EstadoEntidad _estado = EstadoEntidad.Nuevo;

    public IndicadorBuilder WithCodigo(int? codigo)
    {
        _codigo = codigo;
        return this;
    }

    public IndicadorBuilder WithDescripcion(string descripcion)
    {
        _descripcion = descripcion;
        return this;
    }

    public IndicadorBuilder WithBitAnd(int bitAnd)
    {
        _bitAnd = bitAnd;
        return this;
    }

    public IndicadorBuilder WithOrden(int orden)
    {
        _orden = orden;
        return this;
    }

    public IndicadorBuilder WithEstado(EstadoEntidad estado)
    {
        _estado = estado;
        return this;
    }

    public Indicador Build()
    {
        return new Indicador
        {
            Codigo = _codigo,
            Descripcion = _descripcion,
            BitAnd = _bitAnd,
            Orden = _orden,
            Estado = _estado
        };
    }
}
