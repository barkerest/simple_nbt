using System.IO;
using System.Runtime.InteropServices;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class ShortTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.Short, new ShortTag().Type);
		}

		[Theory]
		[InlineData((short)0)]
		[InlineData((short)-1)]
		[InlineData((short)4200)]
		[InlineData(short.MinValue)]
		[InlineData(short.MaxValue)]
		public void EncodeDecodeCorrectly(short value)
		{
			var item = new ShortTag() {Payload = value};
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new ShortTag();
			item.DecodePayload(strm);
			
			Assert.Equal(value, item.Payload);
		}
		
		
	}
}