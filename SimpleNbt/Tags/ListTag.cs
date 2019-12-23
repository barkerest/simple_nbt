using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A list of tags.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class ListTag<T> : INamedListBinaryTag, IList<T> where T : class, INamedBinaryTag, new()
	{
		public ListTag()
		{
			ListType = new T().Type;
			Name = "";
		}
		
		public ListTag(string name)
		{
			ListType = new T().Type;
			Name = name;
		}

		/// <inheritdoc />
		public TagType Type { get; } = TagType.List;

		/// <inheritdoc />
		public TagType ListType { get; }

		/// <inheritdoc />
		public string Name { get; }

		private readonly List<T> _payload = new List<T>();
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			var size = _payload.Count;
			output.EncodePayload(size);
			for (var i = 0; i < size; i++)
			{
				_payload[i].EncodePayload(output);
			}
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			var size = input.DecodeInt32Payload();
			Clear();
			for (var i = 0; i < size; i++)
			{
				var item = new T();
				item.DecodePayload(input);
				_payload.Add(item);
			}
		}

		/// <inheritdoc />
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => _payload.GetEnumerator();

		/// <inheritdoc />
		public IEnumerator GetEnumerator() => _payload.GetEnumerator();

		/// <inheritdoc />
		public void Add(T item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_payload.Add(item);
		}
		
		/// <inheritdoc />
		public void Clear() => _payload.Clear();

		/// <inheritdoc />
		public bool Contains(T item) => _payload.Contains(item);

		/// <inheritdoc />
		public void CopyTo(T[] array, int arrayIndex) => _payload.CopyTo(array, arrayIndex);

		/// <inheritdoc />
		public bool Remove(T item) => _payload.Remove(item);

		/// <inheritdoc />
		public int Count => _payload.Count;

		/// <inheritdoc />
		public bool IsReadOnly { get; } = false;


		/// <inheritdoc />
		public int IndexOf(T item) => _payload.IndexOf(item);

		/// <inheritdoc />
		public void Insert(int index, T item)
		{
			if (item is null) throw new ArgumentNullException(nameof(item));
			_payload.Insert(index, item);
		}

		/// <inheritdoc />
		public void RemoveAt(int index) => _payload.RemoveAt(index);

		/// <inheritdoc />
		public T this[int index]
		{
			get => _payload[index];
			set
			{
				if (value is null) throw new ArgumentNullException(nameof(value));
				_payload[index] = value;
			}
		}

		#region IList

		int IList.Add(object value)
		{
			lock (((ICollection) _payload).SyncRoot)
			{
				var ret = _payload.Count;
				Add((T) value);
				return ret;
			}
		}

		bool IList.Contains(object value) => Contains((T) value);

		int IList.IndexOf(object value) => IndexOf((T) value);

		void IList.Insert(int index, object value) => Insert(index, (T) value);

		void IList.Remove(object value) => Remove((T) value);

		void ICollection.CopyTo(Array array, int index) => ((ICollection)_payload).CopyTo(array, index);

		bool ICollection.IsSynchronized => ((ICollection) _payload).IsSynchronized;

		object ICollection.SyncRoot => ((ICollection) _payload).SyncRoot;

		object IList.this[int index]
		{
			get => _payload[index];
			set
			{
				if (value is null) throw new ArgumentNullException(nameof(value));
				if (!(value is T val)) throw new InvalidCastException();
				_payload[index] = val;
			}
		}

		bool IList.IsFixedSize => ((IList) _payload).IsFixedSize;

		#endregion
		
		public override string ToString()
		{
			return "[" + string.Join(",", _payload) + "]";
		}
	}
}