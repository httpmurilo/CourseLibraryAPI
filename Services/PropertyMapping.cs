using System;
using System.Collections.Generic;
namespace CourseLibrary.Services {
    public class PropertyMapping<TSource, TDestination> : IPropertyMappings {
        public Dictionary<string, PropertyMappingValue> _mappingDictionary { get; private set; }
        public PropertyMapping (Dictionary<string, PropertyMappingValue> mappingDictionary) {
            _mappingDictionary = mappingDictionary ??
                throw new ArgumentNullException (nameof (mappingDictionary));
        }
    }
}