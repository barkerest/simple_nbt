using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag storing a single signed 16-bit integer value. 
	/// </summary>
	public sealed class ShortTag : INamedBinaryTag<short>
	{
		public ShortTag()
		{
			Name = "";
		}
		
		public ShortTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.Short;
		
		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The signed 16-bit integer value.
		/// </summary>
		public short Payload { get; set; }

		/// <inheritdoc />
		public void EncodePayload(Stream output)
			=> output.EncodePayload(Payload);

		/// <inheritdoc />
		public void DecodePayload(Stream input)
			=> Payload = input.DecodeInt16Payload();

		public override string ToString()
		{
			return $"{Payload}S";
		}
	}
}
