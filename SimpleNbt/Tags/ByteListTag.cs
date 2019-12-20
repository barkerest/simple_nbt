using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A list of bytes.
	/// </summary>
	public sealed class ByteListTag : INamedListBinaryTag
	{
		public ByteListTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.List;
		
		/// <inheritdoc />
		public TagType ListType { get; } = TagType.Byte;

		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The list of bytes in this tag.
		/// </summary>
		public List<byte> Payload { get; } = new List<byte>(); 
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			output.EncodePayload(Payload.Count);
			output.Write(Payload.ToArray());
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			var size = input.DecodeInt32Payload();
			Payload.Clear();
			if (size == 0) return;
			
			var data = new byte[size];
			if (input.Read(data) != size) throw new InvalidDataException($"Expected to read {size} bytes for byte list.");
			Payload.AddRange(data);
		}

		/// <inheritdoc />
		public IEnumerator GetEnumerator() => Payload.GetEnumerator();
	}
}
