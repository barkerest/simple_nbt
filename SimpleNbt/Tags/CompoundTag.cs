using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag that contains other tags.
	/// </summary>
	public sealed class CompoundTag : INamedBinaryTag, IDictionary<string, INamedBinaryTag>, IList<INamedBinaryTag>
	{
		public CompoundTag()
		{
			Name = "";
		}
		
		/// <summary>
		/// Creates a new compound tag.
		/// </summary>
		/// <param name="name"></param>
		public CompoundTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.Compound;

		/// <inheritdoc />
		public string Name { get; }

		private readonly List<INamedBinaryTag> _children = new List<INamedBinaryTag>();
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			foreach (var child in _children)
			{
				output.EncodeTag(child);
			}
			output.WriteByte((byte)TagType.End);
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			Clear();
			
			while (true)
			{
				var child = input.DecodeTag();
				if (child.Type == TagType.End) return;
				Add(child);
			}
		}

		/// <inheritdoc />
		IEnumerator<INamedBinaryTag> IEnumerable<INamedBinaryTag>.GetEnumerator() => _children.GetEnumerator();

		/// <inheritdoc />
		IEnumerator<KeyValuePair<string, INamedBinaryTag>> IEnumerable<KeyValuePair<string, INamedBinaryTag>>.GetEnumerator()
		{
			foreach (var child in _children)
			{
				yield return new KeyValuePair<string, INamedBinaryTag>(child.Name, child);
			}
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();

		/// <inheritdoc />
		void ICollection<KeyValuePair<string, INamedBinaryTag>>.Add(KeyValuePair<string, INamedBinaryTag> item)
		{
			if (item.Value is null) throw new ArgumentException("Item cannot be null.");
			if (!string.Equals(item.Key, item.Value.Name)) throw new ArgumentException("Item name must match the key.");
			Add(item.Value);
		}

		/// <inheritdoc />
		public void Add(INamedBinaryTag item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			if (_children.Any(x => string.Equals(x.Name, item.Name))) throw new ArgumentException("Item name already exists in the collection.");
			_children.Add(item);
		}

		/// <inheritdoc cref="ICollection{T}.Clear" />
		public void Clear() => _children.Clear();

		/// <inheritdoc />
		public bool Contains(INamedBinaryTag item) => item != null && _children.Any(x => string.Equals(x.Name, item.Name));

		/// <inheritdoc />
		public void CopyTo(INamedBinaryTag[] array, int arrayIndex) => _children.CopyTo(array, arrayIndex);
		
		/// <inheritdoc />
		public bool Remove(INamedBinaryTag item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			
			var idx = _children.FindIndex(x => string.Equals(x.Name, item.Name));
			if (idx < 0) return false;
			_children.RemoveAt(idx);
			return true;
		}

		/// <inheritdoc />
		bool ICollection<KeyValuePair<string, INamedBinaryTag>>.Contains(KeyValuePair<string, INamedBinaryTag> item)
		{
			if (item.Value is null) throw new ArgumentException("Item cannot be null.");
			if (!string.Equals(item.Key, item.Value.Name)) throw new ArgumentException("Item name must match the key.");
			return Contains(item.Value);
		}

		/// <inheritdoc />
		void ICollection<KeyValuePair<string, INamedBinaryTag>>.CopyTo(KeyValuePair<string, INamedBinaryTag>[] array, int arrayIndex)
		{
			foreach (var child in _children)
			{
				array[arrayIndex] = new KeyValuePair<string, INamedBinaryTag>(child.Name, child);
				arrayIndex++;
			}
		}

		/// <inheritdoc />
		bool ICollection<KeyValuePair<string, INamedBinaryTag>>.Remove(KeyValuePair<string, INamedBinaryTag> item)
		{
			if (item.Value is null) throw new ArgumentException("Item cannot be null.");
			if (!string.Equals(item.Key, item.Value.Name)) throw new ArgumentException("Item name must match the key.");
			return Remove(item.Value);
		}

		/// <inheritdoc cref="ICollection{T}.Count" />
		public int Count => _children.Count;

		/// <inheritdoc cref="ICollection{T}.IsReadOnly" />
		public bool IsReadOnly { get; } = false;
		
		/// <inheritdoc />
		public void Add(string key, INamedBinaryTag value)
		{
			if (value is null) throw new ArgumentNullException(nameof(value));
			if (!string.Equals(key, value.Name)) throw new ArgumentException("Item name must match the key.");
			Add(value);
		}

		/// <inheritdoc />
		public bool ContainsKey(string key) => _children.Any(x => string.Equals(x.Name, key));
		
		/// <inheritdoc />
		public bool Remove(string key)
		{
			var idx = _children.FindIndex(x => string.Equals(x.Name, key));
			if (idx < 0) return false;
			_children.RemoveAt(idx);
			return true;
		}

		/// <inheritdoc />
		public bool TryGetValue(string key, out INamedBinaryTag value)
		{
			value = null;
			var idx = _children.FindIndex(x => string.Equals(x.Name, key));
			if (idx < 0) return false;
			value = _children[idx];
			return true;
		}

		/// <inheritdoc />
		public INamedBinaryTag this[string key]
		{
			get
			{
				TryGetValue(key, out var ret);
				return ret;
			}
			set
			{
				var idx = _children.FindIndex(x => string.Equals(x.Name, key));
				if (idx < 0)
				{
					if (value is null) return;
					_children.Add(value);
				}
				else
				{
					if (value is null)
					{
						_children.RemoveAt(idx);
					}
					else
					{
						_children[idx] = value;
					}
				}
			}
		}

		/// <inheritdoc />
		public ICollection<string> Keys => _children.Select(x => x.Name).ToArray();

		/// <inheritdoc />
		public ICollection<INamedBinaryTag> Values => _children.ToArray();

		/// <inheritdoc />
		public int IndexOf(INamedBinaryTag item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			return _children.FindIndex(x => string.Equals(x.Name, item.Name));
		}

		/// <inheritdoc />
		public void Insert(int index, INamedBinaryTag item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			var idx = _children.FindIndex(x => string.Equals(x.Name, item.Name));
			if (idx > -1)throw new ArgumentException("Item name already exists in the collection.");
			_children.Insert(index, item);
		}
		
		/// <inheritdoc />
		public void RemoveAt(int index) => _children.RemoveAt(index);

		/// <inheritdoc />
		public INamedBinaryTag this[int index]
		{
			get => _children[index];
			set
			{
				if (value is null) throw new ArgumentNullException(nameof(value));
				var idx = _children.FindIndex(x => string.Equals(x.Name, value.Name));
				if (idx > -1 && idx != index) throw new ArgumentException("Item name already exists in the collection.");
				_children[index] = value;
			}
		}
	}
}
