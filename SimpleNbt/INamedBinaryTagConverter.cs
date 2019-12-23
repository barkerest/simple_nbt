using System;

namespace SimpleNbt
{
    /// <summary>
    /// Defines a converter for tags.
    /// </summary>
    public interface INamedBinaryTagConverter
    {
        /// <summary>
        /// The data type handled by this converter.
        /// </summary>
        Type DataType { get; }
        
        /// <summary>
        /// The type of the tag handled by this converter.
        /// </summary>
        Type TagType { get; }

        /// <summary>
        /// Creates a tag with the specified value.
        /// </summary>
        /// <param name="name">The name for this tag.</param>
        /// <param name="value">The value for this tag.</param>
        /// <param name="convention">The naming convention to use when naming child tags.</param>
        /// <returns>Returns a named binary tag.</returns>
        INamedBinaryTag ConvertToTag(string name, object value, NamingConvention convention);

        /// <summary>
        /// Gets the value from the specified tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="convention">The naming convention to use when decoding child tags.</param>
        /// <returns>Returns the value of the tag.</returns>
        object ConvertFromTag(INamedBinaryTag tag, NamingConvention convention);
    }
}