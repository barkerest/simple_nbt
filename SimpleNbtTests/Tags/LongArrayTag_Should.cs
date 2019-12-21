using System.IO;
using System.Linq;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class LongArrayTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.LongArray, new LongArrayTag().Type);
		}

		[Theory]
		[InlineData(null)]
		[InlineData(new long[0])]
		[InlineData(new long[]{ long.MinValue, 1, 2, -1, -2, long.MaxValue})]
		public void EncodeDecodeCorrectly(long[] value)
		{
			var item = new LongArrayTag() {Payload = value};
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new LongArrayTag();
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