using System.Collections;

namespace SimpleNbt
{
	/// <summary>
	/// Common attributes for the list.
	/// </summary>
	public interface INamedListBinaryTag : INamedBinaryTag, IList
	{
		/// <summary>
		/// The type of the data entries in this list.
		/// </summary>
		TagType ListType { get; }
		
	}
}
