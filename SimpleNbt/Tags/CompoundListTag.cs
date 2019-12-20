using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A list of compound values.
	/// </summary>
	public sealed class CompoundListTag : INamedListBinaryTag
	{
		public CompoundListTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.List;
		
		/// <inheritdoc />
		public TagType ListType { get; } = TagType.Compound;

		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The list of compound values in this tag.
		/// </summary>
		public List<CompoundTag> Payload { get; } = new List<CompoundTag>(); 
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			output.EncodePayload(Payload.Count);
			for (var i = 0; i < Payload.Count; i++)
			{
				Payload[i].EncodePayload(output);
			}
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			var size = input.DecodeInt32Payload();
			Payload.Clear();
			for (var i = 1; i <= size; i++)
			{
				var child = new CompoundTag($"Item {i}");
				child.DecodePayload(input);
				Payload.Add(child);
			}
		}
		
		/// <inheritdoc />
		public IEnumerator GetEnumerator() => Payload.GetEnumerator();
	}
}
