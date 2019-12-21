using System;
using System.IO;
using System.IO.Compression;
using SimpleNbt.Tags;

namespace SimpleNbt
{
	public static class Utility
	{

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