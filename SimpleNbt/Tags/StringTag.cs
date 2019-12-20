using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag holding a string value.
	/// </summary>
	public sealed class StringTag : INamedBinaryTag
	{
		public StringTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.String;
		
		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The string value.
		/// </summary>
		public string Payload { get; set; }

		/// <inheritdoc />
		public void EncodePayload(Stream output)
			=> output.EncodePayload(Payload);

		/// <inheritdoc />
		public void DecodePayload(Stream input)
			=> Payload = input.DecodeStringPayload();
	}
}
