using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace HM.Presupuestos.Server.Helper
{
    public static class DatosHelper
    {
        public static HashSet<string> ObtenerCamposModificados<T>(T from, T to, IEnumerable<string>? NombresCamposParaComprobarCambios = null) where T : class
        {
            HashSet<string> camposModificados = [];
            ParaCadaCampoDe<T>(propiedadCampo => {
                var sourceValue = propiedadCampo.GetValue(to);
                var valor = propiedadCampo.GetValue(from);
                if (!Equals(valor, sourceValue))
                {
                    camposModificados.Add(propiedadCampo.Name);
                }
            }, NombresCamposParaComprobarCambios);
            return camposModificados;
        }


        public static void AplicarCambios<T>(T from, T to, IEnumerable<string>? NombresCamposDondeAplicarCambios = null) where T : class
        {
            ParaCadaCampoDe<T>(propiedadCampo => {
                var sourceValue = propiedadCampo.GetValue(to);
                var valor = propiedadCampo.GetValue(from);
                if (!Equals(valor, sourceValue))
                {
                    propiedadCampo.SetValue(to, valor);
                   
                }
            }, NombresCamposDondeAplicarCambios);
           
        }


        static void ParaCadaCampoDe<T>(Action<PropertyDescriptor> func, IEnumerable<string>? NombresCampos = null)
        {
            var propiedades = TypeDescriptor.GetProperties(typeof(T));
            NombresCampos ??= propiedades.OfType<PropertyDescriptor>().Select(x => x.Name);

            foreach (string campo in NombresCampos)
            {
                var propiedadCampo = propiedades[campo];
                if (propiedadCampo != null) 
                {
                    func(propiedadCampo);
                }
            }
        }


        /// <summary>
        /// Funcion que serializa al objeto pasado por parámetro y luego lo deserializa para crear una copia profunda del objeto (deep copy)
        /// </summary>
        /// <typeparam name="T">Tipo del objeto a clonar</typeparam>
        /// <param name="objeto">Objeto a clonar</param>
        /// <returns>Objeto del tipo indicado clonado</returns>
        public static T ClonarObjeto<T>(T objeto)
        {
            if (objeto == null) throw new ArgumentNullException(nameof(objeto));

            string serializado = JsonSerializer.Serialize(objeto);
            var resultado = JsonSerializer.Deserialize<T>(serializado);
            return resultado == null ? throw new InvalidOperationException("La deserialización devolvió un valor nulo.") : resultado;
        }

        /// <summary>
        /// Metodo para comparar dos objetos aunque estos tengan a su vez propiedades que sean objetos
        /// </summary>
        /// <param name="obj1"></param>
        /// <param name="obj2"></param>
        /// <returns></returns>
        public static bool SonIguales(object obj1, object obj2)
        {
            var diferencias = ObtenerPropiedadesDiferentes(obj1, obj2);
            return !diferencias.Any();
        }


        public static List<string> ObtenerPropiedadesDiferentes(object? obj1, object? obj2, string path = "")
        {
            var diferencias = new List<string>();

            if (obj1 == null && obj2 == null) return diferencias;
            if (obj1 == null || obj2 == null)
            {
                diferencias.Add($"{path}: {obj1} != {obj2}");
                return diferencias;
            }

            var type = obj1.GetType();

            // Comparación para strings o tipos simples
            if (type.IsPrimitive || type.IsEnum || type == typeof(string) || type.IsValueType)
            {
                if (!obj1.Equals(obj2))
                    diferencias.Add($"{path}: {obj1} != {obj2}");
                return diferencias;
            }

            // Si es una colección
            if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                var list1 = ((IEnumerable)obj1).Cast<object?>().ToList();
                var list2 = ((IEnumerable)obj2).Cast<object?>().ToList();

                if (list1.Count != list2.Count)
                    diferencias.Add($"{path}.Count: {list1.Count} != {list2.Count}");

                int max = Math.Min(list1.Count, list2.Count);
                for (int i = 0; i < max; i++)
                {
                    var subDiffs = ObtenerPropiedadesDiferentes(list1[i], list2[i], $"{path}[{i}]");
                    diferencias.AddRange(subDiffs);
                }

                return diferencias;
            }

            // Comparar propiedades de un objeto complejo
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var valor1 = prop.GetValue(obj1);
                var valor2 = prop.GetValue(obj2);

                string propPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
                var subDiffs = ObtenerPropiedadesDiferentes(valor1, valor2, propPath);
                diferencias.AddRange(subDiffs);
            }

            return diferencias;
        }



    }
}