using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag storing a single signed byte.
	/// </summary>
	public sealed class ByteTag : INamedBinaryTag<sbyte>
	{
		public ByteTag()
		{
			Name = "";
		}
		
		public ByteTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.Byte;
		
		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The signed byte value.
		/// </summary>
		public sbyte Payload { get; set; }
		
		/// <inheritdoc />
		public void EncodePayload(Stream output) => output.EncodePayload(Payload);

		/// <inheritdoc />
		public void DecodePayload(Stream input) => Payload = input.DecodeInt8Payload();

		public override string ToString()
		{
			return $"{Payload}B";
		}
	}
}
