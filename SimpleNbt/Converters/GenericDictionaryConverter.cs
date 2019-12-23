using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using SimpleNbt.Internal;
using SimpleNbt.Tags;

namespace SimpleNbt.Converters
{
	internal class GenericDictionaryConverter : INamedBinaryTagConverter
	{
		private readonly ConstructorInfo _construct;
		private readonly bool _hasStringKey;
		

		public GenericDictionaryConverter(Type modelType)
		{
			DataType = modelType ?? throw new ArgumentNullException(nameof(modelType));
			
			if (modelType.IsInterface) throw new ArgumentException("Model type cannot be an interface.");
			if (modelType.IsAbstract) throw new ArgumentException("Model type cannot be abstract.");
			if (modelType.IsPrimitive) throw new ArgumentException("Model type cannot be a primitive.");
			if (modelType == typeof(string)) throw new ArgumentException("Model type cannot be string.");
			if (!typeof(IDictionary).IsAssignableFrom(modelType)) throw new ArgumentException("Model type must be a dictionary.");
			
			_construct = modelType.GetConstructor(Type.EmptyTypes);
			if (_construct is null) throw new ArgumentException("Model type must have a public parameterless constructor.");

			if (modelType.ImplementsGeneric(typeof(IDictionary<,>)))
			{
				_hasStringKey = modelType.GetGenericImplementationType(typeof(IDictionary<,>), 0) == typeof(string);
			}
			else
			{
				// unknown key type
				_hasStringKey = false;
			}
			
			if (!Utility.RegisterConverter(this)) throw new InvalidOperationException($"Cannot register generic converter for {modelType}.");
		}
		
		public Type DataType { get; }

		public Type TagType { get; } = typeof(CompoundTag);

		private INamedBinaryTag WithStringKeys(string name, IDictionary value, NamingConvention convention)
		{
			var ret = new CompoundTag(name);
			ret.Add(new StringTag(Utility.DataTypeNameProperty) {Payload = DataType.FullName});
			
			foreach (var key in value.Keys)
			{
				ret.Add(ObjectConverter.ConvertToTag(key.ToString(), value[key], convention));
			}

			return ret;
		}

		private INamedBinaryTag WithoutStringKeys(string name, IDictionary value, NamingConvention convention)
		{
			var ret = new CompoundTag(name);
			ret.Add(new StringTag(Utility.DataTypeNameProperty) {Payload = DataType.FullName});

			var i = 0;
			foreach (var key in value.Keys)
			{
				var propName = string.Format(Utility.NonStringKeyValue, i);
				var prop = new CompoundTag(propName);
				prop.Add(ObjectConverter.ConvertToTag("Key", key, convention));
				prop.Add(ObjectConverter.ConvertToTag("Value", value[key], convention));
				ret.Add(prop);
				i++;
			}

			return ret;
		}
		
		public INamedBinaryTag ConvertToTag(string name, object value, NamingConvention convention)
		{
			if (value is null) return new CompoundTag();
			if (!(value is IDictionary dict)) throw new InvalidCastException();

			return _hasStringKey ? WithStringKeys(name, dict, convention) : WithoutStringKeys(name, dict, convention);
		}

		private IDictionary WithStringKeys(CompoundTag tag, NamingConvention convention)
		{
			IDictionary ret = (IDictionary) _construct.Invoke(null);

			foreach (var key in tag.Keys)
			{
				if (key != Utility.DataTypeNameProperty)
				{
					var value = ObjectConverter.ConvertFromTag(tag[key], convention);
					ret[key] = value;
				}
			}
			
			return ret;
		}

		private IDictionary WithoutStringKeys(CompoundTag tag, NamingConvention convention)
		{
			IDictionary ret = (IDictionary) _construct.Invoke(null);

			foreach (var key in tag.Keys)
			{
				if (Utility.NonStringKeyValueMatch.IsMatch(key))
				{
					var kv = tag[key] as CompoundTag ?? throw new InvalidCastException("Child entries should be compound tags.");

					var k = ObjectConverter.ConvertFromTag(kv["Key"], convention);
					var v = ObjectConverter.ConvertFromTag(kv["Value"], convention);

					ret[k] = v;
				}
			}
			
			return ret;
		}
		
		public object ConvertFromTag(INamedBinaryTag tag, NamingConvention convention)
		{
			if (tag is null) return null;
			if (!(tag is CompoundTag compoundTag)) throw new InvalidCastException("Tag is not a compound tag.");

			return _hasStringKey ? WithStringKeys(compoundTag, convention) : WithoutStringKeys(compoundTag, convention);
		}
		
	}
}
