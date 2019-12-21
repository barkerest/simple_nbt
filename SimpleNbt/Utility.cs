using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using SimpleNbt.Tags;

namespace SimpleNbt
{
	public static class Utility
	{
		public delegate INamedBinaryTag ConvertToNbt(string name, object value);

		public delegate object ConvertFromNbt(INamedBinaryTag tag);


		#region Default Converters

		private static readonly Dictionary<Type, (ConvertToNbt, ConvertFromNbt)> Converters
			= new Dictionary<Type, (ConvertToNbt, ConvertFromNbt)>()
			{
				{
					typeof(bool), (
						(nm, val) => new ByteTag(nm) {Payload = (sbyte) ((bool) val ? 1 : 0)},
						tag => ((ByteTag) tag).Payload != 0
					)
				},
				{
					typeof(byte), (
						(nm, val) => new ByteTag(nm) {Payload = unchecked((sbyte) (byte) val)},
						tag => unchecked((byte) ((ByteTag) tag).Payload)
					)
				},
				{
					typeof(sbyte), (
						(nm, val) => new ByteTag(nm) {Payload = (sbyte) val},
						tag => ((ByteTag) tag).Payload
					)
				},
				{
					typeof(short), (
						(nm, val) => new ShortTag(nm) {Payload = (short) val},
						tag => ((ShortTag) tag).Payload
					)
				},
				{
					typeof(ushort), (
						(nm, val) => new ShortTag(nm) {Payload = unchecked((short) (ushort) val)},
						tag => unchecked((ushort) ((ShortTag) tag).Payload)
					)
				},
				{
					typeof(int), (
						(nm, val) => new IntTag(nm) {Payload = (int) val},
						tag => ((IntTag) tag).Payload
					)
				},
				{
					typeof(uint), (
						(nm, val) => new IntTag(nm) {Payload = unchecked((int) (uint) val)},
						tag => unchecked((uint) ((IntTag) tag).Payload)
					)
				},
				{
					typeof(long), (
						(nm, val) => new LongTag(nm) {Payload = (long) val},
						tag => ((LongTag) tag).Payload
					)
				},
				{
					typeof(ulong), (
						(nm, val) => new LongTag(nm) {Payload = unchecked((long) (ulong) val)},
						tag => unchecked((ulong) ((LongTag) tag).Payload)
					)
				},
				{
					typeof(float), (
						(nm, val) => new SingleTag(nm) {Payload = (float) val},
						tag => ((SingleTag) tag).Payload
					)
				},
				{
					typeof(double), (
						(nm, val) => new DoubleTag(nm) {Payload = (double) val},
						tag => ((DoubleTag) tag).Payload
					)
				},
				{
					typeof(DateTime), (
						(nm, val) => new LongTag(nm) {Payload = ((DateTime) val).Ticks},
						tag => new DateTime(((LongTag) tag).Payload)
					)
				},
				{
					typeof(Guid), (
						(nm, val) => new ByteArrayTag(nm) {Payload = ((Guid) val).ToByteArray()},
						tag => new Guid(((ByteArrayTag) tag).Payload)
					)
				},
				{
					typeof(decimal), (
						(nm, val) => new IntArrayTag(nm) {Payload = decimal.GetBits((decimal) val)},
						tag => new decimal(((IntArrayTag) tag).Payload)
					)
				},
				{
					typeof(string), (
						(nm, val) => new StringTag(nm) {Payload = (string) val},
						tag => ((StringTag) tag).Payload
					)
				},
				{
					typeof(char), (
						(nm, val) => new ShortTag(nm) {Payload = unchecked((short) (char) val)},
						tag => unchecked((char) ((ShortTag) tag).Payload)
					)
				}
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