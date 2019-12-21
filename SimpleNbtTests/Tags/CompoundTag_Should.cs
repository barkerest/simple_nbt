using System.IO;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class CompoundTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			Assert.Equal(TagType.Compound, new CompoundTag().Type);
		}

		[Fact]
		public void EncodeDecodeCorrectly()
		{
			var item = new CompoundTag();
			item.Add(new IntTag("intVal"){Payload = 12345});
			item.Add(new StringTag("stringVal"){Payload = "Hello World"});
			
			var strm = new MemoryStream();
			item.EncodePayload(strm);
			strm.Position = 0;
			
			item = new CompoundTag();
			item.DecodePayload(strm);
			
			Assert.True(item.ContainsKey("intVal"));
			Assert.True(item.ContainsKey("stringVal"));
			Assert.Equal(12345, ((IntTag)item["intVal"]).Payload);
			Assert.Equal("Hello World", ((StringTag)item["stringVal"]).Payload);

		}
	}
}