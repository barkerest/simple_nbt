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

			DataType = property.PropertyType;

			var t = attribute?.TagType;
			if (t is null)
			{
				Converter = Utility.FindDefaultToConverter(DataType)
					?? throw new InvalidCastException($"Failed to locate converter for {DataType}.");
				TagType = Converter.TagType;
			}
			else
			{
				TagType = t;
				Converter = Utility.FindConverter(TagType, DataType) 
					?? throw new InvalidCastException($"Failed to locate converter from {DataType} to {TagType}.");
			}
		}
		
		public string Name { get; }
		
		public string ExplicitName { get; }
		
		public Type DataType { get; }
		
		public Type TagType { get; }

		public INamedBinaryTagConverter Converter { get; }

		public object GetValue(object entity) => _property.GetValue(entity);

		public void SetValue(object entity, object value) => _property.SetValue(entity, value);
	}
}
