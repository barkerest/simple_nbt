using System.IO;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class IntTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.Int, new IntTag().Type);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		[InlineData(1)]
		[InlineData(int.MinValue)]
		[InlineData(int.MaxValue)]
		public void EncodeDecodeCorrectly(int value)
		{
			var item = new IntTag(){Payload = value};
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new IntTag();
			item.DecodePayload(strm);
			
			Assert.Equal(value, item.Payload);
		}
	}
}