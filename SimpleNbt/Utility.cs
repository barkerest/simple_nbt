using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleNbt.Converters;
using SimpleNbt.Tags;

namespace SimpleNbt
{
	public static class Utility
	{
		internal static readonly string DataTypeNameProperty = "--- SimpleNbt:DataTypeName ---";
		internal static readonly string DataWrapperProperty = "--- SimpleNbt:DataWrapper ---";
		internal static readonly string NonStringKeyValue = "--- SimpleNbt:KeyValue {0} ---";
		internal static readonly Regex NonStringKeyValueMatch = new Regex(@"^--- SimpleNbt:KeyValue (\d+) ---$");
		
		#region Default Converters

		private static long[] GuidToLongArray(Guid v)
		{
			var b   = v.ToByteArray();
			var ret = new long[2];
			ret[0] = BitConverter.ToInt64(b, 0);
			ret[1] = BitConverter.ToInt64(b, 8);
			return ret;
		}

		private static Guid GuidFromLongArray(long[] v)
		{
			var b = new byte[16];
			BitConverter.GetBytes(v[0]).CopyTo(b, 0);
			BitConverter.GetBytes(v[1]).CopyTo(b, 8);
			return new Guid(b);
		}

		private static readonly INamedBinaryTagConverter[] DefaultConverters =
		{
			new SimpleTypeConverter<StringTag, string>(nm => new StringTag(nm)),
			new SimpleTypeConverter<ByteTag, sbyte, bool>(nm => new ByteTag(nm), v => (v != 0), v => (sbyte) (v ? 1 : 0)),
			new SimpleTypeConverter<ByteTag, sbyte, byte>(nm => new ByteTag(nm), v => unchecked((byte) v), v => unchecked((sbyte) v)),
			new SimpleTypeConverter<ByteTag, sbyte>(nm => new ByteTag(nm)),
			new SimpleTypeConverter<ShortTag, short, byte>(nm => new ShortTag(nm), v => unchecked((byte) v), v => v),
			new SimpleTypeConverter<IntTag, int, byte>(nm => new IntTag(nm), v => unchecked((byte) v), v => v),
			new SimpleTypeConverter<ShortTag, short>(nm => new ShortTag(nm)),
			new SimpleTypeConverter<ShortTag, short, ushort>(nm => new ShortTag(nm), v => unchecked((ushort) v), v => unchecked((short) v)),
			new SimpleTypeConverter<IntTag, int, ushort>(nm => new IntTag(nm), v => unchecked((ushort) v), v => v),
			new SimpleTypeConverter<ShortTag, short, char>(nm => new ShortTag(nm), v => unchecked((char) v), v => unchecked((short) v)),
			new SimpleTypeConverter<IntTag, int, char>(nm => new IntTag(nm), v => unchecked((char) v), v => v),
			new SimpleTypeConverter<IntTag, int>(nm => new IntTag(nm)),
			new SimpleTypeConverter<IntTag, int, uint>(nm => new IntTag(nm), v => unchecked((uint) v), v => unchecked((int) v)),
			new SimpleTypeConverter<LongTag, long, uint>(nm => new LongTag(nm), v => unchecked((uint) v), v => v),
			new SimpleTypeConverter<LongTag, long>(nm => new LongTag(nm)),
			new SimpleTypeConverter<LongTag, long, ulong>(nm => new LongTag(nm), v => unchecked((ulong) v), v => unchecked((long) v)),
			new SimpleTypeConverter<SingleTag, float>(nm => new SingleTag(nm)),
			new SimpleTypeConverter<SingleTag, float, double>(nm => new SingleTag(nm), v => v, v => (float) v),
			new SimpleTypeConverter<DoubleTag, double>(nm => new DoubleTag(nm)),
			new SimpleTypeConverter<DoubleTag, double, float>(nm => new DoubleTag(nm), v => (float) v, v => v),
			new SimpleTypeConverter<LongTag, long, DateTime>(nm => new LongTag(nm), v => new DateTime(v), v => v.Ticks),
			new SimpleTypeConverter<IntArrayTag, int[], decimal>(nm => new IntArrayTag(nm), v => new decimal(v), v => decimal.GetBits(v)),
			new SimpleTypeConverter<DoubleTag, double, decimal>(nm => new DoubleTag(nm), v => (decimal) v, v => (double) v),
			new SimpleTypeConverter<StringTag, string, Guid>(nm => new StringTag(nm), v => Guid.Parse(v), v => v.ToString()),
			new SimpleTypeConverter<ByteArrayTag, byte[], Guid>(nm => new ByteArrayTag(nm), v => new Guid(v), v => v.ToByteArray()),
			new SimpleTypeConverter<LongArrayTag, long[], Guid>(nm => new LongArrayTag(nm), GuidFromLongArray, GuidToLongArray),
			new SimpleTypeConverter<ByteArrayTag, byte[]>(nm => new ByteArrayTag(nm)),
			new SimpleTypeConverter<IntArrayTag, int[]>(nm => new IntArrayTag(nm)),
			new SimpleTypeConverter<LongArrayTag, long[]>(nm => new LongArrayTag(nm)),

			new SimpleListConverter<ListTag<StringTag>, StringTag, string, string[]>(nm => new ListTag<StringTag>(nm), v => v.ToArray()),
			new SimpleListConverter<ListTag<StringTag>, StringTag, string, List<string>>(nm => new ListTag<StringTag>(nm), v => v.ToList()),
			new SimpleListConverter<ListTag<StringTag>, StringTag, string, IEnumerable<string>>(nm => new ListTag<StringTag>(nm), v => v),

			new SimpleListConverter<ListTag<ByteTag>, ByteTag, sbyte, sbyte[]>(nm => new ListTag<ByteTag>(nm), v => v.ToArray()),
			new SimpleListConverter<ListTag<ByteTag>, ByteTag, sbyte, byte[], byte>(nm => new ListTag<ByteTag>(nm), v => v.ToArray(), v => unchecked((byte) v), v => unchecked((sbyte) v)),
			new SimpleListConverter<ListTag<ByteTag>, ByteTag, sbyte, List<sbyte>>(nm => new ListTag<ByteTag>(nm), v => v.ToList()),
			new SimpleListConverter<ListTag<ByteTag>, ByteTag, sbyte, List<byte>, byte>(nm => new ListTag<ByteTag>(nm), v => v.ToList(), v => unchecked((byte) v), v => unchecked((sbyte) v)),
			new SimpleListConverter<ListTag<ByteTag>, ByteTag, sbyte, IEnumerable<sbyte>>(nm => new ListTag<ByteTag>(nm), v => v),
			new SimpleListConverter<ListTag<ByteTag>, ByteTag, sbyte, IEnumerable<byte>, byte>(nm => new ListTag<ByteTag>(nm), v => v, v => unchecked((byte) v), v => unchecked((sbyte) v)),

			new SimpleListConverter<ListTag<IntTag>, IntTag, int, int[]>(nm => new ListTag<IntTag>(nm), v => v.ToArray()),
			new SimpleListConverter<ListTag<IntTag>, IntTag, int, uint[], uint>(nm => new ListTag<IntTag>(nm), v => v.ToArray(), v => unchecked((uint) v), v => unchecked((int) v)),
			new SimpleListConverter<ListTag<IntTag>, IntTag, int, List<int>>(nm => new ListTag<IntTag>(nm), v => v.ToList()),
			new SimpleListConverter<ListTag<IntTag>, IntTag, int, List<uint>, uint>(nm => new ListTag<IntTag>(nm), v => v.ToList(), v => unchecked((uint) v), v => unchecked((int) v)),
			new SimpleListConverter<ListTag<IntTag>, IntTag, int, IEnumerable<int>>(nm => new ListTag<IntTag>(nm), v => v),
			new SimpleListConverter<ListTag<IntTag>, IntTag, int, IEnumerable<uint>, uint>(nm => new ListTag<IntTag>(nm), v => v, v => unchecked((uint) v), v => unchecked((int) v)),

			new SimpleListConverter<ListTag<LongTag>, LongTag, long, long[]>(nm => new ListTag<LongTag>(nm), v => v.ToArray()),
			new SimpleListConverter<ListTag<LongTag>, LongTag, long, ulong[], ulong>(nm => new ListTag<LongTag>(nm), v => v.ToArray(), v => unchecked((ulong) v), v => unchecked((long) v)),
			new SimpleListConverter<ListTag<LongTag>, LongTag, long, List<long>>(nm => new ListTag<LongTag>(nm), v => v.ToList()),
			new SimpleListConverter<ListTag<LongTag>, LongTag, long, List<ulong>, ulong>(nm => new ListTag<LongTag>(nm), v => v.ToList(), v => unchecked((ulong) v), v => unchecked((long) v)),
			new SimpleListConverter<ListTag<LongTag>, LongTag, long, IEnumerable<long>>(nm => new ListTag<LongTag>(nm), v => v),
			new SimpleListConverter<ListTag<LongTag>, LongTag, long, IEnumerable<ulong>, ulong>(nm => new ListTag<LongTag>(nm), v => v, v => unchecked((ulong) v), v => unchecked((long) v)),

			new SimpleListConverter<ListTag<SingleTag>, SingleTag, float, float[]>(nm => new ListTag<SingleTag>(nm), v => v.ToArray()),
			new SimpleListConverter<ListTag<SingleTag>, SingleTag, float, List<float>>(nm => new ListTag<SingleTag>(nm), v => v.ToList()),
			new SimpleListConverter<ListTag<SingleTag>, SingleTag, float, IEnumerable<float>>(nm => new ListTag<SingleTag>(nm), v => v),

			new SimpleListConverter<ListTag<DoubleTag>, DoubleTag, double, double[]>(nm => new ListTag<DoubleTag>(nm), v => v.ToArray()),
			new SimpleListConverter<ListTag<DoubleTag>, DoubleTag, double, List<double>>(nm => new ListTag<DoubleTag>(nm), v => v.ToList()),
			new SimpleListConverter<ListTag<DoubleTag>, DoubleTag, double, IEnumerable<double>>(nm => new ListTag<DoubleTag>(nm), v => v),

			new SimpleListConverter<ListTag<DoubleTag>, DoubleTag, double, decimal[], decimal>(nm => new ListTag<DoubleTag>(nm), v => v.ToArray(), v => (decimal) v, v => (double) v),
			new SimpleListConverter<ListTag<DoubleTag>, DoubleTag, double, List<decimal>, decimal>(nm => new ListTag<DoubleTag>(nm), v => v.ToList(), v => (decimal) v, v => (double) v),
			new SimpleListConverter<ListTag<DoubleTag>, DoubleTag, double, IEnumerable<decimal>, decimal>(nm => new ListTag<DoubleTag>(nm), v => v, v => (decimal) v, v => (double) v),
			new SimpleListConverter<ListTag<IntArrayTag>, IntArrayTag, int[], decimal[], decimal>(nm => new ListTag<IntArrayTag>(nm), v => v.ToArray(), v => new decimal(v), v => decimal.GetBits(v)),
			new SimpleListConverter<ListTag<IntArrayTag>, IntArrayTag, int[], List<decimal>, decimal>(nm => new ListTag<IntArrayTag>(nm), v => v.ToList(), v => new decimal(v),
			                                                                                          v => decimal.GetBits(v)),
			new SimpleListConverter<ListTag<IntArrayTag>, IntArrayTag, int[], IEnumerable<decimal>, decimal>(nm => new ListTag<IntArrayTag>(nm), v => v, v => new decimal(v), v => decimal.GetBits(v)),

			new SimpleListConverter<ListTag<ByteArrayTag>, ByteArrayTag, byte[], byte[][]>(nm => new ListTag<ByteArrayTag>(nm), v => v.ToArray()),
			new SimpleListConverter<ListTag<ByteArrayTag>, ByteArrayTag, byte[], List<byte[]>>(nm => new ListTag<ByteArrayTag>(nm), v => v.ToList()),
			new SimpleListConverter<ListTag<ByteArrayTag>, ByteArrayTag, byte[], IEnumerable<byte[]>>(nm => new ListTag<ByteArrayTag>(nm), v => v),

			new SimpleListConverter<ListTag<IntArrayTag>, IntArrayTag, int[], int[][]>(nm => new ListTag<IntArrayTag>(nm), v => v.ToArray()),
			new SimpleListConverter<ListTag<IntArrayTag>, IntArrayTag, int[], List<int[]>>(nm => new ListTag<IntArrayTag>(nm), v => v.ToList()),
			new SimpleListConverter<ListTag<IntArrayTag>, IntArrayTag, int[], IEnumerable<int[]>>(nm => new ListTag<IntArrayTag>(nm), v => v),

			new SimpleListConverter<ListTag<LongArrayTag>, LongArrayTag, long[], long[][]>(nm => new ListTag<LongArrayTag>(nm), v => v.ToArray()),
			new SimpleListConverter<ListTag<LongArrayTag>, LongArrayTag, long[], List<long[]>>(nm => new ListTag<LongArrayTag>(nm), v => v.ToList()),
			new SimpleListConverter<ListTag<LongArrayTag>, LongArrayTag, long[], IEnumerable<long[]>>(nm => new ListTag<LongArrayTag>(nm), v => v),

			new SimpleListConverter<ListTag<ByteTag>, ByteTag, sbyte, bool[], bool>(nm => new ListTag<ByteTag>(nm), v => v.ToArray(), v => (v != 0), v => (sbyte) (v ? 1 : 0)),
			new SimpleListConverter<ListTag<ByteTag>, ByteTag, sbyte, List<bool>, bool>(nm => new ListTag<ByteTag>(nm), v => v.ToList(), v => (v != 0), v => (sbyte) (v ? 1 : 0)),
			new SimpleListConverter<ListTag<ByteTag>, ByteTag, sbyte, IEnumerable<bool>, bool>(nm => new ListTag<ByteTag>(nm), v => v, v => (v != 0), v => (sbyte) (v ? 1 : 0)),

			new SimpleListConverter<ListTag<ByteArrayTag>, ByteArrayTag, byte[], Guid[], Guid>(nm => new ListTag<ByteArrayTag>(nm), v => v.ToArray(), v => new Guid(v), v => v.ToByteArray()),
			new SimpleListConverter<ListTag<ByteArrayTag>, ByteArrayTag, byte[], List<Guid>, Guid>(nm => new ListTag<ByteArrayTag>(nm), v => v.ToList(), v => new Guid(v), v => v.ToByteArray()),
			new SimpleListConverter<ListTag<ByteArrayTag>, ByteArrayTag, byte[], IEnumerable<Guid>, Guid>(nm => new ListTag<ByteArrayTag>(nm), v => v, v => new Guid(v), v => v.ToByteArray()),
			new SimpleListConverter<ListTag<LongArrayTag>, LongArrayTag, long[], Guid[], Guid>(nm => new ListTag<LongArrayTag>(nm), v => v.ToArray(), GuidFromLongArray, GuidToLongArray),
			new SimpleListConverter<ListTag<LongArrayTag>, LongArrayTag, long[], List<Guid>, Guid>(nm => new ListTag<LongArrayTag>(nm), v => v.ToList(), GuidFromLongArray, GuidToLongArray),
			new SimpleListConverter<ListTag<LongArrayTag>, LongArrayTag, long[], IEnumerable<Guid>, Guid>(nm => new ListTag<LongArrayTag>(nm), v => v, GuidFromLongArray, GuidToLongArray),
			new SimpleListConverter<ListTag<StringTag>, StringTag, string, Guid[], Guid>(nm => new ListTag<StringTag>(nm), v => v.ToArray(), v => Guid.Parse(v), v => v.ToString()),
			new SimpleListConverter<ListTag<StringTag>, StringTag, string, List<Guid>, Guid>(nm => new ListTag<StringTag>(nm), v => v.ToList(), v => Guid.Parse(v), v => v.ToString()),
			new SimpleListConverter<ListTag<StringTag>, StringTag, string, IEnumerable<Guid>, Guid>(nm => new ListTag<StringTag>(nm), v => v, v => Guid.Parse(v), v => v.ToString()),

			new SimpleListConverter<ListTag<LongTag>, LongTag, long, DateTime[], DateTime>(nm => new ListTag<LongTag>(nm), v => v.ToArray(), v => new DateTime(v), v => v.Ticks),
			new SimpleListConverter<ListTag<LongTag>, LongTag, long, List<DateTime>, DateTime>(nm => new ListTag<LongTag>(nm), v => v.ToList(), v => new DateTime(v), v => v.Ticks),
			new SimpleListConverter<ListTag<LongTag>, LongTag, long, IEnumerable<DateTime>, DateTime>(nm => new ListTag<LongTag>(nm), v => v, v => new DateTime(v), v => v.Ticks),
		};

		#endregion

		/// <summary>
		/// Saves an NBT file using the compound tag for the contents.
		/// </summary>
		/// <param name="filename">The name of the file to save.</param>
		/// <param name="contents">The contents to save.</param>
		/// <param name="compress">If true the file will be saved in GZip format.</param>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="ArgumentNullException"></exception>
		public static void SaveToFile(string filename, CompoundTag contents, bool compress = true)
		{
			if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Filename is required.");
			if (contents is null) throw new ArgumentNullException(nameof(contents));

			using (var fs = File.Create(filename))
			{
				if (compress)
				{
					using (var gz = new GZipStream(fs, CompressionMode.Compress))
					{
						gz.EncodeTag(contents);
					}
				}
				else
				{
					fs.EncodeTag(contents);
				}
			}
		}

		/// <summary>
		/// Loads an NBT file into a CompoundTag.
		/// </summary>
		/// <param name="filename">The file to load.</param>
		/// <param name="compressed">If the file is known to be compressed, or null to autodetect.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		/// <exception cref="FileNotFoundException"></exception>
		/// <exception cref="InvalidDataException"></exception>
		public static CompoundTag LoadFromFile(string filename, bool? compressed = null)
		{
			if (string.IsNullOrWhiteSpace(filename)) throw new ArgumentException("Filename is required.");
			if (!File.Exists(filename)) throw new FileNotFoundException();

			INamedBinaryTag ret;


			using (var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				// autodetect
				if (compressed is null)
				{
					var magic = new byte[2];
					fs.Read(magic);
					fs.Position = 0;
					// check for 0x1f, 0x8b at the beginning of the file.
					compressed = (magic[0] == 0x1f && magic[1] == 0x8b);
				}

				if (compressed.Value)
				{
					using (var gz = new GZipStream(fs, CompressionMode.Decompress))
					{
						ret = gz.DecodeTag();
					}
				}
				else
				{
					ret = fs.DecodeTag();
				}
			}

			return ret as CompoundTag ?? throw new InvalidDataException($"Expected to load compound tag, not {ret}.");
		}

		private static readonly Regex NamingConventionNonAlphaSplit = new Regex(@"[^A-Za-z0-9]");
		private static readonly Regex NamingConventionCamelSplit = new Regex(@"((?:^|[A-Z])[a-z0-9]*)");

		private static IEnumerable<string> TitleCased(this IEnumerable<string> words)
			=> words.Select(x => x.Substring(0, 1).ToUpper() + x.Substring(1).ToLower());
		
		/// <summary>
		/// Formats a name according to the specific naming convention.
		/// </summary>
		/// <param name="convention">The naming convention.</param>
		/// <param name="name">The original name.</param>
		/// <returns>The formatted name.</returns>
		/// <remarks>
		/// The original name must either be in CamelCase or must have non-alphanumeric characters separating words.
		/// The first character can be lowercase for camelCase names.
		/// eg - TheOriginalName42 or the_original_name42
		/// </remarks>
		public static string FormatName(this NamingConvention convention, string name)
		{
			if (string.IsNullOrWhiteSpace(name)) return "";
			var words = NamingConventionNonAlphaSplit
			            .Split(name.Trim())
			            .SelectMany(n => NamingConventionCamelSplit.Matches(n).Select(m => m.Groups[1].Value.ToLower()))
			            .ToArray();

			switch (convention)
			{
				case NamingConvention.LowerCase:
					return string.Join("", words);
				case NamingConvention.UpperCase:
					return string.Join("", words).ToUpper();
				case NamingConvention.LowerCamelCase:
					return words[0] + string.Join("", words.Skip(1).TitleCased());
				case NamingConvention.UpperCamelCase:
					return string.Join("", words.TitleCased());
				case NamingConvention.LowerSnakeCase:
					return string.Join("_", words);
				case NamingConvention.UpperSnakeCase:
					return string.Join("_", words).ToUpper();
				case NamingConvention.TitleSnakeCase:
					return string.Join("_", words.TitleCased());
				case NamingConvention.LowerKebabCase:
					return string.Join("-", words);
				case NamingConvention.UpperKebabCase:
					return string.Join("-", words).ToUpper();
				case NamingConvention.TitleKebabCase:
					return string.Join("-", words.TitleCased());
				case NamingConvention.LowerSpaced:
					return string.Join(" ", words);
				case NamingConvention.UpperSpaced:
					return string.Join(" ", words).ToUpper();
				case NamingConvention.TitleSpaced:
					return string.Join(" ", words.TitleCased());
				default:
					throw new ArgumentOutOfRangeException(nameof(convention), convention, null);
			}
		}
		
		private static readonly  object                                     ConverterLock = new object();
		private static           List<INamedBinaryTagConverter>             _converters;

		private static void InitConverters()
		{
			if (_converters != null) return;

			_converters = new List<INamedBinaryTagConverter>();

			foreach (var conv in DefaultConverters)
			{
				if (!AddConverter(conv)) throw new InvalidOperationException("Failed to register default converters.");
			}
		}

		private static bool AddConverter(INamedBinaryTagConverter converter)
		{
			if (_converters.Any(x => x.TagType == converter.TagType && x.DataType == converter.DataType)) return false;

			_converters.Add(converter);

			return true;
		}

		/// <summary>
		/// Registers a converter.
		/// </summary>
		/// <param name="converter">The converter to register.</param>
		/// <returns>Returns true on success, or false if another converter is already registered for the specific TagType and DataType.</returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static bool RegisterConverter(INamedBinaryTagConverter converter)
		{
			if (converter is null) throw new ArgumentNullException(nameof(converter));
			lock (ConverterLock)
			{
				InitConverters();
				return AddConverter(converter);
			}
		}

		/// <summary>
		/// Removes a registered converter.
		/// </summary>
		/// <param name="converter"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public static void DeregisterConverter(INamedBinaryTagConverter converter)
		{
			if (converter is null) throw new ArgumentNullException(nameof(converter));
			DeregisterConverter(converter.TagType, converter.DataType);
		}

		/// <summary>
		/// Removes a registered converter.
		/// </summary>
		/// <param name="tagType"></param>
		/// <param name="dataType"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public static void DeregisterConverter(Type tagType, Type dataType)
		{
			if (tagType is null) throw new ArgumentNullException(nameof(tagType));
			if (dataType is null) throw new ArgumentNullException(nameof(dataType));
			lock (ConverterLock)
			{
				InitConverters();
				_converters.RemoveAll(x => x.TagType == tagType && x.DataType == dataType);
			}
		}

		/// <summary>
		/// Finds the converter for the specified tag type and data type.
		/// </summary>
		/// <param name="tagType">The named binary tag type.</param>
		/// <param name="dataType">The data to store in the tag.</param>
		/// <param name="createGenericIfMissing"></param>
		/// <returns>Returns the converter on success, null otherwise.</returns>
		public static INamedBinaryTagConverter FindConverter(Type tagType, Type dataType, bool createGenericIfMissing = true)
		{
			if (tagType is null) throw new ArgumentNullException(nameof(tagType));
			if (dataType is null) throw new ArgumentNullException(nameof(dataType));
			lock (ConverterLock)
			{
				InitConverters();
				var ret = _converters.FirstOrDefault(x => x.TagType == tagType && x.DataType == dataType);

				if (ret != null) return ret;

				if (tagType == typeof(CompoundTag))
				{
					if (!createGenericIfMissing) return null;
					
					if (typeof(IDictionary).IsAssignableFrom(dataType))
					{
						return new GenericDictionaryConverter(dataType);
					}
				
					if (typeof(IList).IsAssignableFrom(dataType))
					{
						return new GenericListConverter(dataType);
					}
				
					return new GenericModelConverter(dataType);
				}

				return null;
			}
		}

		/// <summary>
		/// Finds the first registered converter for the specified data type.
		/// </summary>
		/// <param name="dataType"></param>
		/// <param name="createGenericIfMissing"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static INamedBinaryTagConverter FindDefaultToConverter(Type dataType, bool createGenericIfMissing = true)
		{
			if (dataType is null) throw new ArgumentNullException(nameof(dataType));
			lock (ConverterLock)
			{
				InitConverters();
				var ret = _converters.FirstOrDefault(x => x.DataType == dataType);
				
				if (ret != null) return ret;

				if (!createGenericIfMissing) return null;

				if (typeof(IDictionary).IsAssignableFrom(dataType))
				{
					return new GenericDictionaryConverter(dataType);
				}
				
				if (typeof(IList).IsAssignableFrom(dataType))
				{
					return new GenericListConverter(dataType);
				}
				
				return new GenericModelConverter(dataType);
			}
		}

		/// <summary>
		/// Finds the first registered converter for the specified tag type.
		/// </summary>
		/// <param name="tagType"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static INamedBinaryTagConverter FindDefaultFromConverter(Type tagType)
		{
			if (tagType is null) throw new ArgumentNullException(nameof(tagType));
			lock (ConverterLock)
			{
				InitConverters();
				return _converters.FirstOrDefault(x => x.TagType == tagType);
			}
		}
	}
}
