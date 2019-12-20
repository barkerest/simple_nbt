using System.IO;

namespace SimpleNbt
{
	/// <summary>
	/// Common interface for named binary tags.
	/// </summary>
	public interface INamedBinaryTag
	{
		/// <summary>
		/// The type of tag this represents.
		/// </summary>
		TagType Type { get; }
		
		/// <summary>
		/// The name of this tag.
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Writes the payload to the stream. 
		/// </summary>
		/// <param name="output"></param>
		void EncodePayload(Stream output);

		/// <summary>
		/// Reads the payload from the stream.
		/// </summary>
		/// <param name="input"></param>
		void DecodePayload(Stream input);

	}
}
