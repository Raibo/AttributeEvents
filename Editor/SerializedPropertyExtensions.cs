using System;
using UnityEditor;
using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static object OriginalValue(this SerializedProperty property)
        {
            var currentFieldType = property.serializedObject.targetObject.GetType();
            object currentFieldValue = property.serializedObject.targetObject;

            var propertyNames = property.propertyPath.Replace("Array.data[", "Arraydata[").Split('.');

            foreach (var propName in propertyNames)
            {
                if (!propName.StartsWith("Arraydata["))
                {
                    currentFieldValue = currentFieldType.GetField(propName).GetValue(currentFieldValue);
                    currentFieldType = currentFieldType.GetField(propName).FieldType;
                    continue;
                }

                // Handling arrays and lists
                var elementType = GetArraydataElementType(currentFieldType);

                // Unity does not allow serialization of multi-dimensional arrays
                // Unity also supports only int as index
                if (!int.TryParse(propName.Replace("Arraydata[", "").Replace("]", ""), out var index))
                {
                    Debug.LogError($"Was not able to parse index while retrieving value of SerializedProperty with type" +
                        $" {property.serializedObject.targetObject.GetType().Name}");

                    return null;
                }

                currentFieldValue = GetArraydataElementValue(currentFieldValue, currentFieldType, index);
                currentFieldType = elementType;
            }

            return currentFieldValue;
        }


        public static Type OriginalType(this SerializedProperty property)
        {
            var currentFieldType = property.serializedObject.targetObject.GetType();
            var propertyNames = property.propertyPath.Replace("Array.data[", "Arraydata[").Split('.');

            foreach (var propName in propertyNames)
            {
                if (propName.StartsWith("Arraydata["))
                    currentFieldType = GetArraydataElementType(currentFieldType);
                else
                    currentFieldType = currentFieldType.GetField(propName).FieldType;
            }

            return currentFieldType;
        }


        private static Type GetArraydataElementType(Type listOrArrayType) =>
            listOrArrayType.IsArray
                ? listOrArrayType.GetElementType()
                : listOrArrayType.GetGenericArguments()[0];


        private static object GetArraydataElementValue(object listOrArrayValue, Type listOrArrayType, int index)
        {
            if (listOrArrayType.IsArray)
                return ((Array)listOrArrayValue).GetValue(index);

            var indexer = listOrArrayType.GetProperty("Item");
            return indexer.GetValue(listOrArrayValue, new object[] { index });
        }
    }
}
