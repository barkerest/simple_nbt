using System.IO;
using System.Runtime.InteropServices;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class DoubleTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.Double, new DoubleTag().Type);
		}

		[Theory]
		[InlineData(double.MinValue)]
		[InlineData(double.MaxValue)]
		[InlineData(1.0)]
		[InlineData(-1.0)]
		[InlineData(0.0)]
		[InlineData(double.NegativeInfinity)]
		[InlineData(double.PositiveInfinity)]
		[InlineData(double.NaN)]
		public void EncodeDecodeCorrectly(double value)
		{
			var item = new DoubleTag(){Payload = value};
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new DoubleTag();
			item.DecodePayload(strm);
			
			Assert.Equal(value, item.Payload);
		}
	}
}