using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using SimpleNbt.Attributes;
using SimpleNbt.Tags;

namespace SimpleNbt.Converters
{
	public class GenericModelConverter : INamedBinaryTagConverter
	{
		private readonly ConstructorInfo _construct;
		
		public GenericModelConverter(Type modelType)
		{
			DataType = modelType ?? throw new ArgumentNullException(nameof(modelType));
			
			if (modelType.IsInterface) throw new ArgumentException("Model type cannot be an interface.");
			if (modelType.IsAbstract) throw new ArgumentException("Model type cannot be abstract.");
			if (modelType.IsPrimitive) throw new ArgumentException("Model type cannot be a primitive.");
			if (modelType == typeof(string)) throw new ArgumentException("Model type cannot be string.");
			if (modelType.IsArray) throw new ArgumentException("Model type cannot be an array.");
			if (typeof(IList).IsAssignableFrom(modelType)) throw new ArgumentException("Model type cannot be a list.");
			if (typeof(IDictionary).IsAssignableFrom(modelType)) throw new ArgumentException("Model type cannot be a dictionary.");
			if (modelType.IsPointer) throw new ArgumentException("Model type cannot be a pointer.");

			_construct = modelType.GetConstructor(Type.EmptyTypes);
			if (_construct is null) throw new ArgumentException("Model type must have a public parameterless constructor.");

			var props = modelType
				.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(x => x.CanRead && x.CanWrite)
				.Select(x => new
				{
					Prop = x, 
					Attr = x.GetCustomAttributes().OfType<INamedBinaryPropertyAttribute>().FirstOrDefault()
				})
				.Where(x => !(x.Attr?.Ignore ?? false))
				.ToArray();
			
			var fields = modelType
				.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Select(x => new
				{
					Field=x, 
					Attr = x.GetCustomAttributes().OfType<INamedBinaryPropertyAttribute>().FirstOrDefault()
				})
				.Where(x => !(x.Attr?.Ignore ?? false))
				.ToArray();

			
			
			
		}
		
		public Type DataType { get; }

		public Type TagType { get; } = typeof(CompoundTag);
		
		public INamedBinaryTag ConvertToTag(string name, object value)
		{
			throw new NotImplementedException();
		}

		public object ConvertFromTag(INamedBinaryTag tag)
		{
			throw new NotImplementedException();
		}
	}
}