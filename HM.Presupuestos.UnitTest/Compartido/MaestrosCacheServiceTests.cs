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

        var opciones = Options.Create(new Builders.MaestrosCacheOptionsBuilder().WithTtlMinutos(30).Build());
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

    [Test]
    public async Task ObtenerAlcances_SegundaLlamada_NoInvocaInnerService()
    {
        // Arrange
        var alcances = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "Alcance1" } };
        _innerMock.Setup(s => s.ObtenerAlcances()).ReturnsAsync(alcances);

        // Act
        await _sut.ObtenerAlcances();
        await _sut.ObtenerAlcances();

        // Assert
        _innerMock.Verify(s => s.ObtenerAlcances(), Times.Once);
    }

    [Test]
    public async Task ObtenerDisciplinas_SegundaLlamada_NoInvocaInnerService()
    {
        // Arrange
        var disciplinas = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "Disciplina1" } };
        _innerMock.Setup(s => s.ObtenerDisciplinas()).ReturnsAsync(disciplinas);

        // Act
        await _sut.ObtenerDisciplinas();
        await _sut.ObtenerDisciplinas();

        // Assert
        _innerMock.Verify(s => s.ObtenerDisciplinas(), Times.Once);
    }

    [Test]
    public async Task ObtenerDiversifiedsNCB_SegundaLlamada_NoInvocaInnerService()
    {
        // Arrange
        var diversifieds = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "Div1" } };
        _innerMock.Setup(s => s.ObtenerDiversifiedsNCB()).ReturnsAsync(diversifieds);

        // Act
        await _sut.ObtenerDiversifiedsNCB();
        await _sut.ObtenerDiversifiedsNCB();

        // Assert
        _innerMock.Verify(s => s.ObtenerDiversifiedsNCB(), Times.Once);
    }

    [Test]
    public async Task ObtenerObjetivos_SegundaLlamada_NoInvocaInnerService()
    {
        // Arrange
        var objetivos = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "Objetivo1" } };
        _innerMock.Setup(s => s.ObtenerObjetivos()).ReturnsAsync(objetivos);

        // Act
        await _sut.ObtenerObjetivos();
        await _sut.ObtenerObjetivos();

        // Assert
        _innerMock.Verify(s => s.ObtenerObjetivos(), Times.Once);
    }

    [Test]
    public async Task ObtenerTiposCompra_SegundaLlamada_NoInvocaInnerService()
    {
        // Arrange
        var tiposCompra = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "TipoCompra1" } };
        _innerMock.Setup(s => s.ObtenerTiposCompra()).ReturnsAsync(tiposCompra);

        // Act
        await _sut.ObtenerTiposCompra();
        await _sut.ObtenerTiposCompra();

        // Assert
        _innerMock.Verify(s => s.ObtenerTiposCompra(), Times.Once);
    }

    [Test]
    public async Task ObtenerDisciplinasGrupos_SegundaLlamada_NoInvocaInnerService()
    {
        // Arrange
        var disciplinasGrupo = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "GrupoDisciplina1" } };
        _innerMock.Setup(s => s.ObtenerDisciplinasGrupos()).ReturnsAsync(disciplinasGrupo);

        // Act
        await _sut.ObtenerDisciplinasGrupos();
        await _sut.ObtenerDisciplinasGrupos();

        // Assert
        _innerMock.Verify(s => s.ObtenerDisciplinasGrupos(), Times.Once);
    }

    [Test]
    public async Task ObtenerTiposDisciplinas_SegundaLlamada_NoInvocaInnerService()
    {
        // Arrange
        var tiposDisciplina = new List<CodigoDescripcion> { new() { Codigo = 1, Descripcion = "TipoDisciplina1" } };
        _innerMock.Setup(s => s.ObtenerTiposDisciplinas()).ReturnsAsync(tiposDisciplina);

        // Act
        await _sut.ObtenerTiposDisciplinas();
        await _sut.ObtenerTiposDisciplinas();

        // Assert
        _innerMock.Verify(s => s.ObtenerTiposDisciplinas(), Times.Once);
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
