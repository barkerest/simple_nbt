using System.IO;
using System.IO.Enumeration;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class LongTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.Long, new LongTag().Type);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		[InlineData(long.MinValue)]
		[InlineData(long.MaxValue)]
		[InlineData(4200)]
		public void EncodeDecodeCorrectly(long value)
		{
			var item = new LongTag(){Payload = value};
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new LongTag();
			item.DecodePayload(strm);
			
			Assert.Equal(value, item.Payload);
		}
	}
}