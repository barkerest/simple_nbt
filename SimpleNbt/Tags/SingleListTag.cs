using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A list of single precision floating point values.
	/// </summary>
	public sealed class SingleListTag : INamedListBinaryTag
	{
		public SingleListTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.List;
		
		/// <inheritdoc />
		public TagType ListType { get; } = TagType.Float;

		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The list of single precision floating point values in this tag.
		/// </summary>
		public List<float> Payload { get; } = new List<float>(); 
		
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
				Payload.Add(input.DecodeSinglePayload());
				size--;
			}
		}
		
		/// <inheritdoc />
		public IEnumerator GetEnumerator() => Payload.GetEnumerator();
	}
}
