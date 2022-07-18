using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigMerger
{
    public static class PropertyInfoExtensions
    {
        public static void SetValueByType<T>(this PropertyInfo prop, T newT, List<string> values)
        {
            if (newT == null)
                throw new ArgumentNullException(nameof(newT));
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));

            if (prop.PropertyType.IsArray)
            {
                Type? arrayElementType = prop.PropertyType.GetElementType();
                if (arrayElementType != null &&
                    (arrayElementType.IsPrimitive || arrayElementType == typeof(string) || arrayElementType == typeof(DateTime)))
                {

                    Array arr = Array.CreateInstance(arrayElementType, values.Count);
                    for (int i = 0; i < values.Count; i++)
                    {
                        arr.SetValue(TryGetValue(values[i], arrayElementType), i);
                    }
                    prop.SetValue(newT, arr);

                }
            }
            else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listElementType = prop.PropertyType.GetGenericArguments();
                if (listElementType.First().IsPrimitive || listElementType.First() == typeof(string) || listElementType.First() == typeof(DateTime))
                {
                    var listType = typeof(List<>);
                    var listofElements = listType.MakeGenericType(listElementType);
                    var newList = Activator.CreateInstance(listofElements);
                    var list = prop.GetValue(newT);
                    list = newList;
                    var add = prop.PropertyType.GetMethod("Add");
                    foreach (var val in values)
                    {
                        add?.Invoke(list, new[] { TryGetValue(val, listElementType.First()) });
                    }
                    prop.SetValue(newT, list);
                }
            }
            else
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || prop.PropertyType == typeof(DateTime))
                {
                    var value = values.FirstOrDefault();
                    if (value != null)
                        prop.SetValue(newT, TryGetValue(value, prop.PropertyType));
                }
            }
        }
        public static void SetValueByType<T>(this PropertyInfo prop, T newT, string value)
        {
            if (newT == null)
                throw new ArgumentNullException(nameof(newT));
            if (prop == null)
                throw new ArgumentNullException(nameof(prop));

            if (prop.PropertyType.IsArray)
            {
                Type? arrayElementType = prop.PropertyType.GetElementType();
                if (arrayElementType != null &&
                    (arrayElementType.IsPrimitive || arrayElementType == typeof(string) || arrayElementType == typeof(DateTime)))
                {
                    Array arr = Array.CreateInstance(arrayElementType, 1);
                    arr.SetValue(TryGetValue(value, arrayElementType), 0);
                    prop.SetValue(newT, arr);
                }
            }
            else if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listElementType = prop.PropertyType.GetGenericArguments();
                if (listElementType.First().IsPrimitive || listElementType.First() == typeof(string) || listElementType.First() == typeof(DateTime))
                {
                    var listType = typeof(List<>);
                    var listofElements = listType.MakeGenericType(listElementType);
                    var newList = Activator.CreateInstance(listofElements);
                    var list = prop.GetValue(newT);
                    list = newList;
                    var add = prop.PropertyType.GetMethod("Add");
                    add?.Invoke(list, new[] { TryGetValue(value, listElementType.First()) });
                    prop.SetValue(newT, list);
                }
            }
            else
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string) || prop.PropertyType == typeof(DateTime))
                {
                    if (value != null)
                        prop.SetValue(newT, TryGetValue(value, prop.PropertyType));
                }
            }
        }

        private static object GetValue(string value, Type t)
        {
            return Convert.ChangeType(value, t);
        }

        private static object? TryGetValue(string value, Type t)
        {
            try
            {
                return GetValue(value, t);
            }
            catch (System.Exception)
            {
                return Activator.CreateInstance(t);
            }

        }
    }
}
