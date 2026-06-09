using System.Text.Json;
using Microsoft.Extensions.Configuration;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Web.Adaptadores.Idioma;
using HM.Presupuestos.Web.Adaptadores.Navegacion;

namespace HM.Presupuestos.UnitTest.Navegacion;

[TestFixture]
[Category("RecursosApp")]
public class RecursosAppTests
{
    private Mock<IProveedorRecursosJson> _proveedorJsonMock;
    private Mock<IGestorIdioma> _gestorIdiomaMock;
    private Mock<ILocalizadorRecursos> _localizadorRecursosMock;
    private IConfiguration _configuracion;
    private RecursosApp _sut;

    [SetUp]
    public void SetUp()
    {
        _proveedorJsonMock = new Mock<IProveedorRecursosJson>();
        _gestorIdiomaMock = new Mock<IGestorIdioma>();
        _localizadorRecursosMock = new Mock<ILocalizadorRecursos>();

        _configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { { "AppSettings:DefaultLanguage", "es" } })
            .Build();

        _gestorIdiomaMock
            .SetupGet(g => g.IdiomaActual)
            .Returns("es");

        _sut = new RecursosApp(
            _proveedorJsonMock.Object,
            _gestorIdiomaMock.Object,
            _localizadorRecursosMock.Object,
            _configuracion);
    }

    private static JsonDocument CrearDocumentoConAccionesLog(string jsonAccionesLog) =>
        JsonDocument.Parse($$"""{ "AccionesLog": {{jsonAccionesLog}} }""");

    private static JsonDocument CrearDocumentoConMenu(string jsonMenu) =>
        JsonDocument.Parse($$"""{ "Menu": {{jsonMenu}} }""");

    #region ObtenerAccionesLog

    [Test]
    public void ObtenerAccionesLog_IdiomaActivo_DevuelveLabelsEnIdiomaActivo()
    {
        // Arrange
        var doc = CrearDocumentoConAccionesLog("""
            {
                "EntrarEnPresupuestosWebSSO": { "label": "Log into Presupuestos Web (SSO)", "visible": true },
                "EliminarIndicador":          { "label": "Delete indicator", "visible": true },
                "ImpersonacionUsuario":       { "label": "User impersonation", "visible": true }
            }
            """);
        _gestorIdiomaMock.SetupGet(g => g.IdiomaActual).Returns("en");
        _proveedorJsonMock.Setup(p => p.ObtenerDocumento("en")).Returns(doc);

        // Act
        var resultado = _sut.ObtenerAccionesLog();

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(3));
        Assert.That(resultado.Select(x => x.Descripcion),
            Does.Contain("Log into Presupuestos Web (SSO)"));
    }

    [Test]
    public void ObtenerAccionesLog_EntradaConVisibleFalse_NoAparece()
    {
        // Arrange
        var doc = CrearDocumentoConAccionesLog("""
            {
                "EntrarEnPresupuestosWebSSO": { "label": "Entrar (SSO)", "visible": true },
                "CopiarInversionesFinalizado": { "label": "Copia finalizada", "visible": false }
            }
            """);
        _proveedorJsonMock.Setup(p => p.ObtenerDocumento("es")).Returns(doc);

        // Act
        var resultado = _sut.ObtenerAccionesLog();

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(1));
        Assert.That(resultado[0].Descripcion, Is.EqualTo("Entrar (SSO)"));
    }

    [Test]
    public void ObtenerAccionesLog_ResultadoOrdenadoAlfabeticamentePorDescripcion()
    {
        // Arrange
        var doc = CrearDocumentoConAccionesLog("""
            {
                "EliminarIndicador":    { "label": "Zorro", "visible": true },
                "EnviarAviso":          { "label": "Aviso", "visible": true },
                "ActualizacionUsuario": { "label": "Mantenimiento", "visible": true }
            }
            """);
        _proveedorJsonMock.Setup(p => p.ObtenerDocumento("es")).Returns(doc);

        // Act
        var resultado = _sut.ObtenerAccionesLog();

        // Assert
        Assert.That(resultado.Select(x => x.Descripcion),
            Is.EqualTo(new[] { "Aviso", "Mantenimiento", "Zorro" }));
    }

    [Test]
    public void ObtenerAccionesLog_JsonSinSeccionAccionesLog_DevuelveListaVacia()
    {
        // Arrange
        var doc = JsonDocument.Parse("""{ "Menu": {} }""");
        _proveedorJsonMock.Setup(p => p.ObtenerDocumento("es")).Returns(doc);

        // Act
        var resultado = _sut.ObtenerAccionesLog();

        // Assert
        Assert.That(resultado, Is.Empty);
    }

    [Test]
    public void ObtenerAccionesLog_CodigoEnResultado_CorrespondeAlEnumAccionesLog()
    {
        // Arrange
        var doc = CrearDocumentoConAccionesLog("""
            {
                "EntrarEnPresupuestosWebSSO": { "label": "Entrar (SSO)", "visible": true }
            }
            """);
        _proveedorJsonMock.Setup(p => p.ObtenerDocumento("es")).Returns(doc);

        // Act
        var resultado = _sut.ObtenerAccionesLog();

        // Assert
        Assert.That(resultado[0].Codigo, Is.EqualTo((int)AccionesLog.EntrarEnPresupuestosWebSSO));
    }

    #endregion

    #region ObtenerPaginasNavegables

    [Test]
    public void ObtenerPaginasNavegables_JsonConPaginas_DevuelveSoloPaginasConUrl()
    {
        // Arrange — un ítem con URL y otro sin URL
        var doc = CrearDocumentoConMenu("""
            {
                "Menu_1": { "code": 1, "label": "Indicadores", "url": "/indicadores", "visible": true },
                "Menu_2": { "code": 2, "label": "Separador",   "url": "",             "visible": true },
                "Menu_3": { "code": 3, "label": "Auditorías",  "url": "/admin/auditorias", "visible": true }
            }
            """);
        _proveedorJsonMock.Setup(p => p.ObtenerDocumento("es")).Returns(doc);

        // Act
        var resultado = _sut.ObtenerPaginasNavegables();

        // Assert — solo los que tienen URL no vacía
        Assert.That(resultado, Has.Count.EqualTo(2));
        Assert.That(resultado.Select(x => x.Descripcion),
            Does.Contain("Indicadores").And.Contain("Auditorías"));
    }

    #endregion
}
