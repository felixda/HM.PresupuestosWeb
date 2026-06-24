using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using Microsoft.Extensions.Logging;

namespace HM.Presupuestos.UnitTest.Mantenimientos;

[TestFixture]
public class IndicadoresServiceTests
{
    private Mock<ILogger<IndicadoresService>> _loggerMock = null!;
    private InMemoryIndicadoresRepository _repository = null!;
    private Mock<IVersionesService> _versionesServiceMock = null!;
    private Mock<ILogAccionesService> _logAccionesMock = null!;
    private IndicadoresService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<IndicadoresService>>();
        _repository = new InMemoryIndicadoresRepository();
        _versionesServiceMock = new Mock<IVersionesService>();
        _logAccionesMock = new Mock<ILogAccionesService>();

        _sut = new IndicadoresService(
            _loggerMock.Object,
            _repository,
            _versionesServiceMock.Object,
            _logAccionesMock.Object);
    }

    #region ObtenerIndicadoresConIdiomas

    [Test]
    public async Task ObtenerIndicadoresConIdiomas_SinFiltro_DevuelveListaCompleta()
    {
        // Arrange
        _repository.SembrarIndicadores(
            new IndicadorBuilder().WithCodigo(1).WithDescripcion("Activo").WithBitAnd(1).WithOrden(10).Build(),
            new IndicadorBuilder().WithCodigo(2).WithDescripcion("Cerrado").WithBitAnd(2).WithOrden(20).Build());

        // Act
        var resultado = await _sut.ObtenerIndicadoresConIdiomas();

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(2));
        Assert.That(resultado[0].Descripcion, Is.EqualTo("Activo"));
        Assert.That(resultado[1].Descripcion, Is.EqualTo("Cerrado"));
    }

    [Test]
    public async Task ObtenerIndicadoresConIdiomas_SinResultados_DevuelveListaVacia()
    {
        // Act
        var resultado = await _sut.ObtenerIndicadoresConIdiomas();

        // Assert
        Assert.That(resultado, Is.Empty);
    }

    [Test]
    public async Task ObtenerIndicadoresConIdiomas_ConFiltro_DevuelveSoloCoincidencias()
    {
        // Arrange
        _repository.SembrarIndicadores(
            new IndicadorBuilder().WithCodigo(1).WithDescripcion("Activo").Build(),
            new IndicadorBuilder().WithCodigo(2).WithDescripcion("Cerrado").Build());

        // Act
        var resultado = await _sut.ObtenerIndicadoresConIdiomas("Activo");

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(1));
        Assert.That(resultado[0].Descripcion, Is.EqualTo("Activo"));
    }

    #endregion

    #region Grabar

    [Test]
    public async Task Grabar_IndicadorNuevoSinDuplicados_InsertaYHaceCommit()
    {
        // Arrange
        var indicador = new IndicadorBuilder()
            .WithDescripcion("Nuevo")
            .WithBitAnd(4)
            .WithOrden(30)
            .WithEstado(EstadoEntidad.Nuevo)
            .Build();

        // Act
        await _sut.Grabar(indicador, [], [], []);

        // Assert
        Assert.That(indicador.Codigo, Is.Not.Null);
        Assert.That(_repository.Indicadores.ContainsKey(indicador.Codigo!.Value), Is.True);
        Assert.That(_repository.UltimaTransaccion.CommitInvocado, Is.True);
    }

    [Test]
    public void Grabar_DescripcionDuplicada_LanzaValidacionException()
    {
        // Arrange
        _repository.SembrarIndicadores(
            new IndicadorBuilder().WithCodigo(1).WithDescripcion("Duplicado").WithBitAnd(8).WithOrden(40).Build());

        var indicador = new IndicadorBuilder()
            .WithDescripcion("Duplicado")
            .WithBitAnd(4)
            .WithOrden(30)
            .WithEstado(EstadoEntidad.Nuevo)
            .Build();

        // Act & Assert
        Assert.ThrowsAsync<ValidacionException>(() => _sut.Grabar(indicador, [], [], []));
    }

    [Test]
    public void Grabar_OrdenDuplicado_LanzaValidacionException()
    {
        // Arrange
        _repository.SembrarIndicadores(
            new IndicadorBuilder().WithCodigo(1).WithDescripcion("Base").WithBitAnd(8).WithOrden(30).Build());

        var indicador = new IndicadorBuilder()
            .WithDescripcion("Indicador")
            .WithBitAnd(4)
            .WithOrden(30)
            .WithEstado(EstadoEntidad.Nuevo)
            .Build();

        // Act & Assert
        Assert.ThrowsAsync<ValidacionException>(() => _sut.Grabar(indicador, [], [], []));
    }

    [Test]
    public void Grabar_BitAndDuplicado_LanzaValidacionException()
    {
        // Arrange
        _repository.SembrarIndicadores(
            new IndicadorBuilder().WithCodigo(1).WithDescripcion("Base").WithBitAnd(4).WithOrden(40).Build());

        var indicador = new IndicadorBuilder()
            .WithDescripcion("Indicador")
            .WithBitAnd(4)
            .WithOrden(30)
            .WithEstado(EstadoEntidad.Nuevo)
            .Build();

        // Act & Assert
        Assert.ThrowsAsync<ValidacionException>(() => _sut.Grabar(indicador, [], [], []));
    }

    [Test]
    public async Task Grabar_IndicadorModificado_ActualizaYHaceCommit()
    {
        // Arrange
        _repository.SembrarIndicadores(
            new IndicadorBuilder().WithCodigo(5).WithDescripcion("Original").WithBitAnd(4).WithOrden(30).Build());

        var indicador = new IndicadorBuilder()
            .WithCodigo(5)
            .WithDescripcion("Modificado")
            .WithBitAnd(4)
            .WithOrden(30)
            .WithEstado(EstadoEntidad.Modificado)
            .Build();

        // Act
        await _sut.Grabar(indicador, [], [], []);

        // Assert
        Assert.That(_repository.Indicadores[5].Descripcion, Is.EqualTo("Modificado"));
        Assert.That(_repository.UltimaTransaccion.CommitInvocado, Is.True);
    }

    #endregion

    #region ObtenerUltimoBitAnd y ObtenerUltimoOrden

    [Test]
    public async Task ObtenerUltimoBitAnd_DevuelveElValorDelRepositorio()
    {
        // Arrange
        _repository.SembrarIndicadores(
            new IndicadorBuilder().WithCodigo(1).WithBitAnd(16).WithOrden(10).Build(),
            new IndicadorBuilder().WithCodigo(2).WithBitAnd(8).WithOrden(20).Build());

        // Act
        var resultado = await _sut.ObtenerUltimoBitAnd();

        // Assert
        Assert.That(resultado, Is.EqualTo(16));
    }

    [Test]
    public async Task ObtenerUltimoOrden_DevuelveElValorDelRepositorio()
    {
        // Arrange
        _repository.SembrarIndicadores(
            new IndicadorBuilder().WithCodigo(1).WithBitAnd(2).WithOrden(50).Build(),
            new IndicadorBuilder().WithCodigo(2).WithBitAnd(4).WithOrden(10).Build());

        // Act
        var resultado = await _sut.ObtenerUltimoOrden();

        // Assert
        Assert.That(resultado, Is.EqualTo(50));
    }

    #endregion
}
