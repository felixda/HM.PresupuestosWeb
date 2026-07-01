using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Application.CasosDeUso.LogAcciones;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.UnitTest.Fakes;
using Microsoft.Extensions.Logging;

namespace HM.Presupuestos.UnitTest.Sobreprimas;

[TestFixture]
public class SobreprimasServiceTests
{
    private Mock<ILogger<SobreprimasService>> _loggerMock = null!;
    private InMemorySobreprimasRepository _repo = null!;
    private Mock<ILogAccionesService> _logAccionesMock = null!;
    private SobreprimasService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<SobreprimasService>>();
        _repo = new InMemorySobreprimasRepository();
        _logAccionesMock = new Mock<ILogAccionesService>();

        _sut = new SobreprimasService(
            _loggerMock.Object,
            _repo,
            _logAccionesMock.Object);
    }

    [Test]
    public async Task ObtenerSobreprimas_DevuelveListaDesdeRepositorio()
    {
        // Arrange
        var filtro = new SobreprimaFiltro();
        // seed data
            _repo.InsertSobreprima(new Builders.SobreprimaBuilder().WithCodigo(0).WithPorcentaje(5m).Build()).Wait();

        // Act
        var resultado = await _sut.ObtenerSobreprimas(filtro);

        // Assert
        Assert.That(resultado.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task ExistenSobreprimas_SinSobreprima_DelegatesToRepository()
    {
        var filtro = new SobreprimaFiltro();
        // seed data so repository reports existence
            _repo.InsertSobreprima(new Builders.SobreprimaBuilder().WithCodigo(0).WithPorcentaje(1m).Build()).Wait();

        var resultado = await _sut.ExistenSobreprimas(filtro, null);

        Assert.That(resultado, Is.True);
    }

    [Test]
    public async Task GrabarSobreprimas_InsertaActualizaYHaceCommit()
    {
        // Arrange
        var items = new List<Sobreprima>
        {
              new Builders.SobreprimaBuilder().WithCodigo(0).WithPorcentaje(1m).Build(), // insertar
              new Builders.SobreprimaBuilder().WithCodigo(2).WithPorcentaje(0m).Build(), // eliminar
              new Builders.SobreprimaBuilder().WithCodigo(3).WithPorcentaje(2m).Build()  // actualizar
        };

        // seed existing items so eliminar/actualizar find targets
           _repo.InsertSobreprima(new Builders.SobreprimaBuilder().WithCodigo(2).WithPorcentaje(9m).Build()).Wait();
           _repo.InsertSobreprima(new Builders.SobreprimaBuilder().WithCodigo(3).WithPorcentaje(8m).Build()).Wait();

        var trans = _repo.Transaccion;

        // Act
        await _sut.GrabarSobreprimas(items);

        // Assert
        // verify state
        var all = _repo.All();
        Assert.That(all.Any(s => s.Porcentaje == 1m)); // inserted
        Assert.That(all.All(s => s.Codigo != 2)); // deleted
        Assert.That(all.Any(s => s.Codigo == 3 && s.Porcentaje == 2m)); // updated
        Assert.That(trans.CommitInvocado, Is.True);
    }
}
