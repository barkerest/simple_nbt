using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using SimpleNbt;
using SimpleNbt.Converters;
using SimpleNbt.Tags;
using SimpleNbtTests.Models;
using Xunit;

namespace SimpleNbtTests
{
	public class Utility_Should
	{
		private static string TestFileDirectory([CallerFilePath] string filename = "") =>
			Path.GetDirectoryName(filename) + @"/data/";

		[Fact]
		public void LoadHelloWorld()
		{
			var file = TestFileDirectory() + @"uncompressed/hello_world.nbt";

			var data = Utility.LoadFromFile(file, false);
			
			Assert.NotNull(data);
			Assert.Equal("hello world", data.Name);
			Assert.Equal(1, data.Count);
			Assert.True(data.ContainsKey("name"));
			var name = data["name"] as StringTag;
			Assert.NotNull(name);
			Assert.Equal("Bananrama", name.Payload);
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void LoadBigTest(bool compressed)
		{
			var file = TestFileDirectory() + (compressed ? "compressed" : "uncompressed") + @"/bigtest.nbt";

			var data = Utility.LoadFromFile(file, compressed);

			Assert.NotNull(data);
			
			Assert.Equal("Level", data.Name);
			
			Assert.True(data.ContainsKey("nested compound test"));
			var nested1 = data["nested compound test"] as CompoundTag;
			Assert.NotNull(nested1);
			Assert.True(nested1.ContainsKey("egg"));
			Assert.True(nested1.ContainsKey("ham"));
			
			var nested2 = nested1["egg"] as CompoundTag;
			Assert.NotNull(nested2);
			Assert.True(nested2.ContainsKey("name"));
			Assert.True(nested2.ContainsKey("value"));
			var stringTag = nested2["name"] as StringTag;
			Assert.NotNull(stringTag);
			Assert.Equal("Eggbert", stringTag.Payload);
			var singleTag = nested2["value"] as SingleTag;
			Assert.NotNull(singleTag);
			Assert.Equal(0.5f, singleTag.Payload);
			
			nested2 = nested1["ham"] as CompoundTag;
			Assert.NotNull(nested2);
			Assert.True(nested2.ContainsKey("name"));
			Assert.True(nested2.ContainsKey("value"));
			stringTag = nested2["name"] as StringTag;
			Assert.NotNull(stringTag);
			Assert.Equal("Hampus", stringTag.Payload);
			singleTag = nested2["value"] as SingleTag;
			Assert.NotNull(singleTag);
			Assert.Equal(0.75f, singleTag.Payload);

			Assert.True(data.ContainsKey("intTest"));
			var intTag = data["intTest"] as IntTag;
			Assert.NotNull(intTag);
			Assert.Equal(2147483647, intTag.Payload);
			
			Assert.True(data.ContainsKey("byteTest"));
			var byteTag = data["byteTest"] as ByteTag;
			Assert.NotNull(byteTag);
			Assert.Equal(127, byteTag.Payload);

			Assert.True(data.ContainsKey("shortTest"));
			var shortTag = data["shortTest"] as ShortTag;
			Assert.NotNull(shortTag);
			Assert.Equal(32767, shortTag.Payload);
			
			Assert.True(data.ContainsKey("stringTest"));
			stringTag = data["stringTest"] as StringTag;
			Assert.NotNull(stringTag);
			Assert.Equal("HELLO WORLD THIS IS A TEST STRING ÅÄÖ!", stringTag.Payload);
			
			Assert.True(data.ContainsKey("doubleTest"));
			var doubleTag = data["doubleTest"] as DoubleTag;
			Assert.NotNull(doubleTag);
			Assert.Equal(0.49312871321823148, doubleTag.Payload);
			
			Assert.True(data.ContainsKey("floatTest"));
			singleTag = data["floatTest"] as SingleTag;
			Assert.NotNull(singleTag);
			Assert.Equal(0.49823147058486938f, singleTag.Payload);
			
			Assert.True(data.ContainsKey("longTest"));
			var longTag = data["longTest"] as LongTag;
			Assert.NotNull(longTag);
			Assert.Equal(9223372036854775807L, longTag.Payload);
			
			Assert.True(data.ContainsKey("listTest (long)"));
			var longListTag = data["listTest (long)"] as ListTag<LongTag>;
			Assert.NotNull(longListTag);
			Assert.Equal(5, longListTag.Count);
			Assert.Equal(11, longListTag[0].Payload);
			Assert.Equal(12, longListTag[1].Payload);
			Assert.Equal(13, longListTag[2].Payload);
			Assert.Equal(14, longListTag[3].Payload);
			Assert.Equal(15, longListTag[4].Payload);

			Assert.True(data.ContainsKey("listTest (compound)"));
			var compoundListTag = data["listTest (compound)"] as ListTag<CompoundTag>;
			Assert.NotNull(compoundListTag);
			
			Assert.Equal(2, compoundListTag.Count);
			
			var compoundTag = compoundListTag[0];
			Assert.True(compoundTag.ContainsKey("created-on"));
			Assert.True(compoundTag.ContainsKey("name"));
			longTag = compoundTag["created-on"] as LongTag;
			Assert.NotNull(longTag);
			Assert.Equal(1264099775885L, longTag.Payload);
			stringTag = compoundTag["name"] as StringTag;
			Assert.NotNull(stringTag);
			Assert.Equal("Compound tag #0", stringTag.Payload);
			
			compoundTag = compoundListTag[1];
			Assert.True(compoundTag.ContainsKey("created-on"));
			Assert.True(compoundTag.ContainsKey("name"));
			longTag = compoundTag["created-on"] as LongTag;
			Assert.NotNull(longTag);
			Assert.Equal(1264099775885L, longTag.Payload);
			stringTag = compoundTag["name"] as StringTag;
			Assert.NotNull(stringTag);
			Assert.Equal("Compound tag #1", stringTag.Payload);

			Assert.True(data.ContainsKey("byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))"));
			var byteArrayTag =
				data["byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))"]
					as ByteArrayTag;
			Assert.NotNull(byteArrayTag);
			Assert.NotNull(byteArrayTag.Payload);
			var bytes = byteArrayTag.Payload;
			Assert.Equal(1000, bytes.Length);
			for (var n = 0; n < 1000; n++)
			{
				var x = (byte)(((n * n * 255) + (n * 7)) % 100);
				Assert.Equal(x, bytes[n]);
			}
		}

		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void AutodetectCompression(bool compressed)
		{
			var file = TestFileDirectory() + (compressed ? "compressed" : "uncompressed") + @"/bigtest.nbt";

			var data = Utility.LoadFromFile(file, null);
			
			Assert.NotNull(data);
			Assert.Equal("Level", data.Name);
		}

		[Fact]
		public void LoadBigTestClass()
		{
			var file = TestFileDirectory() + @"compressed/bigtest.nbt";

			var data = Utility.LoadFromFile(file);

			var entity = ObjectConverter.ConvertFromTag<BigTest>(data, NamingConvention.LowerCamelCase);

			var level = entity.Level;
			Assert.NotNull(level);
			
		}
	}
}