using HM.Presupuestos.Server.Services;
using Microsoft.JSInterop;

namespace HM.Presupuestos.Server.Utils
{
    public static class ResourceServiceWrapper
    {
        private static IResourceService? _service;

        public static void Initialize(IResourceService service)
        {
            _service = service;
        }

        [JSInvokable]
        public static string GetValue(string elementExpression, string languageCode)
        {
            var result = "";
            if (_service != null)
            {
                result= _service.T(elementExpression, languageCode);
            }
            return result;
        }

        [JSInvokable]
        public static void KeepAlive()
        {
            // Dummy method for alive check
        }
    }
}
