using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleNbt.Attributes;
using SimpleNbt.Internal;
using SimpleNbt.Tags;

namespace SimpleNbt.Converters
{
	internal class GenericModelConverter : INamedBinaryTagConverter
	{
		private readonly ConstructorInfo _construct;
		private readonly IPropOrFieldHelper[] _properties;
		
		public GenericModelConverter(Type modelType)
		{
			DataType = modelType ?? throw new ArgumentNullException(nameof(modelType));
			
			if (modelType.IsInterface) throw new ArgumentException("Model type cannot be an interface.");
			if (modelType.IsAbstract) throw new ArgumentException("Model type cannot be abstract.");
			if (modelType.IsPrimitive) throw new ArgumentException("Model type cannot be a primitive.");
			if (modelType == typeof(string)) throw new ArgumentException("Model type cannot be string.");
			if (typeof(IDictionary).IsAssignableFrom(modelType)) throw new ArgumentException("Model type cannot be a dictionary.");
			if (typeof(IList).IsAssignableFrom(modelType)) throw new ArgumentException("Model type cannot be a list.");
			if (modelType.IsPointer) throw new ArgumentException("Model type cannot be a pointer.");
			
			_construct = modelType.GetConstructor(Type.EmptyTypes);
			if (_construct is null) throw new ArgumentException("Model type must have a public parameterless constructor.");

			if (!Utility.RegisterConverter(this)) throw new InvalidOperationException($"Cannot register generic converter for {modelType}.");

			try
			{
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
					             Field = x,
					             Attr  = x.GetCustomAttributes().OfType<INamedBinaryPropertyAttribute>().FirstOrDefault()
				             })
				             .Where(x => !(x.Attr?.Ignore ?? false))
				             .ToArray();

				_properties = props.Select(p => (IPropOrFieldHelper) new PropertyHelper(p.Prop, p.Attr))
				                   .Union(fields.Select(f => (IPropOrFieldHelper) new FieldHelper(f.Field, f.Attr)))
				                   .ToArray();
			}
			catch
			{
				Utility.DeregisterConverter(TagType, DataType);
				throw;
			}
		}
		
		public Type DataType { get; }

		public Type TagType { get; } = typeof(CompoundTag);
		
		public INamedBinaryTag ConvertToTag(string name, object value, NamingConvention convention)
		{
			if (value is null) return null;
			
			if (!DataType.IsInstanceOfType(value)) throw new InvalidCastException($"Provided value is not an instance of the {DataType} type.");
			
			var ret = new CompoundTag(name);

			ret.Add(new StringTag(Utility.DataTypeNameProperty) {Payload = DataType.FullName});
			
			foreach (var prop in _properties)
			{
				var propValue = prop.GetValue(value);
				var propName = prop.ExplicitName ?? convention.FormatName(prop.Name);
				INamedBinaryTag propTag;
				
				if (prop.DataType != null)	// explicit property type.
				{
					propTag = Utility.FindDefaultToConverter(prop.DataType).ConvertToTag(propName, propValue, convention);
				}
				else
				{
					propTag = ObjectConverter.ConvertToTag(propName, propValue, convention);
				}
				
				if (propTag != null)
				{
					ret.Add(propTag);
				}
			}
			
			return ret;
		}

		public object ConvertFromTag(INamedBinaryTag tag, NamingConvention convention)
		{
			if (tag is null) return null;
			if (!(tag is CompoundTag compound)) throw new InvalidCastException($"Provided tag is not a CompoundTag.");

			var ret = _construct.Invoke(null);

			foreach (var prop in _properties)
			{
				var propName = prop.ExplicitName ?? convention.FormatName(prop.Name);
				if (compound.ContainsKey(propName))
				{
					var itemTag = compound[propName];
					
					object propValue;
					if (prop.DataType != null)
					{
						propValue = Utility.FindConverter(itemTag.GetType(), prop.DataType).ConvertFromTag(itemTag, convention);
					}
					else
					{
						propValue = ObjectConverter.ConvertFromTag(itemTag, convention);
					}
					
					prop.SetValue(ret, propValue);
				}
			}

			return ret;
		}
		
	}
}