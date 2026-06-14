using HM.Presupuestos.Application.CasosDeUso.Compartido;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using Microsoft.Extensions.Options;

namespace HM.Presupuestos.UnitTest.Compartido;

[TestFixture]
public class MaestrosCacheServiceTests
{
    private Mock<IMaestrosService> _innerMock;
    private MaestrosCacheService _sut;

    [SetUp]
    public void SetUp()
    {
        _innerMock = new Mock<IMaestrosService>();

        var opciones = Options.Create(new MaestrosCacheOptions { TtlMinutos = 30 });
        _sut = new MaestrosCacheService(_innerMock.Object, opciones);
    }

    #region 4.1 El segundo llamado no invoca al inner service

    [Test]
    public async Task ObtenerNetworks_SegundaLlamada_NoInvocaInnerService()
    {
        // Arrange
        var networks = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "NET1" } };
        _innerMock.Setup(s => s.ObtenerNetworks()).ReturnsAsync(networks);

        // Act
        await _sut.ObtenerNetworks();
        await _sut.ObtenerNetworks();

        // Assert
        _innerMock.Verify(s => s.ObtenerNetworks(), Times.Once);
    }

    [Test]
    public async Task ObtenerMediosPorNetWork_SegundaLlamadaMismosParams_NoInvocaInnerService()
    {
        // Arrange
        const string codigos = "1,2,3";
        var medios = new List<CodigoDescripcion> { new() { Codigo = 10, Descripcion = "TV" } };
        _innerMock.Setup(s => s.ObtenerMediosPorNetWork(codigos)).ReturnsAsync(medios);

        // Act
        await _sut.ObtenerMediosPorNetWork(codigos);
        await _sut.ObtenerMediosPorNetWork(codigos);

        // Assert
        _innerMock.Verify(s => s.ObtenerMediosPorNetWork(codigos), Times.Once);
    }

    [Test]
    public async Task ObtenerAgrupacionesYEditoriales_SegundaLlamada_NoInvocaInnerService()
    {
        // Arrange
        const string codigos = "10,20";
        var resultado = (new List<CodigoDescripcion>(), new List<CodigoDescripcion>());
        _innerMock.Setup(s => s.ObtenerAgrupacionesYEditoriales(codigos)).ReturnsAsync(resultado);

        // Act
        await _sut.ObtenerAgrupacionesYEditoriales(codigos);
        await _sut.ObtenerAgrupacionesYEditoriales(codigos);

        // Assert
        _innerMock.Verify(s => s.ObtenerAgrupacionesYEditoriales(codigos), Times.Once);
    }

    #endregion

    #region 4.2 Invalidar(recurso) fuerza recarga en la siguiente llamada

    [Test]
    public async Task Invalidar_RecursoNetworks_FuerzaRecargaSiguienteLlamada()
    {
        // Arrange
        var networks = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "NET1" } };
        _innerMock.Setup(s => s.ObtenerNetworks()).ReturnsAsync(networks);

        await _sut.ObtenerNetworks();       // primera: carga y cachea

        // Act
        _sut.Invalidar("networks");
        await _sut.ObtenerNetworks();       // segunda: debe recargar

        // Assert
        _innerMock.Verify(s => s.ObtenerNetworks(), Times.Exactly(2));
    }

    [Test]
    public async Task Invalidar_RecursoAjeno_NoAfectaOtrasEntradas()
    {
        // Arrange
        var networks = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "NET1" } };
        _innerMock.Setup(s => s.ObtenerNetworks()).ReturnsAsync(networks);

        await _sut.ObtenerNetworks();       // cachea networks

        // Act — invalida una clave que no existe
        _sut.Invalidar("recurso-inexistente");
        await _sut.ObtenerNetworks();       // debe seguir usando la caché

        // Assert
        _innerMock.Verify(s => s.ObtenerNetworks(), Times.Once);
    }

    #endregion

    #region 4.3 InvalidarTodo limpia todas las entradas del usuario

    [Test]
    public async Task InvalidarTodo_LimpiaTodasLasEntradas()
    {
        // Arrange
        var networks = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "NET1" } };
        var medios = new List<CodigoDescripcion> { new() { Codigo = 10, Descripcion = "TV" } };
        _innerMock.Setup(s => s.ObtenerNetworks()).ReturnsAsync(networks);
        _innerMock.Setup(s => s.ObtenerMedios()).ReturnsAsync(medios);

        await _sut.ObtenerNetworks();   // cachea networks
        await _sut.ObtenerMedios();     // cachea medios

        // Act
        _sut.InvalidarTodo();
        await _sut.ObtenerNetworks();   // debe recargar
        await _sut.ObtenerMedios();     // debe recargar

        // Assert
        _innerMock.Verify(s => s.ObtenerNetworks(), Times.Exactly(2));
        _innerMock.Verify(s => s.ObtenerMedios(), Times.Exactly(2));
    }

    #endregion
}
