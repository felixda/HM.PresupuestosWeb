using System;
using System.Security.Cryptography;
using System.Text;

namespace HM.Presupuestos.E2ETest.Helpers
{
    public static class JwtHelper
    {
        public static string CreateJwt(string signingKey)
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
            var payloadObj = new System.Collections.Generic.Dictionary<string, object>
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

            var payloadJson = System.Text.Json.JsonSerializer.Serialize(payloadObj);
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
