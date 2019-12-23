using System;
using SimpleNbt;
using SimpleNbt.Converters;
using SimpleNbt.Tags;
using Xunit;

namespace SimpleNbtTests.Converters
{
	public class ObjectConverter_Should
	{
		private const NamingConvention Convention = NamingConvention.LowerCamelCase;
		
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ConvertBooleanValues(bool value)
		{
			var tag = ObjectConverter.ConvertToTag("TestBoolean", value, Convention);
			Assert.IsType<ByteTag>(tag);
			var result = ObjectConverter.ConvertFromTag(tag, Convention);
			Assert.IsType<bool>(result);
			Assert.Equal(value, (bool)result);
		}
		
		
		
	}
}
