using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Entidades.LogAcciones;
using HM.Presupuestos.Domain.Puertos;

namespace HM.Presupuestos.UnitTest.LogAcciones;

[TestFixture]
public class LogsTecnicosServiceTests
{
    private HM.Presupuestos.UnitTest.Fakes.InMemoryLogsTecnicosRepository _repository = null!;
    private LogsTecnicosService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new HM.Presupuestos.UnitTest.Fakes.InMemoryLogsTecnicosRepository();
        _sut = new LogsTecnicosService(_repository);
    }

    [Test]
    public async Task ObtenerLogs_ConFiltroPorFechaNivelUsuarioYCategoria_DevuelveCoincidencias()
    {
        // Arrange
        _repository.SembrarLogs(
            new LogTecnicoSeedBuilder()
                .WithFecha(new DateTime(2026, 6, 25, 10, 0, 0))
                .WithNivel("Error")
                .WithUsuario("felix")
                .WithCategoria("RegistrarEvento")
                .Build(),
            new LogTecnicoSeedBuilder()
                .WithFecha(new DateTime(2026, 6, 25, 11, 0, 0))
                .WithNivel("Warn")
                .WithUsuario("felix")
                .WithCategoria("RegistrarEvento")
                .Build(),
            new LogTecnicoSeedBuilder()
                .WithFecha(new DateTime(2026, 6, 26, 11, 0, 0))
                .WithNivel("Error")
                .WithUsuario("ana")
                .WithCategoria("OtroMetodo")
                .Build());

        var filtro = new Builders.FiltroLogsTecnicosBuilder()
            .WithFechaDesde(new DateTime(2026, 6, 25))
            .WithFechaHasta(new DateTime(2026, 6, 25, 23, 59, 59))
            .WithNivel("Error")
            .WithUsuario("felix")
            .WithCategoria("RegistrarEvento")
            .Build();

        // Act
        var resultado = await _sut.ObtenerLogs(filtro);

        // Assert
        Assert.That(resultado, Has.Count.EqualTo(1));
        Assert.That(resultado[0].Categoria, Is.EqualTo("RegistrarEvento"));
        Assert.That(resultado[0].Usuario, Is.EqualTo("felix"));
    }

    [Test]
    public async Task ObtenerLogs_SinResultados_DevuelveListaVacia()
    {
        // Act
        var resultado = await _sut.ObtenerLogs(new FiltroLogsTecnicos { Nivel = "Fatal" });

        // Assert
        Assert.That(resultado, Is.Empty);
    }

    [Test]
    public async Task ObtenerUsuariosDisponibles_CuandoHayUsuariosVacios_AgregaOpcionSinUsuario()
    {
        // Arrange
        _repository.SembrarLogs(
            new LogTecnicoSeedBuilder().WithUsuario("felix").Build(),
            new LogTecnicoSeedBuilder().WithUsuario(string.Empty).Build(),
            new LogTecnicoSeedBuilder().WithUsuario(" ").Build());

        // Act
        var resultado = await _sut.ObtenerUsuariosDisponibles();

        // Assert
        Assert.That(resultado.Select(x => x.Descripcion), Contains.Item("felix"));
        Assert.That(resultado.Select(x => x.Descripcion), Contains.Item(LogsTecnicosConstantes.UsuarioSinUsuario));
    }

    [Test]
    public async Task ObtenerNivelesDisponibles_DevuelveCatalogoDelRepositorio()
    {
        // Arrange
        _repository.SembrarNiveles(
            new Builders.CodigoDescripcionBuilder().WithCodigo(1).WithDescripcion("Trace").Build(),
            new Builders.CodigoDescripcionBuilder().WithCodigo(2).WithDescripcion("Error").Build());

        // Act
        var resultado = await _sut.ObtenerNivelesDisponibles();

        // Assert
        Assert.That(resultado.Select(x => x.Descripcion), Is.EqualTo(new[] { "Trace", "Error" }));
    }
}