using System;

namespace SimpleNbt.Converters
{
    /// <summary>
    /// A generic converter for simple types.
    /// </summary>
    /// <typeparam name="TTag">The named binary tag type.</typeparam>
    /// <typeparam name="TData">The type of data stored in the tag.</typeparam>
    public sealed class SimpleTypeConverter<TTag, TData> : INamedBinaryTagConverter where TTag : class, INamedBinaryTag<TData>
    {
        private readonly Func<string, TTag> _construct;
        
        public SimpleTypeConverter(Func<string, TTag> construct)
        {
            _construct = construct ?? throw new ArgumentNullException(nameof(construct));
        }

        /// <inheritdoc />
        public Type DataType { get; } = typeof(TData);
        
        /// <inheritdoc />
        public Type TagType { get; } = typeof(TTag);
        
        /// <inheritdoc />
        public INamedBinaryTag ConvertToTag(string name, object value)
        {
            var ret = _construct(name);
            ret.Payload = (TData) value;
            return ret;
        }

        /// <inheritdoc />
        public object ConvertFromTag(INamedBinaryTag tag)
        {
            var t = tag as TTag ?? throw new InvalidCastException();
            return t.Payload;
        }
    }

    /// <summary>
    /// A generic converter for simple types.
    /// </summary>
    /// <typeparam name="TTag">The named binary tag type.</typeparam>
    /// <typeparam name="TTagData">The type of data stored in the tag.</typeparam>
    /// <typeparam name="TRetData">The type of data being converted.</typeparam>
    public sealed class SimpleTypeConverter<TTag, TTagData, TRetData> : INamedBinaryTagConverter
        where TTag : class, INamedBinaryTag<TTagData>
    {
        private readonly Func<string, TTag> _construct;
        private readonly Func<TTagData, TRetData> _to;
        private readonly Func<TRetData, TTagData> _from;
        
        public SimpleTypeConverter(Func<string, TTag> construct, Func<TTagData, TRetData> convertTo, Func<TRetData, TTagData> convertFrom)
        {
            _construct = construct ?? throw new ArgumentNullException(nameof(construct));
            _to = convertTo ?? throw new ArgumentNullException(nameof(convertTo));
            _from = convertFrom ?? throw new ArgumentNullException(nameof(convertFrom));
        }

        /// <inheritdoc />
        public Type DataType { get; } = typeof(TRetData);
        
        /// <inheritdoc />
        public Type TagType { get; } = typeof(TTag);
        
        /// <inheritdoc />
        public INamedBinaryTag ConvertToTag(string name, object value)
        {
            var ret = _construct(name);
            ret.Payload = _from((TRetData) value);
            return ret;
        }

        /// <inheritdoc />
        public object ConvertFromTag(INamedBinaryTag tag)
        {
            var t = tag as TTag ?? throw new InvalidCastException();
            return _to(t.Payload);
        }
    }
}