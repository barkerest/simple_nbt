using System.IO;

namespace SimpleNbt.Tags
{
	/// <summary>
	/// A tag holding an array of 64-bit integers.
	/// </summary>
	public sealed class LongArrayTag : INamedBinaryTag
	{
		public LongArrayTag()
		{
			Name = "";
		}
		
		public LongArrayTag(string name)
		{
			Name = name;
		}
		
		/// <inheritdoc />
		public TagType Type { get; } = TagType.IntArray;
		
		/// <inheritdoc />
		public string Name { get; }
		
		/// <summary>
		/// The array of 64-bit integers stored in this tag.
		/// </summary>
		public long[] Payload { get; set; }
		
		/// <inheritdoc />
		public void EncodePayload(Stream output)
		{
			var size = Payload?.Length ?? 0;
			output.EncodePayload(size);
			if (size < 1) return;
			for (var i = 0; i < size; i++)
			{
				output.EncodePayload(Payload[i]);
			}
		}

		/// <inheritdoc />
		public void DecodePayload(Stream input)
		{
			var size = input.DecodeInt32Payload();
			Payload = new long[size];
			for (var i = 0; i < size; i++)
			{
				Payload[i] = input.DecodeInt64Payload();
			}
		}
	}
}