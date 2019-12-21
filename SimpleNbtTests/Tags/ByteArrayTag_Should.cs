using System.IO;
using System.Linq;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class ByteArrayTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.ByteArray, new ByteArrayTag().Type);
		}

		[Theory]
		[InlineData(null)]
		[InlineData(new byte[0])]
		[InlineData(new byte[] {byte.MinValue, 1, 2, 3, 4, 5, 6, byte.MaxValue})]
		public void EncodeDecodeCorrectly(byte[] value)
		{
			var item = new ByteArrayTag() {Payload = value};
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;

			item = new ByteArrayTag();
			item.DecodePayload(strm);

			if (value is null)
			{
				Assert.Empty(item.Payload);
			}
			else
			{
				Assert.True(value.SequenceEqual(item.Payload));
			}
		}
	}
}