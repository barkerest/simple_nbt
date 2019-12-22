using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleNbt.Converters
{
    /// <summary>
    /// A generic converter for simple lists.
    /// </summary>
    /// <typeparam name="TListTag">The named binary tag type of the list.</typeparam>
    /// <typeparam name="TEntryTag">The named binary tag type stored in the list.</typeparam>
    /// <typeparam name="TEntryData">The type of data stored in the entry tags.</typeparam>
    /// <typeparam name="TList">The list type being converted.</typeparam>
    public sealed class SimpleListConverter<TListTag, TEntryTag, TEntryData, TList> : INamedBinaryTagConverter
        where TListTag : class, INamedListBinaryTag, IList<TEntryTag>
        where TEntryTag : class, INamedBinaryTag<TEntryData>, new()
        where TList : class, IEnumerable<TEntryData>
    {
        private readonly Func<string, TListTag> _construct;
        private readonly Func<IEnumerable<TEntryData>, TList> _deconstruct;

        public SimpleListConverter(Func<string, TListTag> construct, Func<IEnumerable<TEntryData>, TList> deconstruct)
        {
            _construct = construct ?? throw new ArgumentNullException(nameof(construct));
            _deconstruct = deconstruct ?? throw new ArgumentNullException(nameof(deconstruct));
        }
        
        /// <inheritdoc />
        public Type DataType { get; } = typeof(TList);
        
        /// <inheritdoc />
        public Type TagType { get; } = typeof(TListTag);
        
        /// <inheritdoc />
        public INamedBinaryTag ConvertToTag(string name, object value)
        {
            var values = value as IEnumerable<TEntryData>;
            if (values is null) throw new InvalidCastException();

            var ret = _construct(name);
            foreach (var val in values)
            {
                var item = new TEntryTag();
                item.Payload = val;
                ret.Add(item);
            }

            return ret;
        }

        /// <inheritdoc />
        public object ConvertFromTag(INamedBinaryTag tag)
        {
            var t = tag as TListTag;
            return _deconstruct(t.Select(x => x.Payload));
        }
    }

    /// <summary>
    /// A generic converter for simple lists.
    /// </summary>
    /// <typeparam name="TListTag">The named binary tag type of the list.</typeparam>
    /// <typeparam name="TEntryTag">The named binary tag type stored in the list.</typeparam>
    /// <typeparam name="TEntryData">The type of data stored in the entry tags.</typeparam>
    /// <typeparam name="TList">The list type being converted.</typeparam>
    /// <typeparam name="TListData">The type of data being converted.</typeparam>
    public sealed class SimpleListConverter<TListTag, TEntryTag, TEntryData, TList, TListData> : INamedBinaryTagConverter
        where TListTag : class, INamedListBinaryTag, IList<TEntryTag>
        where TEntryTag : class, INamedBinaryTag<TEntryData>, new()
        where TList : class, IEnumerable<TListData>
    {
        private readonly Func<string, TListTag> _construct;
        private readonly Func<IEnumerable<TListData>, TList> _deconstruct;
        private readonly Func<TEntryData, TListData> _to;
        private readonly Func<TListData, TEntryData> _from;

        public SimpleListConverter(Func<string, TListTag> construct, Func<IEnumerable<TListData>, TList> deconstruct, Func<TEntryData, TListData> convertTo, Func<TListData, TEntryData> convertFrom)
        {
            _construct = construct ?? throw new ArgumentNullException(nameof(construct));
            _deconstruct = deconstruct ?? throw new ArgumentNullException(nameof(deconstruct));
            _to = convertTo ?? throw new ArgumentNullException(nameof(convertTo));
            _from = convertFrom ?? throw new ArgumentNullException(nameof(convertFrom));
        }

        /// <inheritdoc />
        public Type DataType { get; } = typeof(TList);

        /// <inheritdoc />
        public Type TagType { get; } = typeof(TListTag);

        /// <inheritdoc />
        public INamedBinaryTag ConvertToTag(string name, object value)
        {
            var values = value as IEnumerable<TListData>;
            if (values is null) throw new InvalidCastException();

            var ret = _construct(name);
            foreach (var val in values)
            {
                var item = new TEntryTag();
                item.Payload = _from(val);
                ret.Add(item);
            }

            return ret;
        }

        /// <inheritdoc />
        public object ConvertFromTag(INamedBinaryTag tag)
        {
            var t = tag as TListTag;
            return _deconstruct(t.Select(x => _to(x.Payload)));
        }
    }
}