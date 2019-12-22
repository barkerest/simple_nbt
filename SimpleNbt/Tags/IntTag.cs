using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag storing a single signed 32-bit integer.
	/// </summary>
	public sealed class IntTag : INamedBinaryTag<int>
	{
		public IntTag()
		{
			Name = "";
		}
		
		public IntTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.Int;
		
		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The signed 32-bit integer value.
		/// </summary>
		public int Payload { get; set; }

		/// <inheritdoc />
		public void EncodePayload(Stream output)
			=> output.EncodePayload(Payload);

		/// <inheritdoc />
		public void DecodePayload(Stream input)
			=> Payload = input.DecodeInt32Payload();
	}
}
