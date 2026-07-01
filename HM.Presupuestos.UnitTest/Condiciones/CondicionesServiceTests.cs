using HM.Presupuestos.Application.CasosDeUso;
using HM.Presupuestos.Domain.Entidades;
using HM.Presupuestos.Domain.Puertos;
using HM.Presupuestos.UnitTest.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace HM.Presupuestos.UnitTest.Condiciones;

[TestFixture]
public class CondicionesServiceTests
{
    private InMemoryCondicionesRepository _repo = null!;
    private Mock<HM.Presupuestos.Application.CasosDeUso.Compartido.IMaestrosService> _maestrosMock = null!;
    private Mock<HM.Presupuestos.Application.CasosDeUso.LogAcciones.ILogAccionesService> _logMock = null!;
    private CondicionesService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repo = new HM.Presupuestos.UnitTest.Fakes.InMemoryCondicionesRepository();
        _maestrosMock = new Mock<HM.Presupuestos.Application.CasosDeUso.Compartido.IMaestrosService>();
        _logMock = new Mock<HM.Presupuestos.Application.CasosDeUso.LogAcciones.ILogAccionesService>();

        _sut = new CondicionesService(new NullLogger<CondicionesService>(), _repo, _maestrosMock.Object, _logMock.Object);
    }

    [Test]
    public async Task ValidarSolapesVigencia_NoSolapamiento_ReturnsTrue()
    {
        // Arrange: insertar una vigencia existente de 1..3
        var existing = new VigenciaBuilder().WithCodigo(1).WithVersion(1).WithGrupoCliente(1).WithNetwork(1).WithMesDesde(1).WithMesHasta(3).WithIndicadorAcuerdo(0).Build();
        await _repo.InsertarVigencia(existing);

        var nueva = new VigenciaBuilder().WithCodigo(2).WithVersion(1).WithGrupoCliente(1).WithNetwork(1).WithMesDesde(4).WithMesHasta(5).WithIndicadorAcuerdo(0).Build();

        // Act
        var result = await _sut.ValidarSolapesVigencia(nueva);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ValidarSolapesVigencia_ConSolapamiento_ReturnsFalse()
    {
        // Arrange: existing 1..3
        var existing = new VigenciaBuilder().WithCodigo(1).WithVersion(1).WithGrupoCliente(1).WithNetwork(1).WithMesDesde(1).WithMesHasta(3).WithIndicadorAcuerdo(0).Build();
        await _repo.InsertarVigencia(existing);

        var nueva = new VigenciaBuilder().WithCodigo(2).WithVersion(1).WithGrupoCliente(1).WithNetwork(1).WithMesDesde(3).WithMesHasta(4).WithIndicadorAcuerdo(0).Build();

        // Act
        var result = await _sut.ValidarSolapesVigencia(nueva);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ObtenerCondicionesPorVigencia_NoCondiciones_ReturnsMediosFromMaestros()
    {
        // Arrange: no condiciones en repo
        var medios = new List<HM.Presupuestos.Domain.Entidades.CodigoDescripcion> { new() { Codigo = 10, Descripcion = "TV" } };
        _maestrosMock.Setup(m => m.ObtenerMediosPorNetWork("1")).ReturnsAsync(medios);

        // Act
        var result = await _sut.ObtenerCondicionesPorVigencia(99, 1);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].CodigoMedio, Is.EqualTo(10));
    }
}
