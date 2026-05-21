using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;
using Microsoft.Extensions.Logging;
using Moq;

namespace HM.Presupuestos.UnitTest.Mantenimientos;

[TestFixture]
public class IndicadoresServiceTests
{
    private Mock<ILogger<IndicadoresService>> _loggerMock;
    private Mock<IIndicadoresRepository> _repositoryMock;
    private Mock<IVersionesService> _versionesServiceMock;
    private Mock<ILogAccionesService> _logAccionesMock;
    private Mock<ITransaccion> _transaccionMock;
    private IndicadoresService _sut;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<IndicadoresService>>();
        _repositoryMock = new Mock<IIndicadoresRepository>();
        _versionesServiceMock = new Mock<IVersionesService>();
        _logAccionesMock = new Mock<ILogAccionesService>();
        _transaccionMock = new Mock<ITransaccion>();

        _repositoryMock
            .Setup(r => r.ObtenerTransaccion())
            .Returns(_transaccionMock.Object);

        _transaccionMock
            .Setup(t => t.CommitAsync())
            .Returns(Task.CompletedTask);

        _transaccionMock
            .Setup(t => t.RollbackAsync())
            .Returns(Task.CompletedTask);

        _sut = new IndicadoresService(
            _loggerMock.Object,
            _repositoryMock.Object,
            _versionesServiceMock.Object,
            _logAccionesMock.Object);
    }

    #region ObtenerIndicadoresConIdiomas

    [Test]
    public async Task ObtenerIndicadoresConIdiomas_SinFiltro_DevuelveListaCompleta()
    {
        // Arrange
        var indicadoresEsperados = new List<Indicador>
        {
            new() { Codigo = 1, Descripcion = "Activo", BitAnd = 1, Orden = 10 },
            new() { Codigo = 2, Descripcion = "Cerrado", BitAnd = 2, Orden = 20 },
        };

        _repositoryMock
            .Setup(r => r.ObtenerIndicadoresConIdiomas(null))
            .ReturnsAsync(indicadoresEsperados);

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
        // Arrange
        _repositoryMock
            .Setup(r => r.ObtenerIndicadoresConIdiomas(null))
            .ReturnsAsync([]);

        // Act
        var resultado = await _sut.ObtenerIndicadoresConIdiomas();

        // Assert
        Assert.That(resultado, Is.Empty);
    }

    [Test]
    public async Task ObtenerIndicadoresConIdiomas_ConFiltro_LlamaAlRepositorioConElFiltro()
    {
        // Arrange
        const string filtro = "Activo";
        _repositoryMock
            .Setup(r => r.ObtenerIndicadoresConIdiomas(filtro))
            .ReturnsAsync([new() { Codigo = 1, Descripcion = "Activo", BitAnd = 1, Orden = 10 }]);

        // Act
        await _sut.ObtenerIndicadoresConIdiomas(filtro);

        // Assert
        _repositoryMock.Verify(r => r.ObtenerIndicadoresConIdiomas(filtro), Times.Once);
    }

    #endregion

    #region Grabar

    [Test]
    public async Task Grabar_IndicadorNuevoSinDuplicados_InsertaYHaceCommit()
    {
        // Arrange
        var indicador = new Indicador { Descripcion = "Nuevo", BitAnd = 4, Orden = 30, Estado = EstadoEntidad.Nuevo };

        _repositoryMock.Setup(r => r.ExisteIndicador(indicador)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.ExisteOrden(indicador)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.ExisteBitAnd(indicador)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.InsertarIndicador(indicador)).ReturnsAsync(99);

        // Act
        await _sut.Grabar(indicador, [], [], []);

        // Assert
        _repositoryMock.Verify(r => r.InsertarIndicador(indicador), Times.Once);
        _transaccionMock.Verify(t => t.CommitAsync(), Times.Once);
        Assert.That(indicador.Codigo, Is.EqualTo(99));
    }

    [Test]
    public void Grabar_DescripcionDuplicada_LanzaValidacionException()
    {
        // Arrange
        var indicador = new Indicador { Descripcion = "Duplicado", BitAnd = 4, Orden = 30, Estado = EstadoEntidad.Nuevo };

        _repositoryMock.Setup(r => r.ExisteIndicador(indicador)).ReturnsAsync(true);

        // Act & Assert
        Assert.ThrowsAsync<ValidacionException>(() => _sut.Grabar(indicador, [], [], []));
    }

    [Test]
    public void Grabar_OrdenDuplicado_LanzaValidacionException()
    {
        // Arrange
        var indicador = new Indicador { Descripcion = "Indicador", BitAnd = 4, Orden = 30, Estado = EstadoEntidad.Nuevo };

        _repositoryMock.Setup(r => r.ExisteIndicador(indicador)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.ExisteOrden(indicador)).ReturnsAsync(true);

        // Act & Assert
        Assert.ThrowsAsync<ValidacionException>(() => _sut.Grabar(indicador, [], [], []));
    }

    [Test]
    public void Grabar_BitAndDuplicado_LanzaValidacionException()
    {
        // Arrange
        var indicador = new Indicador { Descripcion = "Indicador", BitAnd = 4, Orden = 30, Estado = EstadoEntidad.Nuevo };

        _repositoryMock.Setup(r => r.ExisteIndicador(indicador)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.ExisteOrden(indicador)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.ExisteBitAnd(indicador)).ReturnsAsync(true);

        // Act & Assert
        Assert.ThrowsAsync<ValidacionException>(() => _sut.Grabar(indicador, [], [], []));
    }

    [Test]
    public async Task Grabar_IndicadorModificado_ActualizaYHaceCommit()
    {
        // Arrange
        var indicador = new Indicador { Codigo = 5, Descripcion = "Modificado", BitAnd = 4, Orden = 30, Estado = EstadoEntidad.Modificado };

        _repositoryMock.Setup(r => r.ExisteIndicador(indicador)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.ExisteOrden(indicador)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.ExisteBitAnd(indicador)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.ActualizarIndicador(indicador)).Returns(Task.CompletedTask);

        // Act
        await _sut.Grabar(indicador, [], [], []);

        // Assert
        _repositoryMock.Verify(r => r.ActualizarIndicador(indicador), Times.Once);
        _repositoryMock.Verify(r => r.InsertarIndicador(It.IsAny<Indicador>()), Times.Never);
        _transaccionMock.Verify(t => t.CommitAsync(), Times.Once);
    }

    #endregion

    #region ObtenerUltimoBitAnd y ObtenerUltimoOrden

    [Test]
    public async Task ObtenerUltimoBitAnd_DevuelveElValorDelRepositorio()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ObtenerUltimoBitAnd()).ReturnsAsync(16);

        // Act
        var resultado = await _sut.ObtenerUltimoBitAnd();

        // Assert
        Assert.That(resultado, Is.EqualTo(16));
    }

    [Test]
    public async Task ObtenerUltimoOrden_DevuelveElValorDelRepositorio()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ObtenerUltimoOrden()).ReturnsAsync(50);

        // Act
        var resultado = await _sut.ObtenerUltimoOrden();

        // Assert
        Assert.That(resultado, Is.EqualTo(50));
    }

    #endregion
}
