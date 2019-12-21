using System.IO;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class SingleTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.Float, new SingleTag().Type);
		}

		[Theory]
		[InlineData(float.MinValue)]
		[InlineData(float.MaxValue)]
		[InlineData(1.0f)]
		[InlineData(-1.0f)]
		[InlineData(0.0f)]
		[InlineData(float.NegativeInfinity)]
		[InlineData(float.PositiveInfinity)]
		[InlineData(float.NaN)]
		public void EncodeDecodeCorrectly(float value)
		{
			var item = new SingleTag(){Payload = value};
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new SingleTag();
			item.DecodePayload(strm);
			
			Assert.Equal(value, item.Payload);
		}
	}
}