using System.IO;
using System.Linq;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class IntArrayTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.IntArray, new IntArrayTag().Type);
		}

		[Theory]
		[InlineData(null)]
		[InlineData(new int[0])]
		[InlineData(new int[]{int.MinValue, int.MaxValue, 1, 2, 3, -1, -2, -3})]
		public void EncodeDecodeCorrectly(int[] value)
		{
			var item = new IntArrayTag(){Payload = value};
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new IntArrayTag();
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