using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(
            this IEnumerable<TSource> source, string fields)
            {
                if(source == null)
                {
                    throw new ArgumentNullException(nameof(source));
                }
                //objetos dinamicos
                var ExpandoObjectList = new List<ExpandoObject>();
                //property info do objeto TSource
                var propertyInfoList = new List<PropertyInfo>();
                if(string.IsNullOrWhiteSpace(fields))
                {
                    //todas as propriedades pulubicas no expando
                    var propertyInfos = typeof(TSource)
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    propertyInfoList.AddRange(propertyInfos);
                }
                else
                {
                    //campos separados ','
                    var fieldsAfterSplit = fields.Split(',');
                    foreach(var field in fieldsAfterSplit)
                    {
                        var propertyName = field.Trim();
                        var propertyInfo = typeof(TSource)
                            .GetProperty(propertyName, BindingFlags.IgnoreCase |
                            BindingFlags.Public | BindingFlags.Instance);
                        
                        if(propertyInfo == null)
                        {
                            throw new Exception($"Property {propertyName} nao foi encontrado" +
                            $"{typeof(TSource)}");
                        }
                        propertyInfoList.Add(propertyInfo);
                    }
                }

                foreach(TSource sourceObject in source)
                {
                    var dataShapedObject = new ExpandoObject();
                    foreach(var propertyInfo in propertyInfoList)
                    {
                        var propertyValue = propertyInfo.GetValue(sourceObject);
                        ((IDictionary<string, object>) dataShapedObject)
                            .Add(propertyInfo.Name, propertyValue);
                    }
                    ExpandoObjectList.Add(dataShapedObject);

                }
                return ExpandoObjectList;
            }
    }
}