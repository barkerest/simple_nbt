using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag storing a single signed 64-bit integer value. 
	/// </summary>
	public sealed class LongTag : INamedBinaryTag
	{
		public LongTag()
		{
			Name = "";
		}
		
		public LongTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.Long;
		
		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The signed 64-bit integer value.
		/// </summary>
		public long Payload { get; set; }

		/// <inheritdoc />
		public void EncodePayload(Stream output)
			=> output.EncodePayload(Payload);

		/// <inheritdoc />
		public void DecodePayload(Stream input)
			=> Payload = input.DecodeInt64Payload();
	}
}
