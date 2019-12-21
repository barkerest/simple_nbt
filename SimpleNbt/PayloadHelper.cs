using System;
using System.IO;
using System.Linq;
using System.Text;
using SimpleNbt.Tags;

namespace SimpleNbt
{
	internal static class PayloadHelper
	{
		public static readonly (TagType, TagType, Func<Stream, string, INamedBinaryTag>)[] Builders =
		{
			(TagType.Compound, TagType.End, (s, nm) => new CompoundTag(nm)),

			(TagType.Byte, TagType.End, (s, nm) => new ByteTag(nm)),
			(TagType.Short, TagType.End, (s, nm) => new ShortTag(nm)),
			(TagType.Int, TagType.End, (s, nm) => new IntTag(nm)),
			(TagType.Long, TagType.End, (s, nm) => new LongTag(nm)),
			(TagType.Float, TagType.End, (s, nm) => new SingleTag(nm)),
			(TagType.Double, TagType.End, (s, nm) => new DoubleTag(nm)),
			(TagType.String, TagType.End, (s, nm) => new StringTag(nm)),
			(TagType.ByteArray, TagType.End, (s, nm) => new ByteArrayTag(nm)),
			(TagType.IntArray, TagType.End, (s, nm) => new IntArrayTag(nm)),
			(TagType.LongArray, TagType.End, (s, nm) => new LongArrayTag(nm)),

			(TagType.List, TagType.End, (s, nm) => new ListTag<EndTag>(nm)),
			(TagType.List, TagType.Byte, (s, nm) => new ListTag<ByteTag>(nm)),
			(TagType.List, TagType.Short, (s, nm) => new ListTag<ShortTag>(nm)),
			(TagType.List, TagType.Int, (s, nm) => new ListTag<IntTag>(nm)),
			(TagType.List, TagType.Long, (s, nm) => new ListTag<LongTag>(nm)),
			(TagType.List, TagType.Float, (s, nm) => new ListTag<SingleTag>(nm)),
			(TagType.List, TagType.Double, (s, nm) => new ListTag<DoubleTag>(nm)),
			(TagType.List, TagType.String, (s, nm) => new ListTag<StringTag>(nm)),
			(TagType.List, TagType.ByteArray, (s, nm) => new ListTag<ByteArrayTag>(nm)),
			(TagType.List, TagType.Compound, (s, nm) => new ListTag<CompoundTag>(nm)),
			(TagType.List, TagType.IntArray, (s, nm) => new ListTag<IntArrayTag>(nm)),
			(TagType.List, TagType.LongArray, (s, nm) => new ListTag<LongArrayTag>(nm)),
			
			(TagType.List, TagType.List, (s, nm) => new ListListTag(nm)),
		};


		public static void EncodePayload(this Stream output, sbyte value)
		{
			var b = unchecked((byte) value);
			output.WriteByte(b);
		}

		public static void EncodePayload(this Stream output, short value)
		{
			var bytes = new[]
			{
				(byte) ((value >> 8) & 0xFF),
				(byte) (value & 0xFF)
			};
			output.Write(bytes);
		}

		public static void EncodePayload(this Stream output, int value)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) bytes = bytes.Reverse().ToArray();
			output.Write(bytes);
		}

		public static void EncodePayload(this Stream output, long value)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) bytes = bytes.Reverse().ToArray();
			output.Write(bytes);
		}

		public static void EncodePayload(this Stream output, float value)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) bytes = bytes.Reverse().ToArray();
			output.Write(bytes);
		}

		public static void EncodePayload(this Stream output, double value)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian) bytes = bytes.Reverse().ToArray();
			output.Write(bytes);
		}

		public static void EncodePayload(this Stream output, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				output.EncodePayload((short) 0);
			}
			else
			{
				var valueBytes = Encoding.UTF8.GetBytes(value);
				output.EncodePayload((short) valueBytes.Length);
				output.Write(valueBytes);
			}
		}

		public static sbyte DecodeInt8Payload(this Stream input)
		{
			var bytes = new byte[1];
			if (input.Read(bytes) != 1) throw new InvalidDataException("Expected 1 byte for signed byte value.");
			return unchecked((sbyte) bytes[0]);
		}

		public static short DecodeInt16Payload(this Stream input)
		{
			var bytes = new byte[2];
			if (input.Read(bytes) != 2) throw new InvalidDataException("Expected 2 bytes for short value.");
			return unchecked((short) ((bytes[0] << 8) | bytes[1]));
		}

		public static int DecodeInt32Payload(this Stream input)
		{
			var bytes = new byte[4];
			if (input.Read(bytes) != 4) throw new InvalidDataException("Expected 4 bytes for int value.");
			if (BitConverter.IsLittleEndian) bytes = bytes.Reverse().ToArray();
			return BitConverter.ToInt32(bytes);
		}

		public static long DecodeInt64Payload(this Stream input)
		{
			var bytes = new byte[8];
			if (input.Read(bytes) != 8) throw new InvalidDataException("Expected 8 bytes for long value.");
			if (BitConverter.IsLittleEndian) bytes = bytes.Reverse().ToArray();
			return BitConverter.ToInt64(bytes);
		}

		public static float DecodeSinglePayload(this Stream input)
		{
			var bytes = new byte[4];
			if (input.Read(bytes) != 4) throw new InvalidDataException("Expected 4 bytes for single value.");
			if (BitConverter.IsLittleEndian) bytes = bytes.Reverse().ToArray();
			return BitConverter.ToSingle(bytes);
		}

		public static double DecodeDoublePayload(this Stream input)
		{
			var bytes = new byte[8];
			if (input.Read(bytes) != 8) throw new InvalidDataException("Expected 8 bytes for double value.");
			if (BitConverter.IsLittleEndian) bytes = bytes.Reverse().ToArray();
			return BitConverter.ToDouble(bytes);
		}

		public static string DecodeStringPayload(this Stream input)
		{
			var size = input.DecodeInt16Payload();
			if (size == 0) return "";

			var bytes = new byte[size];
			if (input.Read(bytes) != size) throw new InvalidDataException($"Expected {size} bytes for string value.");

			return Encoding.UTF8.GetString(bytes);
		}

		public static void EncodeTag(this Stream output, INamedBinaryTag tag)
		{
			output.WriteByte((byte) tag.Type);
			output.EncodePayload(tag.Name);
			if (tag is INamedListBinaryTag listChild)
			{
				output.WriteByte((byte) listChild.ListType);
			}

			tag.EncodePayload(output);
		}

		public static INamedBinaryTag DecodeTag(this Stream input)
		{
			var b = input.ReadByte();
			if (b < 0) throw new InvalidDataException("End of file reached before reading end tag.");
			var type = (TagType) b;
			if (type == TagType.End) return new EndTag();

			var name = input.DecodeStringPayload();

			var builders = Builders.Where(x => x.Item1 == type).ToArray();
			if (builders.Length == 0) throw new InvalidDataException($"Failed to locate builder for {type} tag.");
			if (builders.Length == 1)
			{
				var child = builders[0].Item3(input, name);
				child.DecodePayload(input);
				return child;
			}
			else
			{
				b = input.ReadByte();
				if (b < 0) throw new Exception($"End of file reached before determining builder for {type} tag.");
				var subtype = (TagType) b;
				builders = builders.Where(x => x.Item2 == subtype).ToArray();
				if (builders.Length == 0)
					throw new InvalidDataException($"Failed to locate builder for {type}:{subtype} tag.");
				if (builders.Length != 1)
					throw new InvalidOperationException($"Located multiple builders for {type}:{subtype} tag.");
				var child = builders[0].Item3(input, name);
				child.DecodePayload(input);
				return child;
			}
		}
	}
}