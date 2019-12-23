using System;
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

			DataType = field.FieldType;

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

		public object GetValue(object entity) => _field.GetValue(entity);

		public void SetValue(object entity, object value) => _field.SetValue(entity, value);
	}
}
