using HM.Core.Comun.v6.Loggers.Interfaces;
using HM.Core.Comun.v6.Seguridad.Interfaces;
using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Presupuestos.Domain.Compartido;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.LogAcciones;

[TestFixture]
public class LogAccionesServiceTests
{
    private Mock<ILogger> _loggerMock = null!;
    private Mock<IJwt> _jwtMock = null!;
    private InMemoryLogAccionesRepository _repository = null!;
    private Mock<IRegistroErroresCore> _registroErroresMock = null!;
    private LogAccionesService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger>();
        _jwtMock = new Mock<IJwt>();
        _repository = new InMemoryLogAccionesRepository();
        _registroErroresMock = new Mock<IRegistroErroresCore>();

        _sut = new LogAccionesService(
            _loggerMock.Object,
            _jwtMock.Object,
            _repository,
            _registroErroresMock.Object);
    }

    #region ObtenerAuditorias

    [Test]
    public async Task ObtenerAuditorias_ConTipoSinFechas_DevuelveSoloDelTipo()
    {
        // Arrange
        _repository.SembrarAuditorias(
            new AuditoriaSeedBuilder()
                .WithTipo(AccionesLog.EntrarEnPresupuestosWebSSO)
                .WithUsuario("Felix Davilla")
                .WithDescripcion("[13] -> Entrar")
                .Build(),
            new AuditoriaSeedBuilder()
                .WithTipo(AccionesLog.EntrarEnPresupuestosWebSSO)
                .WithUsuario("Sin Usuario especificado")
                .WithFechaInicio(new DateTime(2026, 6, 2))
                .WithDescripcion("[13] -> Entrar")
                .Build(),
            new AuditoriaSeedBuilder()
                .WithTipo(AccionesLog.AccesoAPagina)
                .WithDescripcion("Acceso a otra página")
                .Build());

        // Act
        var resultado = await _sut.ObtenerAuditorias(AccionesLog.EntrarEnPresupuestosWebSSO, null, null);

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(2));
        Assert.That(resultado.All(a => a.Descripcion.Contains("Entrar")), Is.True);
    }

    [Test]
    public async Task ObtenerAuditorias_ConTipoYFechas_FiltraPorRango()
    {
        // Arrange
        var tipo = AccionesLog.EntrarEnPresupuestosWebSSO;
        var fechaInicio = new DateTime(2026, 6, 3);
        var fechaFin = new DateTime(2026, 6, 8);

        _repository.SembrarAuditorias(
            new AuditoriaSeedBuilder().WithTipo(tipo).WithFechaInicio(new DateTime(2026, 6, 1)).Build(),
            new AuditoriaSeedBuilder().WithTipo(tipo).WithFechaInicio(new DateTime(2026, 6, 4)).Build(),
            new AuditoriaSeedBuilder().WithTipo(tipo).WithFechaInicio(new DateTime(2026, 6, 8)).Build(),
            new AuditoriaSeedBuilder().WithTipo(tipo).WithFechaInicio(new DateTime(2026, 6, 9)).Build());

        // Act
        var resultado = await _sut.ObtenerAuditorias(tipo, fechaInicio, fechaFin);

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(2));
        Assert.That(resultado.All(a => a.FechaInicio >= fechaInicio && a.FechaInicio <= fechaFin), Is.True);
    }

    [Test]
    public async Task ObtenerAuditorias_SinResultados_DevuelveListaVacia()
    {
        // Act
        var resultado = await _sut.ObtenerAuditorias(AccionesLog.EntrarEnPresupuestosWebSSO, null, null);

        // Assert
        Assert.That(resultado, Is.Empty);
    }

    [Test]
    public async Task ObtenerAuditorias_ConCodigoPagina_FiltraPorCodigoPagina()
    {
        // Arrange
        var tipo = AccionesLog.AccesoAPagina;
        const int codigoPagina = 26;

        _repository.SembrarAuditorias(
            new AuditoriaSeedBuilder()
                .WithTipo(tipo)
                .WithCodigoPagina(codigoPagina)
                .WithDescripcion("Acceso a la página: Auditorías")
                .Build(),
            new AuditoriaSeedBuilder()
                .WithTipo(tipo)
                .WithCodigoPagina(99)
                .WithDescripcion("Acceso a la página: Inicio")
                .Build());

        // Act
        var resultado = await _sut.ObtenerAuditorias(tipo, null, null, codigoPagina);

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(1));
        Assert.That(resultado[0].Descripcion, Is.EqualTo("Acceso a la página: Auditorías"));
    }

    #endregion

    #region ObtenerEstadisticas

    [Test]
    public async Task ObtenerEstadisticas_ConDatos_DevuelveTotalesYAgrupaciones()
    {
        // Arrange
        var tipo = AccionesLog.AccesoAPagina;
        var fechaInicio = new DateTime(2026, 6, 1);
        var fechaFin = new DateTime(2026, 6, 12);

        _repository.SembrarAuditorias(
            new AuditoriaSeedBuilder()
                .WithTipo(tipo)
                .WithFechaInicio(new DateTime(2026, 6, 2))
                .WithUsuario("felix")
                .WithDescripcion("Acceso a la página: Auditorías")
                .Build(),
            new AuditoriaSeedBuilder()
                .WithTipo(tipo)
                .WithFechaInicio(new DateTime(2026, 6, 3))
                .WithUsuario("felix")
                .WithDescripcion("Acceso a la página: Auditorías")
                .Build(),
            new AuditoriaSeedBuilder()
                .WithTipo(tipo)
                .WithFechaInicio(new DateTime(2026, 6, 4))
                .WithUsuario("ana")
                .WithDescripcion("Acceso a la página: Inicio")
                .Build());

        // Act
        var resultado = await _sut.ObtenerEstadisticas(tipo, fechaInicio, fechaFin);

        // Assert
        Assert.That(resultado.TotalAcciones, Is.EqualTo(3));
        Assert.That(resultado.UsuariosUnicos, Is.EqualTo(2));
        Assert.That(resultado.UsuarioMasActivo, Is.EqualTo("felix"));
        Assert.That(resultado.UsuarioMasActivoTotal, Is.EqualTo(2));
        Assert.That(resultado.PaginaMasVisitada, Is.EqualTo("Acceso a la página: Auditorías"));
        Assert.That(resultado.PaginaMasVisitadaTotal, Is.EqualTo(2));
        Assert.That(resultado.TopUsuarios, Has.Count.EqualTo(2));
        Assert.That(resultado.ActividadPorDia, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task ObtenerEstadisticas_SinDatos_DevuelveValoresVacios()
    {
        // Act
        var resultado = await _sut.ObtenerEstadisticas(
            AccionesLog.AccesoAPagina,
            DateTime.Today,
            DateTime.Today);

        // Assert
        Assert.That(resultado.TotalAcciones, Is.EqualTo(0));
        Assert.That(resultado.TopUsuarios, Is.Empty);
        Assert.That(resultado.ActividadPorDia, Is.Empty);
    }

    [Test]
    public async Task ObtenerEstadisticas_ConCodigoPagina_AplicaFiltro()
    {
        // Arrange
        var tipo = AccionesLog.AccesoAPagina;
        var fechaInicio = new DateTime(2026, 6, 1);
        var fechaFin = new DateTime(2026, 6, 12);
        const int codigoPagina = 26;

        _repository.SembrarAuditorias(
            new AuditoriaSeedBuilder()
                .WithTipo(tipo)
                .WithCodigoPagina(codigoPagina)
                .WithDescripcion("Acceso a la página: Auditorías")
                .Build(),
            new AuditoriaSeedBuilder()
                .WithTipo(tipo)
                .WithCodigoPagina(99)
                .WithDescripcion("Acceso a la página: Inicio")
                .Build());

        // Act
        var resultado = await _sut.ObtenerEstadisticas(tipo, fechaInicio, fechaFin, codigoPagina);

        // Assert
        Assert.That(resultado.TotalAcciones, Is.EqualTo(1));
        Assert.That(resultado.PaginaMasVisitada, Is.EqualTo("Acceso a la página: Auditorías"));
    }

    #endregion

    #region Insertar

    [Test]
    public async Task Insertar_ConAccionString_PersisteLogEnRepositorio()
    {
        // Arrange
        const string accion = "Acción personalizada";

        // Act
        await _sut.Insertar(accion);

        // Assert
        Assert.That(_repository.LogsInsertados, Has.Count.EqualTo(1));
        Assert.That(_repository.LogsInsertados[0].Accion, Does.Contain(accion));
    }

    [Test]
    public async Task Insertar_ConEnum_PersisteDescripcionDelEnum()
    {
        // Act
        await _sut.Insertar(AccionesLog.GrabarIndicador);

        // Assert
        Assert.That(_repository.LogsInsertados, Has.Count.EqualTo(1));
        Assert.That(_repository.LogsInsertados[0].Accion, Does.Contain("Grabar Indicador"));
    }

    #endregion
}
