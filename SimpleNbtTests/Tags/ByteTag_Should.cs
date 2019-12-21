using System.IO;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class ByteTag_Should
	{

		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.Byte, new ByteTag().Type);
		}

		[Theory]
		[InlineData(sbyte.MinValue)]
		[InlineData(sbyte.MaxValue)]
		[InlineData((sbyte)101)]
		public void EncodeDecodeCorrectly(sbyte value)
		{
			var item = new ByteTag(){ Payload = value };
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new ByteTag();
			item.DecodePayload(strm);
			
			Assert.Equal(value, item.Payload);
		}
	}
}