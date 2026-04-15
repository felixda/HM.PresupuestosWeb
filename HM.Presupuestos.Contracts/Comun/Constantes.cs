
namespace HM.Presupuestos.Contratos.Comun
{
    public static class Constantes
    {
        public const int NUMERO_ESTADOS = 25;

        public static class BitAndVersion
        {
            public const int PILOT_ITRACKER = 512;
            public const int PUBLICADA = 8;
            public const int DESBLOQUEADA = 2;
            public const int CERRADA = 1;
            public const int ABIERTA = 64;
            public const int REAL = 32768;
        }

        /// <summary>
        /// Estos codigos tienen que coincidir en todos los esquemas de la tabla PPT_ESTADOS_VERSIONES
        /// </summary>
        public static class CodigosIndicadores
        {
            public const int CERRADA = 3;
            public const int REAL = 43;
        }


        public static class Environment
        {
            public const string DEV = "DEV";
            public const string PRU = "PRU";
            public const string PRE = "PRE";
            public const string PRO = "PRO";
        }


        public static class Session
        {
            public const string USER = "User";
        }

        public static class UserConfiguration
        {
            public const string MENU_FAVORITES = "MENU_FAVORITOS";
        }


        public static class LocalStorage
        {
            public const string LANGUAGE_CODE = "LanguageCode";
        }

        public static class User
        {
            public const int RULE_ADMIN = 47;
            public const int RULE_AJUSTES = 48;
        }
    }

}
