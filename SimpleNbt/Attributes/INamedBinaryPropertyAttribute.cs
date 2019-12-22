using System;

namespace SimpleNbt.Attributes
{
	/// <summary>
	/// Common interface for named binary property attributes.
	/// </summary>
	public interface INamedBinaryPropertyAttribute
	{
		/// <summary>
		/// The name of the named binary tag for this property.  Defaults to the property name.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Determines if the converter should ignore this property.
		/// </summary>
		bool Ignore { get; }
		
		/// <summary>
		/// Tells the converter to use a specific named binary tag to hold the value of this property.
		/// </summary>
		Type TagType { get; }
	}
}