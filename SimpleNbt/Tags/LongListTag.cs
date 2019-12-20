using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A list of 64-bit integers.
	/// </summary>
	public sealed class LongListTag : INamedListBinaryTag
	{
		public LongListTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.List;
		
		/// <inheritdoc />
		public TagType ListType { get; } = TagType.Long;

		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The list of 64-bit integers in this tag.
		/// </summary>
		public List<long> Payload { get; } = new List<long>(); 
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			output.EncodePayload(Payload.Count);
			for (var i = 0; i < Payload.Count; i++)
			{
				output.EncodePayload(Payload[i]);
			}
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			var size = input.DecodeInt32Payload();
			Payload.Clear();
			while (size > 0)
			{
				Payload.Add(input.DecodeInt64Payload());
				size--;
			}
		}
		
		/// <inheritdoc />
		public IEnumerator GetEnumerator() => Payload.GetEnumerator();
	}
}
