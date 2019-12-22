using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag holding a double precision floating point value.
	/// </summary>
	public sealed class DoubleTag : INamedBinaryTag<double>
	{
		public DoubleTag()
		{
			Name = "";
		}
		
		public DoubleTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.Double;
		
		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The double precision floating point value.
		/// </summary>
		public double Payload { get; set; }

		/// <inheritdoc />
		public void EncodePayload(Stream output)
			=> output.EncodePayload(Payload);

		/// <inheritdoc />
		public void DecodePayload(Stream input)
			=> Payload = input.DecodeDoublePayload();

		public override string ToString()
		{
			return $"{Payload}D";
		}
	}
}
