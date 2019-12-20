using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A list of string values.
	/// </summary>
	public sealed class ByteArrayListTag : INamedListBinaryTag
	{
		public ByteArrayListTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.List;
		
		/// <inheritdoc />
		public TagType ListType { get; } = TagType.ByteArray;

		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The list of string values in this tag.
		/// </summary>
		public List<byte[]> Payload { get; } = new List<byte[]>(); 
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			output.EncodePayload(Payload.Count);
			for (var i = 0; i < Payload.Count; i++)
			{
				var size = Payload[i]?.Length ?? 0;
				output.EncodePayload(size);
				if (size > 0)
				{
					output.Write(Payload[i]);
				}
			}
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			var size = input.DecodeInt32Payload();
			Payload.Clear();
			while (size > 0)
			{
				var len = input.DecodeInt32Payload();
				if (len == 0)
				{
					Payload.Add(new byte[0]);
					continue;
				}

				var buf = new byte[len];
				if (input.Read(buf) != len) throw new InvalidDataException($"Expecting {size} bytes of data for byte array payload.");
				Payload.Add(buf);
				
				size--;
			}
		}
		
		/// <inheritdoc />
		public IEnumerator GetEnumerator() => Payload.GetEnumerator();
	}
}
