using System.IO;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Tags
{
	public class ListListTag_Should
	{
		[Fact]
		public void HaveTheCorrectType()
		{
			var item = new ListListTag();
			Assert.Equal(TagType.List, item.Type);
			Assert.Equal(TagType.List, item.ListType);
		}

		[Fact]
		public void EncodeDecodeCorrectly()
		{
			var item = new ListListTag();
			var list1 = new ListTag<StringTag>();
			var list2 = new ListTag<IntTag>();
			item.Add(list1);
			item.Add(list2);
			list1.Add(new StringTag(){Payload = "Hello"});
			list1.Add(new StringTag(){Payload = "World"});
			list2.Add(new IntTag(){Payload = -1});
			list2.Add(new IntTag(){Payload = 4200});
			
			var strm = new MemoryStream();
			item.EncodePayload(strm);

			strm.Position = 0;
			item = new ListListTag();
			item.DecodePayload(strm);
			
			Assert.Equal(2, item.Count);
			list1 = item[0] as ListTag<StringTag>;
			Assert.NotNull(list1);
			Assert.Equal(2, list1.Count);
			Assert.Equal("Hello", list1[0].Payload);
			Assert.Equal("World", list1[1].Payload);

			list2 = item[1] as ListTag<IntTag>;
			Assert.NotNull(list2);
			Assert.Equal(2, list2.Count);
			Assert.Equal(-1, list2[0].Payload);
			Assert.Equal(4200, list2[1].Payload);
		}
	}
}