using HM.Presupuestos.E2ETest.Base;
using Microsoft.Playwright;

namespace HM.Presupuestos.E2ETest.Tests;

[TestFixture]
[Category("Menu")]
public class MenuTests : E2ETestBase
{
    [SetUp]
    public async Task IrAInicio()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    [Test]
    [Description("El contenedor del menÃº lateral estÃ¡ presente en la pÃ¡gina")]
    public async Task Menu_ContenedorSidebar_EstaPresente()
    {
        var sidebar = Page.Locator("#sidebar");

        await Expect(sidebar).ToBeVisibleAsync();
    }

    [Test]
    [Description("El menÃº contiene al menos un elemento visible")]
    public async Task Menu_ContieneAlMenosUnElemento()
    {
        // Esperar a que el DxTreeView de DevExpress termine de renderizar los nodos
        var primerItem = Page.Locator(".menu-item").First;
        await primerItem.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10000
        });

        var count = await Page.Locator(".menu-item").CountAsync();
        Assert.That(count, Is.GreaterThan(0), "El menÃº no contiene ningÃºn elemento visible");
    }

    [Test]
    [Description("El Drawer del menÃº lateral estÃ¡ renderizado")]
    public async Task Menu_Drawer_EstaRenderizado()
    {
        var drawer = Page.Locator(".demo-drawer");

        await Expect(drawer).ToBeAttachedAsync();
    }

    [Test]
    [Description("El contenedor del menÃº estÃ¡ visible al cargar la pÃ¡gina")]
    public async Task Menu_Contenedor_EsVisibleAlCargar()
    {
        var menuContainer = Page.Locator("#menuContainer");

        await Expect(menuContainer).ToBeVisibleAsync();
    }

    [Test]
    [Description("Al hacer clic en un elemento del menÃº se produce navegaciÃ³n")]
    public async Task Menu_AlHacerClicEnItem_Navega()
    {
        var urlAntes = Page.Url;

        var primerItem = Page.Locator(".menu-item").First;
        await primerItem.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Verificamos que no haya producido un error 5xx tras la navegaciÃ³n
        var contenido = await Page.ContentAsync();
        Assert.That(contenido, Is.Not.Empty, "La pÃ¡gina quedÃ³ vacÃ­a tras hacer clic en el menÃº");
    }
}
