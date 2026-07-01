using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace HM.Presupuestos.E2ETest.Helpers
{
    public class E2EHostRunner : IDisposable
    {
        private Process? _process;
        public Uri? BaseAddress { get; private set; }

        public async Task StartAsync(int port, string environment, string? signingKey = null)
        {
            var apiDll = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "HM.Presupuestos.Api", "bin", "Debug", "net10.0", "HM.Presupuestos.Api.dll"));
            if (!File.Exists(apiDll))
                throw new FileNotFoundException("No se encontró la DLL de la API. Build the solution first.", apiDll);

            var psi = new ProcessStartInfo("dotnet", apiDll)
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            psi.Environment["ASPNETCORE_URLS"] = $"http://localhost:{port}";
            psi.Environment["DOTNET_ENVIRONMENT"] = environment;
            if (!string.IsNullOrWhiteSpace(signingKey))
            {
                psi.Environment["Jwt__Clave"] = signingKey;
                psi.Environment["Auth__SigningKey"] = signingKey;
            }

            _process = Process.Start(psi);
            if (_process == null)
                throw new InvalidOperationException("No se pudo arrancar el proceso dotnet para la API");

            // opcional: leer salida hasta que arranque
            var tcs = new TaskCompletionSource<bool>();
            _process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null && e.Data.Contains("Now listening on"))
                    tcs.TrySetResult(true);
            };
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();

            await Task.WhenAny(tcs.Task, Task.Delay(5000));

            BaseAddress = new Uri($"http://localhost:{port}");
        }

        public void Dispose()
        {
            try
            {
                if (_process != null && !_process.HasExited)
                {
                    _process.Kill(true);
                    _process.Dispose();
                }
            }
            catch { }
        }
    }
}
