using System.IO;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class StringTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.String, new StringTag().Type);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("      ")]
		[InlineData("Hello World")]
		[InlineData("© 2019 §!~╤╥")]
		public void EncodeDecodeCorrectly(string value)
		{
			var item = new StringTag(){Payload = value};
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new StringTag();
			item.DecodePayload(strm);

			if (string.IsNullOrEmpty(value))
			{
				Assert.True(string.IsNullOrEmpty(item.Payload));
			}
			else
			{
				Assert.Equal(value, item.Payload);
			}
		}
	}
}