using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Basics.Data.Dapper
{
    public interface IEnumFieldMappings
    {
        FieldDefinition Of(ValueType @enum);
    }

    public abstract class BaseEnumFieldMappings : IEnumFieldMappings
    {
        private EnumToFieldMappings _mappings;

        public FieldDefinition Of(ValueType @enum)
        {
            Type enumType = @enum.GetType();
            if (!enumType.IsEnum)
                throw new ArgumentException($"{@enum} is not a valid enum. It is of type '{enumType}'.");

            if (_mappings == null)
            {
                _mappings = new EnumToFieldMappings();
                IEnumerable<EnumValueToFieldDefinitionMappings> mappings = GetMappings();
                foreach (EnumValueToFieldDefinitionMappings mapping in mappings)
                    _mappings.Add(mapping);
            }

            EnumValueToFieldDefinitionMappings valueMappings = _mappings[enumType];
            return valueMappings[@enum];
        }

        protected abstract IEnumerable<EnumValueToFieldDefinitionMappings> GetMappings();

        protected EnumValueToFieldDefinitionMappings CreateMappings(Type enumType, params FieldMapping[] fieldMappings) =>
            new EnumValueToFieldDefinitionMappings(enumType, fieldMappings);
    }

    //TODO: Have any implementation that reads the mappings from an external source

    public sealed class FieldMapping
    {
        public FieldMapping(ValueType @enum, string columnName, Type type)
        {
            Enum = @enum;
            ColumnName = columnName;
            Type = type;
        }

        public ValueType Enum { get; }
        public string ColumnName { get; }
        public Type Type { get; }
    }

    public sealed class FieldDefinition
    {
        public FieldDefinition(string columnName, Type type)
        {
            ColumnName = columnName;
            Type = type;
        }

        public string ColumnName { get; }

        public Type Type { get; }
    }

    public sealed class EnumValueToFieldDefinitionMappings
    {
        private readonly Dictionary<ValueType, FieldDefinition> _definitions;

        public EnumValueToFieldDefinitionMappings(Type enumType, params FieldMapping[] fieldMappings)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException($"Expected enum type {enumType.FullName} is not an enum.");

            EnumType = enumType;
            Array enumValues = Enum.GetValues(enumType);
            _definitions = new Dictionary<ValueType, FieldDefinition>(enumValues.GetLength(0));
            foreach (ValueType enumValue in enumValues)
                _definitions.Add(enumValue, null);

            if (fieldMappings != null && fieldMappings.Length > 0)
                ApplyFieldMappings(fieldMappings);
        }

        private void ApplyFieldMappings(IReadOnlyList<FieldMapping> fieldMappings)
        {
            Array enumValues = Enum.GetValues(EnumType);
            for (int i = enumValues.GetLowerBound(0); i < enumValues.GetUpperBound(0); i++)
            {
                var enumValue = (ValueType)enumValues.GetValue(0);
                FieldMapping matchingFieldMapping = fieldMappings.FirstOrDefault(fm => fm.Enum.Equals(enumValue));
                if (matchingFieldMapping != null)
                {
                    this[enumValue] = new FieldDefinition(matchingFieldMapping.ColumnName,
                        matchingFieldMapping.Type);
                }
                else
                {
                    string enumName = Enum.GetName(EnumType, enumValue);
                    this[enumValue] = new FieldDefinition(enumName, typeof(string));
                }
            }
        }

        public Type EnumType { get; }

        public FieldDefinition this[ValueType enumValue]
        {
            get
            {
                if (!enumValue.GetType().IsEnum)
                    throw new ArgumentException($"{enumValue} is not a valid enum value.", nameof(enumValue));
                FieldDefinition fieldDefinition;
                if (!_definitions.TryGetValue(enumValue, out fieldDefinition))
                    throw new Exception($"Enum value {enumValue} does not belong to the {EnumType.FullName} enum.");
                return fieldDefinition;
            }
            set
            {
                if (!enumValue.GetType().IsEnum)
                    throw new ArgumentException($"{enumValue} is not a valid enum value.", nameof(enumValue));
                if (!_definitions.ContainsKey(enumValue))
                    throw new Exception($"Enum value {enumValue} does not belong to the {EnumType.FullName} enum.");
                _definitions[enumValue] = value;
            }
        }
    }

    public sealed class EnumToFieldMappings : KeyedCollection<Type, EnumValueToFieldDefinitionMappings>
    {
        protected override Type GetKeyForItem(EnumValueToFieldDefinitionMappings item) => item.EnumType;
    }
}