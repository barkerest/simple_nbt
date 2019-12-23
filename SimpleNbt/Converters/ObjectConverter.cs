using System;
using System.Collections.Generic;
using SimpleNbt.Tags;

namespace SimpleNbt.Converters
{
	/// <summary>
	/// A converter to/from named binary tags.
	/// </summary>
	public static class ObjectConverter
	{
		/// <summary>
		/// Converts a value to a tag.
		/// </summary>
		/// <param name="name">The name of the new tag.</param>
		/// <param name="value">The value to convert.</param>
		/// <param name="convention">The naming convention for child tags.</param>
		/// <returns>Returns a named binary tag.</returns>
		public static INamedBinaryTag ConvertToTag(string name, object value, NamingConvention convention)
		{
			if (value is null) return new CompoundTag(name);

			return Utility.FindDefaultToConverter(value.GetType()).ConvertToTag(name, value, convention);
		}

		/// <summary>
		/// Converts a value to a compound tag.
		/// </summary>
		/// <param name="name">The name of the new compound tag.</param>
		/// <param name="value">The value to put into the tag.</param>
		/// <param name="convention">The naming convention for child tags.</param>
		/// <returns>Returns a named compound tag.</returns>
		public static CompoundTag ConvertToCompoundTag(string name, object value, NamingConvention convention)
		{
			if (value is null) return new CompoundTag(name);

			var converter = Utility.FindDefaultToConverter(value.GetType());
			if (converter.TagType == typeof(CompoundTag))
			{
				return (CompoundTag) converter.ConvertToTag(name, value, convention);
			}
			
			var ret = new CompoundTag(name);
			ret.Add(new ByteTag(Utility.DataWrapperProperty){Payload = 1});
			ret.Add(converter.ConvertToTag("Value", value, convention));
			return ret;
		}

		/// <summary>
		/// Converts a value to a tag.
		/// </summary>
		/// <param name="name">The name of the new tag.</param>
		/// <param name="value">The value to convert.</param>
		/// <param name="convention">The naming convention for child tags.</param>
		/// <typeparam name="T">The data type being converted.</typeparam>
		/// <returns>Returns a named binary tag.</returns>
		public static INamedBinaryTag ConvertToTag<T>(string name, T value, NamingConvention convention)
		{
			return Utility.FindDefaultToConverter(typeof(T)).ConvertToTag(name, value, convention);
		}

		/// <summary>
		/// Converts a value to a tag.
		/// </summary>
		/// <param name="name">The name of the new tag.</param>
		/// <param name="value">The value to convert.</param>
		/// <param name="valueType"></param>
		/// <param name="convention"></param>
		/// <returns></returns>
		public static INamedBinaryTag ConvertToTag(string name, object value, Type valueType, NamingConvention convention)
		{
			if (valueType is null) throw new ArgumentNullException(nameof(valueType));
			if (value is null && valueType.IsValueType) throw new ArgumentNullException(nameof(value));
			if (value is object && valueType.IsInstanceOfType(value)) throw new InvalidCastException();
			
			return Utility.FindDefaultToConverter(valueType).ConvertToTag(name, value, convention);
		}
		
		/// <summary>
		/// Converts a value to a compound tag.
		/// </summary>
		/// <param name="name">The name of the new tag.</param>
		/// <param name="value">The value to convert.</param>
		/// <param name="convention">The naming convention for child tags.</param>
		/// <typeparam name="T">The data type being converted.</typeparam>
		/// <returns>Returns a named compound tag.</returns>
		public static CompoundTag ConvertToCompoundTag<T>(string name, T value, NamingConvention convention)
		{
			var converter = Utility.FindDefaultToConverter(typeof(T));
			if (converter.TagType == typeof(CompoundTag))
			{
				return (CompoundTag) converter.ConvertToTag(name, value, convention);
			}
			
			var ret = new CompoundTag(name);
			ret.Add(new ByteTag(Utility.DataWrapperProperty){Payload = 1});
			ret.Add(converter.ConvertToTag("Value", value, convention));
			return ret;
		}

		/// <summary>
		/// Converts a value to a compound tag.
		/// </summary>
		/// <param name="name">The name of the new tag.</param>
		/// <param name="value">The value to convert.</param>
		/// <param name="valueType"></param>
		/// <param name="convention"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		/// <exception cref="InvalidCastException"></exception>
		public static CompoundTag ConvertToCompoundTag(string name, object value, Type valueType, NamingConvention convention)
		{
			if (valueType is null) throw new ArgumentNullException(nameof(valueType));
			if (value is null && valueType.IsValueType) throw new ArgumentNullException(nameof(value));
			if (value is object && valueType.IsInstanceOfType(value)) throw new InvalidCastException();
			
			var converter = Utility.FindDefaultToConverter(valueType);
			if (converter.TagType == typeof(CompoundTag))
			{
				return (CompoundTag) converter.ConvertToTag(name, value, convention);
			}
			
			var ret = new CompoundTag(name);
			ret.Add(new ByteTag(Utility.DataWrapperProperty){Payload = 1});
			ret.Add(converter.ConvertToTag("Value", value, convention));
			return ret;
		}
		
		/// <summary>
		/// Converts a named binary tag into a value.
		/// </summary>
		/// <param name="tag">The tag to convert.</param>
		/// <param name="convention">The naming convention for child tags.</param>
		/// <returns>Returns an object.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static object ConvertFromTag(INamedBinaryTag tag, NamingConvention convention)
		{
			if (tag is null) throw new ArgumentNullException(nameof(tag));

			if (tag is CompoundTag compoundTag)
			{
				if (compoundTag.Count == 0) return null;

				if (compoundTag.ContainsKey(Utility.DataWrapperProperty) && compoundTag[Utility.DataWrapperProperty] is ByteTag bt && bt.Payload != 0)
				{
					return ConvertFromTag(compoundTag["Value"], convention);
				}
				
				if (compoundTag.ContainsKey(Utility.DataTypeNameProperty) && compoundTag[Utility.DataTypeNameProperty] is StringTag dataTypeName)
				{
					var type = Type.GetType(dataTypeName.Payload);
					return Utility.FindConverter(typeof(CompoundTag), type).ConvertFromTag(compoundTag, convention);
				}

				var ret = new Dictionary<string, object>();

				foreach (var prop in compoundTag.Values)
				{
					ret[prop.Name] = ConvertFromTag(prop, convention);
				}
				
				return ret;
			}
			
			var converter = Utility.FindDefaultFromConverter(tag.GetType()) ?? throw new InvalidOperationException($"No known decoder for {tag.GetType()} tag.");
			return converter.ConvertFromTag(tag, convention);
		}

		/// <summary>
		/// Converts a named binary tag into a value.
		/// </summary>
		/// <param name="tag">The tag to convert.</param>
		/// <param name="convention">The naming convention for child tags.</param>
		/// <typeparam name="T">The type of value to return.</typeparam>
		/// <returns>Returns the value of the tag.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static T ConvertFromTag<T>(INamedBinaryTag tag, NamingConvention convention)
		{
			if (tag is null) throw new ArgumentNullException(nameof(tag));

			if (tag is CompoundTag compoundTag && compoundTag.ContainsKey(Utility.DataWrapperProperty) && compoundTag[Utility.DataWrapperProperty] is ByteTag bt && bt.Payload != 0)
			{
				return ConvertFromTag<T>(compoundTag["Value"], convention);
			}
			
			return (T) Utility.FindConverter(tag.GetType(), typeof(T)).ConvertFromTag(tag, convention);
		}
		
		
	}
}
