using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// The end of a compound tag list.
	/// </summary>
	public sealed class EndTag : INamedBinaryTag
	{
		public EndTag()
		{
			
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.End;
		
		/// <summary>
		/// Not used.
		/// </summary>
		public string Name { get; } = "";
		
		/// <summary>
		/// Not used.
		/// </summary>
		/// <param name="output"></param>
		public void EncodePayload(Stream output)
		{
			// no payload to encode.
		}

		/// <summary>
		/// Not used.
		/// </summary>
		/// <param name="input"></param>
		public void DecodePayload(Stream input)
		{
			// no payload to decode.
		}
	}
}
