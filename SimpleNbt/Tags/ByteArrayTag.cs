using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag holding an array of bytes.
	/// </summary>
	public sealed class ByteArrayTag : INamedBinaryTag<byte[]>
	{
		public ByteArrayTag()
		{
			Name = "";
		}
		
		public ByteArrayTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.ByteArray;
		
		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The array of bytes stored in this tag.
		/// </summary>
		public byte[] Payload { get; set; }
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			var size = Payload?.Length ?? 0;
			output.EncodePayload(size);
			if (size > 0)
			{
				output.Write(Payload);
			}
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			var size = input.DecodeInt32Payload();
			Payload = new byte[size];
			if (size > 0)
			{
				if (input.Read(Payload) != size) throw new InvalidDataException($"Expecting {size} bytes of data for byte array payload.");
			}
		}
	}
}
