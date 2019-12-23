using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using SimpleNbt.Attributes;

namespace SimpleNbt.Internal
{
	internal class FieldHelper : IPropOrFieldHelper
	{
		private readonly FieldInfo _field;
		
		public FieldHelper(FieldInfo field, INamedBinaryPropertyAttribute attribute)
		{
			_field = field ?? throw new ArgumentNullException(nameof(field));
			
			Name = _field.Name;

			ExplicitName = attribute?.Name;

			if (attribute != null && attribute.DataType != null)
			{
				if (!field.FieldType.IsAssignableFrom(attribute.DataType)) throw new InvalidCastException($"The explicit datatype in the attribute ({attribute.DataType}) is not assignable to the property type ({field.FieldType}).");
				DataType = attribute.DataType;
			}
			else
			{
				DataType = field.FieldType;
			}
		}
		
		public string Name { get; }
		
		public string ExplicitName { get; }
		public Type DataType { get; }

		public object GetValue(object entity) => _field.GetValue(entity);

		public void SetValue(object entity, object value) => _field.SetValue(entity, value);
	}
}
