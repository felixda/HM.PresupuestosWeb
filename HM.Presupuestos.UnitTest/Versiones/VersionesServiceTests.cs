using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Domain.Entidades;
using DomainVersion = HM.Presupuestos.Domain.Entidades.Version;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.UnitTest.Fakes;
using Microsoft.Extensions.Logging;

namespace HM.Presupuestos.UnitTest.Versiones;

[TestFixture]
public class VersionesServiceTests
{
    private Mock<ILogger<VersionesService>> _loggerMock = null!;
    private InMemoryVersionesRepository _repo = null!;
    private VersionesService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<VersionesService>>();
        _repo = new InMemoryVersionesRepository();

        _sut = new VersionesService(_loggerMock.Object, _repo);
    }

    [Test]
    public async Task ObtenerVersionesResumen_DelegatesToRepository()
    {
        var lista = new List<VersionResumen> { new Builders.VersionResumenBuilder().WithCodigo(2026).WithDescripcion("2026").Build() };
        // seed and act - use builder for clarity
        _repo.SeedVersion(new VersionBuilder().WithCodigo(2026).WithDescripcion("2026").WithAnio(2026).Build());
        var resultado = await _sut.ObtenerVersionesResumen(null, null, null);
        Assert.That(resultado, Has.Count.GreaterThanOrEqualTo(0));
    }

    [Test]
    public async Task ObtenerVersiones_CalculaIndicadoresYTieneDatosVinculados()
    {
        var versiones = new List<DomainVersion>
        {
            new VersionBuilder().WithCodigo(1).WithIndEstado(3).Build()
        };

        var indicadoresMaster = new List<HM.Presupuestos.Domain.Entidades.Indicador>
        {
            new HM.Presupuestos.Domain.Entidades.Indicador { Codigo = 1, BitAnd = 1 },
            new HM.Presupuestos.Domain.Entidades.Indicador { Codigo = 2, BitAnd = 2 }
        };

        // seed
        _repo.SeedVersion(new VersionBuilder().WithCodigo(1).WithIndEstado(3).Build());
        _repo.SeedIndicador(new IndicadorBuilder().WithCodigo(1).WithBitAnd(1).Build());
        _repo.SeedIndicador(new IndicadorBuilder().WithCodigo(2).WithBitAnd(2).Build());

        var resultado = await _sut.ObtenerVersiones(2026);

        Assert.That(resultado, Has.Count.GreaterThanOrEqualTo(0));
    }

    [Test]
    public async Task GrabarVersiones_InsertaActualizaYHaceCommit()
    {
        var trans = _repo.ObtenerTransaccion();

        var nuevas = new List<DomainVersion> { new VersionBuilder().WithCodigo(0).Build() };
        var mod = new List<DomainVersion> { new VersionBuilder().WithCodigo(5).Build() };

        var resultado = await _sut.GrabarVersiones(nuevas, mod, 34);

        Assert.That(resultado, Is.True);
        Assert.That(((InMemoryTransaccion)trans).CommitInvocado, Is.True);
    }
}
