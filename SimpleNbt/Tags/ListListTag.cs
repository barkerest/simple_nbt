using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A list of lists.
	/// </summary>
	public sealed class ListListTag : INamedListBinaryTag
	{
		public ListListTag(string name)
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
		/// The list of lists in this tag.
		/// </summary>
		public List<INamedListBinaryTag> Payload { get; } = new List<INamedListBinaryTag>(); 
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			output.EncodePayload(Payload.Count);
			for (var i = 0; i < Payload.Count; i++)
			{
				output.WriteByte((byte)Payload[i].ListType);
				Payload[i].EncodePayload(output);
			}
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			var size = input.DecodeInt32Payload();
			Payload.Clear();
			for (var i = 1; i<= size;i++)
			{
				var b = input.ReadByte();
				if (b < 0) throw new InvalidDataException("Expected byte for list type.");
				var t = (TagType) b;

				var builders = PayloadHelper.Builders.Where(x => x.Item1 == TagType.List && x.Item2 == t).ToArray();
				if (builders.Length == 0) throw new InvalidDataException($"Failed to locate builder for List:{t} tag.");
				if (builders.Length != 1) throw new InvalidOperationException($"Located multiple builders for List:{t} tag.");

				var item = (INamedListBinaryTag) builders[0].Item3(input, $"Item {i}");
				item.DecodePayload(input);
				Payload.Add(item);
			}
		}
		
		/// <inheritdoc />
		public IEnumerator GetEnumerator() => Payload.GetEnumerator();
	}
}
