namespace HM.Presupuestos.E2ETest.Configuracion;

public class E2ETestSettings
{
    public string BaseUrl { get; set; } = "https://localhost:7001";
    public string Usuario { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Headless { get; set; } = true;
    public int SlowMo { get; set; } = 0;
}
