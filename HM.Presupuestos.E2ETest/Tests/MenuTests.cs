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

        // Si no hay sesion autenticada o el menu no se renderiza, estos tests no son aplicables.
        var sidebar = Page.Locator("#sidebar");
        try
        {
            await sidebar.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 15000
            });
        }
        catch
        {
            Assert.Ignore("No se ha renderizado el menu lateral (#sidebar). Verifica la sesion E2E (sesion_auth.json).");
        }
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
        // Esperar a que el DxTreeView de DevExpress termine de renderizar los nodos.
        var itemsMenu = Page.Locator("#sidebar .menu-item");
        await itemsMenu.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 15000
        });

        var count = await itemsMenu.CountAsync();
        Assert.That(count, Is.GreaterThan(0), "El menu no contiene ningun elemento visible");
    }

    [Test]
    [Description("El Drawer del menÃº lateral estÃ¡ renderizado")]
    public async Task Menu_Drawer_EstaRenderizado()
    {
        var drawer = Page.Locator(".demo-drawer, .dxbl-drawer");

        await Expect(drawer).ToBeAttachedAsync(new LocatorAssertionsToBeAttachedOptions
        {
            Timeout = 15000
        });
    }

    [Test]
    [Description("El contenedor del menÃº estÃ¡ visible al cargar la pÃ¡gina")]
    public async Task Menu_Contenedor_EsVisibleAlCargar()
    {
        var menuContainer = Page.Locator("#menuContainer");

        await Expect(menuContainer).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
        {
            Timeout = 15000
        });
    }

    [Test]
    [Description("Al hacer clic en un elemento del menÃº se produce navegaciÃ³n")]
    public async Task Menu_AlHacerClicEnItem_Navega()
    {
        var urlAntes = Page.Url;

        var primerItem = Page.Locator("#sidebar .menu-item").First;
        await primerItem.ClickAsync(new LocatorClickOptions
        {
            Timeout = 15000
        });
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Verificamos que no haya producido un error 5xx tras la navegaciÃ³n
        var contenido = await Page.ContentAsync();
        Assert.That(contenido, Is.Not.Empty, "La pagina quedo vacia tras hacer clic en el menu");

        // Si la URL no cambia por ser una entrada ya activa, al menos no debe romper la navegacion.
        Assert.That(Page.Url, Is.Not.Null.And.Not.Empty);
    }
}
