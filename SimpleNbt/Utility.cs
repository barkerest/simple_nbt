using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using SimpleNbt.Converters;
using SimpleNbt.Tags;

namespace SimpleNbt
{
	public static class Utility
	{
		#region Default Converters

        private static readonly INamedBinaryTagConverter[] DefaultConverters =
        {
            new SimpleTypeConverter<StringTag, string>(nm => new StringTag(nm)), 
            new SimpleTypeConverter<ByteTag, sbyte, bool>(nm => new ByteTag(nm), v => (v != 0), v => (sbyte)(v ? 1 : 0)),
            new SimpleTypeConverter<ByteTag, sbyte, byte>(nm => new ByteTag(nm), v => unchecked((byte)v), v => unchecked((sbyte)v)),
            new SimpleTypeConverter<ByteTag, sbyte>(nm => new ByteTag(nm)),
            new SimpleTypeConverter<ShortTag, short, byte>(nm => new ShortTag(nm), v => unchecked((byte)v), v => v), 
            new SimpleTypeConverter<IntTag, int, byte>(nm => new IntTag(nm), v => unchecked((byte)v), v => v), 
            new SimpleTypeConverter<ShortTag, short>(nm => new ShortTag(nm)),
            new SimpleTypeConverter<ShortTag, short, ushort>(nm => new ShortTag(nm), v => unchecked((ushort)v), v => unchecked((short)v)),
            new SimpleTypeConverter<IntTag, int, ushort>(nm => new IntTag(nm), v => unchecked((ushort)v), v => v),
            new SimpleTypeConverter<ShortTag, short, char>(nm => new ShortTag(nm), v => unchecked((char)v), v => unchecked((short)v)),
            new SimpleTypeConverter<IntTag, int, char>(nm => new IntTag(nm), v => unchecked((char)v), v => v), 
            new SimpleTypeConverter<IntTag, int>(nm => new IntTag(nm)),
            new SimpleTypeConverter<IntTag, int, uint>(nm => new IntTag(nm), v => unchecked((uint)v), v => unchecked((int)v)),
            new SimpleTypeConverter<LongTag, long, uint>(nm => new LongTag(nm), v => unchecked((uint)v), v => v), 
            new SimpleTypeConverter<LongTag, long>(nm => new LongTag(nm)),
            new SimpleTypeConverter<LongTag, long, ulong>(nm => new LongTag(nm), v => unchecked((ulong)v), v => unchecked((long)v)),
            new SimpleTypeConverter<SingleTag, float>(nm => new SingleTag(nm)),
            new SimpleTypeConverter<SingleTag, float, double>(nm => new SingleTag(nm), v => v, v => (float)v), 
            new SimpleTypeConverter<DoubleTag, double>(nm => new DoubleTag(nm)),
            new SimpleTypeConverter<DoubleTag, double, float>(nm => new DoubleTag(nm), v => (float)v, v => v), 
            new SimpleTypeConverter<LongTag, long, DateTime>(nm => new LongTag(nm), v => new DateTime(v), v => v.Ticks),
            new SimpleTypeConverter<IntArrayTag, int[], decimal>(nm => new IntArrayTag(nm), v => new decimal(v), v => decimal.GetBits(v)),
            new SimpleTypeConverter<DoubleTag, double, decimal>(nm => new DoubleTag(nm), v => (decimal)v, v => (double)v),
            new SimpleTypeConverter<StringTag, string, Guid>(nm => new StringTag(nm), v => Guid.Parse(v), v => v.ToString()), 
            new SimpleTypeConverter<ByteArrayTag, byte[], Guid>(nm => new ByteArrayTag(nm), v => new Guid(v), v => v.ToByteArray()),
            new SimpleTypeConverter<LongArrayTag, long[], Guid>(nm => new LongArrayTag(nm), 
                v => {
                    var b = new byte[16];
                    BitConverter.GetBytes(v[0]).CopyTo(b, 0);
                    BitConverter.GetBytes(v[1]).CopyTo(b, 8);
                    return new Guid(b);
                },
                v => {
                    var b = v.ToByteArray();
                    var ret = new long[2];
                    ret[0] = BitConverter.ToInt64(b, 0);
                    ret[1] = BitConverter.ToInt64(b, 8);
                    return ret;
                }
            ),
            new SimpleTypeConverter<ByteArrayTag, byte[]>(nm => new ByteArrayTag(nm)),
            new SimpleTypeConverter<IntArrayTag, int[]>(nm => new IntArrayTag(nm)),
            new SimpleTypeConverter<LongArrayTag, long[]>(nm => new LongArrayTag(nm)), 
            
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
	}
}