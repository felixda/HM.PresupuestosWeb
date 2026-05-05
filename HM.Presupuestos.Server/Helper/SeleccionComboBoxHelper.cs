
using HM.Presupuestos.Domain.Entidades;
using System.Collections;

namespace HM.Presupuestos.Server.Helper
{
    public static class SelecionComboBoxHelper
    {
        public static int? GetPrimerCodigoSeleccionado(object? value)
        {
            if (value == null) return null;

            // Si es directamente IConCodigo
            if (value is IConCodigo ic)
            {
                if (TryParseObjectToInt(ic.Codigo, out int parsed)) return parsed;
            }

            // Si es IDictionary (ej: ListDictionaryInternal)
            //if (value is IDictionary dict)
            //{
            //    foreach (var key in dict.Keys)
            //    {
            //        var item = dict[key];
            //        var c = ExtractCodigoInt(item);
            //        if (c.HasValue) return c;
            //    }
            //}

            // Si es IEnumerable (y no string)
            if (value is IEnumerable enumerable && !(value is string))
            {
                foreach (var element in enumerable)
                {
                    var c = ExtraerCodigo(element);
                    if (c.HasValue) return c;
                }
            }

            // Intentar en el propio objeto
            return ExtraerCodigo(value);
        }

        public static IEnumerable<int> GetCodigosSeleccionados(object? value)
        {
            var result = new List<int>();
            if (value == null) return result;

            if (value is IConCodigo ic && TryParseObjectToInt(ic.Codigo, out int parsedSingle))
            {
                result.Add(parsedSingle);
                return result;
            }
            if (value is IEnumerable enumerable && !(value is string))
            {
                foreach (var element in enumerable)
                {
                    var c = ExtraerCodigo(element);
                    if (c.HasValue) result.Add(c.Value);
                }
                return result;
            }

            var single = ExtraerCodigo(value);
            if (single.HasValue) result.Add(single.Value);
            return result;
        }

        // ----- helpers privados -----
        private static int? ExtraerCodigo(object? item)
        {
            if (item == null) return null;

            // Si implementa IConCodigo (no dar por hecho el tipo de Codigo)
            if (item is IConCodigo ic && TryParseObjectToInt(ic.Codigo, out int parsedFromIcono))
                return parsedFromIcono;

            //// DictionaryEntry (no genérico)
            //if (item is DictionaryEntry de)
            //{
            //    var fromValue = ExtractCodigoInt(de.Value);
            //    if (fromValue.HasValue) return fromValue;
            //    return ExtractCodigoInt(de.Key);
            //}

            //// KeyValuePair<,>
            //var t = item.GetType();
            //if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
            //{
            //    var valProp = t.GetProperty("Value");
            //    if (valProp != null)
            //    {
            //        var val = valProp.GetValue(item);
            //        var fromVal = ExtractCodigoInt(val);
            //        if (fromVal.HasValue) return fromVal;
            //    }

            //    var keyProp = t.GetProperty("Key");
            //    if (keyProp != null)
            //    {
            //        var keyVal = keyProp.GetValue(item);
            //        var fromKey = ExtractCodigoInt(keyVal);
            //        if (fromKey.HasValue) return fromKey;
            //    }
            //}

            // Intentar propiedades comunes por reflexión
            var t = item.GetType();
            string[] propNames = new[] { "Codigo", "Id", "Key", "Value" };
            foreach (var name in propNames)
            {
                var prop = t.GetProperty(name);
                if (prop != null)
                {
                    var valObj = prop.GetValue(item);
                    if (TryParseObjectToInt(valObj, out int parsed)) return parsed;
                }
            }

            // Fallback a ToString()
            if (TryParseObjectToInt(item.ToString(), out int parsedFinal))
                return parsedFinal;

            return null;
        }

        public static IEnumerable<T> GetObjetosSeleccionados<T>(object? value)
        {
            var result = new List<T>();
            if (value == null) return result;

            // Caso: un solo objeto del tipo esperado
            if (value is T single)
            {
                result.Add(single);
                return result;
            }

            // Caso: una colección
            if (value is IEnumerable enumerable && !(value is string))
            {
                foreach (var element in enumerable)
                {
                    if (element is T item)
                        result.Add(item);
                }
                return result;
            }

            return result;
        }


        private static bool TryParseObjectToInt(object? obj, out int result)
        {
            result = 0;
            if (obj == null) return false;

            // Tratar objetos de tipos numéricos y string
            if (obj is int i) { result = i; return true; }
            if (obj is long l && l >= int.MinValue && l <= int.MaxValue) { result = (int)l; return true; }
            if (obj is short s) { result = s; return true; }
            if (obj is byte b) { result = b; return true; }
            if (obj is uint ui && ui <= int.MaxValue) { result = (int)ui; return true; }
            if (obj is ulong || obj is double || obj is float || obj is decimal)
            {
                // Intentar convertir numéricos no enteros sólo si representan un entero exacto y están en rango
                try
                {
                    var dec = Convert.ToDecimal(obj);
                    if (decimal.Truncate(dec) == dec && dec >= int.MinValue && dec <= int.MaxValue)
                    {
                        result = Convert.ToInt32(dec);
                        return true;
                    }
                }
                catch { /* ignore */ }
            }

            if (obj is string sstr && int.TryParse(sstr, out int parsedFromString))
            {
                result = parsedFromString;
                return true;
            }

            // último recurso: intentar parsear ToString()
            var asString = obj.ToString();
            if (!string.IsNullOrEmpty(asString) && int.TryParse(asString, out int parsedFromToString))
            {
                result = parsedFromToString;
                return true;
            }

            return false;
        }
    }

}

