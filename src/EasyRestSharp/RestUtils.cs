namespace EasyRestSharp;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

public static class RestUtils
{
    public static string UrlDecode(this string input)
        => HttpUtility.UrlDecode(input);

    public static string UrlEncode(string input, Encoding? encoding = null)
    {
        if (encoding == null) encoding = Encoding.UTF8;
        var encoded = HttpUtility.UrlEncode(input, encoding);
        return encoded.Replace("+", "%20");
    }

    public static IEnumerable<NameValue> GetNameValues(object obj, params string[] includedProperties)
    {
        // automatically create parameters from object props
        if (obj is IDictionary dict) {
            foreach (var key in dict.Keys) {
                if (key == null) continue;
                var keyStr = key.ToString();
                if (keyStr == null) throw new Exception("Key is not permit to be null");
                yield return new NameValue(keyStr, dict[key]?.ToString());
            }
            yield break;
        }
        else {
            var type = obj.GetType();
            var props = type.GetProperties();

            foreach (var prop in props) {
                if (!IsAllowedProperty(prop.Name, includedProperties)) continue;

                var val = prop.GetValue(obj, null);
                if (val == null) continue;

                var propType = prop.PropertyType;
                if (propType.IsArray) {
                    var elementType = propType.GetElementType();
                    var array = (Array)val;

                    if (array.Length > 0 && elementType != null) {
                        // convert the array to an array of strings
                        var values = array.Cast<object>().Select(item => item.ToString());
                        yield return new NameValue(prop.Name, string.Join(",", values));

                        continue;
                    }
                }
                yield return new NameValue(prop.Name, val.ToString());
            }
        }
    }

    private static bool IsAllowedProperty(string propertyName, params string[] includedProperties)
        => includedProperties.Length == 0 || includedProperties.Length > 0 && includedProperties.Contains(propertyName);

    public struct NameValue
    {
        public string Name { get; set; }
        public string? Value { get; set; }

        public NameValue(string name, string? value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
