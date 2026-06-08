using HM.Core.Comun.v6.Loggers.Interfaces;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.LogAcciones;

[TestFixture]
public class LogAccionesServiceTests
{
    private Mock<ILogger> _loggerMock;
    private Mock<IJwt> _jwtMock;
    private Mock<ILogAccionesRepository> _repositoryMock;
    private Mock<IRegistroErroresCore> _registroErroresMock;
    private LogAccionesService _sut;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger>();
        _jwtMock = new Mock<IJwt>();
        _repositoryMock = new Mock<ILogAccionesRepository>();
        _registroErroresMock = new Mock<IRegistroErroresCore>();

        _sut = new LogAccionesService(
            _loggerMock.Object,
            _jwtMock.Object,
            _repositoryMock.Object,
            _registroErroresMock.Object);
    }

    #region ObtenerAuditorias

    [Test]
    public async Task ObtenerAuditorias_ConTipoSinFechas_DelegaEnRepositorioConParametrosCorrectos()
    {
        // Arrange
        var tipo = AccionesLog.EntrarEnPresupuestosWebSSO;
        var auditorias = new List<Auditoria>
        {
            new() { Descripcion = "[1] -> Entrar", FechaInicio = new DateTime(2026, 6, 1), Usuario = "Felix Davilla" },
            new() { Descripcion = "[1] -> Entrar", FechaInicio = new DateTime(2026, 6, 2), Usuario = "Sin Usuario especificado" },
        };

        _repositoryMock
            .Setup(r => r.ObtenerAuditorias(tipo, null, null, null))
            .ReturnsAsync(auditorias);

        // Act
        var resultado = await _sut.ObtenerAuditorias(tipo, null, null);

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(2));
        _repositoryMock.Verify(r => r.ObtenerAuditorias(tipo, null, null, null), Times.Once);
    }

    [Test]
    public async Task ObtenerAuditorias_ConTipoYFechas_DelegaEnRepositorioConFechas()
    {
        // Arrange
        var tipo = AccionesLog.EntrarEnPresupuestosWebSSO;
        var fechaInicio = new DateTime(2026, 6, 1);
        var fechaFin = new DateTime(2026, 6, 8);

        _repositoryMock
            .Setup(r => r.ObtenerAuditorias(tipo, fechaInicio, fechaFin, null))
            .ReturnsAsync([]);

        // Act
        var resultado = await _sut.ObtenerAuditorias(tipo, fechaInicio, fechaFin);

        // Assert
        _repositoryMock.Verify(r => r.ObtenerAuditorias(tipo, fechaInicio, fechaFin, null), Times.Once);
    }

    [Test]
    public async Task ObtenerAuditorias_RepositorioDevuelveListaVacia_DevuelveListaVacia()
    {
        // Arrange
        _repositoryMock
            .Setup(r => r.ObtenerAuditorias(It.IsAny<AccionesLog>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int?>()))
            .ReturnsAsync(new List<Auditoria>());

        // Act
        var resultado = await _sut.ObtenerAuditorias(AccionesLog.EntrarEnPresupuestosWebSSO, null, null);

        // Assert
        Assert.That(resultado, Is.Empty);
    }

    [Test]
    public async Task ObtenerAuditorias_RepositorioDevuelveResultados_DevuelveLosMismosResultados()
    {
        // Arrange
        var tipo = AccionesLog.EntrarEnPresupuestosWebSSO;
        var auditorias = new List<Auditoria>
        {
            new() { Descripcion = "[1] -> Entrar", FechaInicio = DateTime.Now, Usuario = "Felix Davilla" },
        };

        _repositoryMock
            .Setup(r => r.ObtenerAuditorias(tipo, null, null, null))
            .ReturnsAsync(auditorias);

        // Act
        var resultado = await _sut.ObtenerAuditorias(tipo, null, null);

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(1));
        Assert.That(resultado[0].Descripcion, Is.EqualTo("[1] -> Entrar"));
        Assert.That(resultado[0].Usuario, Is.EqualTo("Felix Davilla"));
    }

    [Test]
    public async Task ObtenerAuditorias_ConCodigoPagina_DelegaEnRepositorioConCodigoPagina()
    {
        // Arrange
        var tipo = AccionesLog.AccesoAPagina;
        var codigoPagina = 26;
        var auditorias = new List<Auditoria>
        {
            new() { Descripcion = "Acceso a la página: Auditorías", FechaInicio = new DateTime(2026, 6, 8), Usuario = "Felix Davilla" },
        };

        _repositoryMock
            .Setup(r => r.ObtenerAuditorias(tipo, null, null, codigoPagina))
            .ReturnsAsync(auditorias);

        // Act
        var resultado = await _sut.ObtenerAuditorias(tipo, null, null, codigoPagina);

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(1));
        _repositoryMock.Verify(r => r.ObtenerAuditorias(tipo, null, null, codigoPagina), Times.Once);
    }

    [Test]
    public async Task ObtenerAuditorias_SinCodigoPagina_DelegaEnRepositorioConCodigoPaginaNull()
    {
        // Arrange
        var tipo = AccionesLog.AccesoAPagina;

        _repositoryMock
            .Setup(r => r.ObtenerAuditorias(tipo, null, null, null))
            .ReturnsAsync([]);

        // Act
        var resultado = await _sut.ObtenerAuditorias(tipo, null, null);

        // Assert
        _repositoryMock.Verify(r => r.ObtenerAuditorias(tipo, null, null, null), Times.Once);
    }

    #endregion
}
