using System;
using System.Reflection;
using SimpleNbt.Attributes;

namespace SimpleNbt.Internal
{
	internal class PropertyHelper : IPropOrFieldHelper
	{
		private readonly PropertyInfo _property;
		
		public PropertyHelper(PropertyInfo property, INamedBinaryPropertyAttribute attribute)
		{
			_property = property ?? throw new ArgumentNullException(nameof(property));
			if (!_property.CanRead) throw new ArgumentException("Property must be readable.");
			if (!_property.CanWrite) throw new ArgumentException("Property must be writable.");
			
			Name = _property.Name;

			ExplicitName = attribute?.Name;

			if (attribute != null && attribute.DataType != null)
			{
				if (!property.PropertyType.IsAssignableFrom(attribute.DataType)) throw new InvalidCastException($"The explicit datatype in the attribute ({attribute.DataType}) is not assignable to the property type ({property.PropertyType}).");
				DataType = attribute.DataType;
			}
			else
			{
				DataType = property.PropertyType;
			}
		}
		
		public string Name { get; }
		
		public string ExplicitName { get; }
		
		public Type DataType { get; }

		public object GetValue(object entity) => _property.GetValue(entity);

		public void SetValue(object entity, object value) => _property.SetValue(entity, value);
	}
}
