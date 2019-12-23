using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using SimpleNbt.Tags;

namespace SimpleNbt.Converters
{
	internal class GenericListConverter : INamedBinaryTagConverter
	{
		private readonly ConstructorInfo _construct;
		private readonly INamedBinaryTagConverter _elementConverter;
		private readonly ConstructorInfo _tagConstruct;

		public GenericListConverter(Type modelType)
		{
			DataType = modelType ?? throw new ArgumentNullException(nameof(modelType));
			
			if (modelType.IsInterface) throw new ArgumentException("Model type cannot be an interface.");
			if (modelType.IsAbstract) throw new ArgumentException("Model type cannot be abstract.");
			if (modelType.IsPrimitive) throw new ArgumentException("Model type cannot be a primitive.");
			if (modelType == typeof(string)) throw new ArgumentException("Model type cannot be string.");
			if (typeof(IDictionary).IsAssignableFrom(modelType)) throw new ArgumentException("Model type cannot be a dictionary.");
			if (!typeof(IList).IsAssignableFrom(modelType)) throw new ArgumentException("Model type must be an array or list.");
			
			_construct = modelType.GetConstructor(new []{typeof(int)});
			if (_construct is null) throw new ArgumentException("Model type must have a public constructor taking an integer size as the sole argument.");
			
			// Determine the type of elements we will be constructing in this array.
			if (modelType.IsArray)
			{
				ElementDataType = modelType.GetElementType();
			}
			else
			{
				var gt = modelType.GetGenericArguments();
				if (gt.Length != 1)
				{
					throw new ArgumentException("The model type has more than one generic type argument.");
				}

				ElementDataType = gt[0];
			}

			_elementConverter = Utility.FindDefaultToConverter(ElementDataType, false);

			if (_elementConverter is null)
			{
				TagType = typeof(ListTag<CompoundTag>);
			}
			else if (typeof(INamedListBinaryTag).IsAssignableFrom(_elementConverter.TagType))
			{
				TagType = typeof(ListListTag);
			}
			else
			{
				TagType = typeof(ListTag<>).MakeGenericType(_elementConverter.TagType);
			}

			_tagConstruct = TagType.GetConstructor(new[] {typeof(string)})
				?? throw new ArgumentException("Failed to locate named binary tag constructor for list tag.");
			
			if (!Utility.RegisterConverter(this)) throw new InvalidOperationException($"Cannot register generic converter for {modelType}.");
		}
		
		public Type DataType { get; }
		
		public Type ElementDataType { get; }
		
		public Type TagType { get; }
		
		public INamedBinaryTag ConvertToTag(string name, object value, NamingConvention convention)
		{
			var tag = (INamedListBinaryTag) _tagConstruct.Invoke(new object[] {name});
			var source = value as IList;
			
			tag.Clear();
			if (source != null)
			{
				foreach (var item in source)
				{
					if (_elementConverter is null)
					{
						tag.Add(ObjectConverter.ConvertToCompoundTag("", item, convention));
					}
					else
					{
						tag.Add(_elementConverter.ConvertToTag("", item, convention));
					}
				}
			}

			return tag;
		}

		public object ConvertFromTag(INamedBinaryTag tag, NamingConvention convention)
		{
			if (tag is null) return null;
			if (!(tag is INamedListBinaryTag list)) throw new InvalidCastException("Tag is not a list tag.");
			var ret = (IList) _construct.Invoke(new object[] {list.Count});

			for (var i = 0; i < list.Count; i++)
			{
				if (_elementConverter is null)
				{
					ret[i] = ObjectConverter.ConvertFromTag((INamedBinaryTag) list[i], convention);
				}
				else
				{
					ret[i] = _elementConverter.ConvertFromTag((INamedBinaryTag)list[i], convention);
				}
			}

			return ret;
		}
		
	}
}
