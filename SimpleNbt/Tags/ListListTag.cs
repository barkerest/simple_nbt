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
	public sealed class ListListTag : INamedListBinaryTag, IList<INamedListBinaryTag>
	{
		public ListListTag()
		{
			Name = "";
		}
		
		public ListListTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.List;
		
		/// <inheritdoc />
		public TagType ListType { get; } = TagType.List;

		/// <inheritdoc />
		public string Name { get; }
		
		private readonly List<INamedListBinaryTag> _payload = new List<INamedListBinaryTag>(); 
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			output.EncodePayload(_payload.Count);
			for (var i = 0; i < _payload.Count; i++)
			{
				output.WriteByte((byte)_payload[i].ListType);
				_payload[i].EncodePayload(output);
			}
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			var size = input.DecodeInt32Payload();
			Clear();
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
				_payload.Add(item);
			}
		}

		/// <inheritdoc />
		IEnumerator<INamedListBinaryTag> IEnumerable<INamedListBinaryTag>.GetEnumerator() => _payload.GetEnumerator();

		/// <inheritdoc />
		public IEnumerator GetEnumerator() => _payload.GetEnumerator();

		/// <inheritdoc />
		public void Add(INamedListBinaryTag item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_payload.Add(item);
		}

		/// <inheritdoc />
		public void Clear() => _payload.Clear();

		/// <inheritdoc />
		public bool Contains(INamedListBinaryTag item) => _payload.Contains(item);

		/// <inheritdoc />
		public void CopyTo(INamedListBinaryTag[] array, int arrayIndex) => _payload.CopyTo(array, arrayIndex);

		/// <inheritdoc />
		public bool Remove(INamedListBinaryTag item) => _payload.Remove(item);

		/// <inheritdoc />
		public int Count => _payload.Count;

		/// <inheritdoc />
		public bool IsReadOnly { get; } = false;

		/// <inheritdoc />
		public int IndexOf(INamedListBinaryTag item) => _payload.IndexOf(item);

		/// <inheritdoc />
		public void Insert(int index, INamedListBinaryTag item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_payload.Insert(index, item);
		}

		/// <inheritdoc />
		public void RemoveAt(int index) => _payload.RemoveAt(index);

		/// <inheritdoc />
		public INamedListBinaryTag this[int index]
		{
			get => _payload[index];
			set
			{
				if (value is null) throw new ArgumentNullException(nameof(value));
				_payload[index] = value;
			}
		}

		public override string ToString()
		{
			return "[" + string.Join(",", _payload) + "]";
		}
	}
}