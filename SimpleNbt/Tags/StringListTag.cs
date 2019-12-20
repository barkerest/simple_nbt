using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A list of string values.
	/// </summary>
	public sealed class StringListTag : INamedListBinaryTag
	{
		public StringListTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.List;
		
		/// <inheritdoc />
		public TagType ListType { get; } = TagType.String;

		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The list of string values in this tag.
		/// </summary>
		public List<string> Payload { get; } = new List<string>(); 
		
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
				Payload.Add(input.DecodeStringPayload());
				size--;
			}
		}
		
		/// <inheritdoc />
		public IEnumerator GetEnumerator() => Payload.GetEnumerator();
	}
}
