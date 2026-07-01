using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using HM.Presupuestos.E2ETest.Helpers;

namespace HM.Presupuestos.E2ETest.Tests
{
    [TestFixture]
    public class MaestrosApiTests
    {
        private int _port;
        private E2EHostRunner? _hostRunner;
        private Uri? _baseAddress;
        private string? _signingKey;

        [OneTimeSetUp]
        public async Task Setup()
        {
            // Puerto libre
            var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            _port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            _signingKey = await GetSigningKeyAsync();

            _hostRunner = new E2EHostRunner();
            await _hostRunner.StartAsync(_port, "Development", _signingKey);
            _baseAddress = _hostRunner.BaseAddress;
            Assert.IsNotNull(_baseAddress);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _hostRunner?.Dispose();
        }

        [Test]
        public async Task ObtenerTipologias_Returns200AndJsonArray()
        {
            using var client = new HttpClient { BaseAddress = new Uri($"http://localhost:{_port}/") };

            if (string.IsNullOrWhiteSpace(_signingKey))
                Assert.Fail("SigningKey no disponible para generar JWT");

            string token = JwtHelper.CreateJwt(_signingKey);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/v1/maestros/tipologias");

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                Assert.Fail($"La petición no fue exitosa. Status: {(int)response.StatusCode} - {response.StatusCode}\nBody:\n{errorBody}");
            }

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(content, Is.Not.Null.And.Not.Empty, "La respuesta está vacía");

            var jsonDoc = JsonDocument.Parse(content);
            Assert.That(jsonDoc.RootElement.ValueKind, Is.EqualTo(JsonValueKind.Array), "La respuesta no es un array JSON");
        }

        private static async Task<string?> GetSigningKeyAsync()
        {
            try
            {
                var apiProjectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "HM.Presupuestos.Api"));
                var psi = new ProcessStartInfo("dotnet", $"user-secrets list --project \"{Path.Combine(apiProjectDir, "HM.Presupuestos.Api.csproj")}\"")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var p = Process.Start(psi);
                if (p != null)
                {
                    var output = await p.StandardOutput.ReadToEndAsync();
                    p.WaitForExit(2000);
                    foreach (var line in output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var kv = line.Split(new[] { '=' }, 2);
                        if (kv.Length != 2) continue;
                        var key = kv[0].Trim();
                        var val = kv[1].Trim();
                        if (string.Equals(key, "Jwt:Clave", StringComparison.OrdinalIgnoreCase) || string.Equals(key, "Auth:SigningKey", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.IsNullOrWhiteSpace(val))
                                return val;
                        }
                    }
                }
            }
            catch { }

            // Fallback a appsettings
            try
            {
                var apiProjectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "HM.Presupuestos.Api"));
                var appsettingsPath = Path.Combine(apiProjectDir, "appsettings.json");
                if (!File.Exists(appsettingsPath))
                    return null;

                using var fs = File.OpenRead(appsettingsPath);
                var doc = await JsonDocument.ParseAsync(fs);
                if (doc.RootElement.TryGetProperty("Auth", out var auth) && auth.TryGetProperty("SigningKey", out var sk))
                    return sk.GetString();
                if (doc.RootElement.TryGetProperty("Jwt", out var jwt) && jwt.TryGetProperty("Clave", out var clave))
                    return clave.GetString();
            }
            catch { }

            return null;
        }

        private static string CreateJwt(string signingKey)
        {
            static string Base64UrlEncodeString(string s)
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(s))
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }

            static string Base64UrlEncodeBytes(byte[] input)
            {
                return Convert.ToBase64String(input)
                    .TrimEnd('=')
                    .Replace('+', '-')
                    .Replace('/', '_');
            }

            var header = Base64UrlEncodeString("{\"alg\":\"HS256\",\"typ\":\"JWT\"}");
            var exp = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds();
            var payloadObj = new Dictionary<string, object>
            {
                ["CodigoUsuario"] = 1,
                ["CodigoAplicacion"] = 1,
                ["CodigoPais"] = 34,
                ["Companias"] = "1",
                ["Login"] = "test",
                ["Nombre"] = "Test",
                ["Apellido1"] = "User",
                ["Jwt"] = "",
                ["exp"] = exp
            };

            var payloadJson = JsonSerializer.Serialize(payloadObj);
            var payload = Base64UrlEncodeString(payloadJson);

            var unsigned = header + "." + payload;
            var keyBytes = Encoding.UTF8.GetBytes(signingKey);
            using var sha = new HMACSHA256(keyBytes);
            var sig = sha.ComputeHash(Encoding.UTF8.GetBytes(unsigned));
            var signature = Base64UrlEncodeBytes(sig);
            return unsigned + "." + signature;
        }
    }
}
