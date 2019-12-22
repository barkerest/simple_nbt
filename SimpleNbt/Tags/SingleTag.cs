using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag holding a single precision floating point value.
	/// </summary>
	public sealed class SingleTag : INamedBinaryTag<float>
	{
		public SingleTag()
		{
			Name = "";
		}
		
		public SingleTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.Float;
		
		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The single precision floating point value.
		/// </summary>
		public float Payload { get; set; }

		/// <inheritdoc />
		public void EncodePayload(Stream output)
			=> output.EncodePayload(Payload);

		/// <inheritdoc />
		public void DecodePayload(Stream input)
			=> Payload = input.DecodeSinglePayload();

	}
}
