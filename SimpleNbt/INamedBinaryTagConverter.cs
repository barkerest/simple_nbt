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
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns>Returns a named binary tag.</returns>
        INamedBinaryTag ConvertToTag(string name, object value);

        /// <summary>
        /// Gets the value from the specified tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns>Returns the value of the tag.</returns>
        object ConvertFromTag(INamedBinaryTag tag);
    }
}