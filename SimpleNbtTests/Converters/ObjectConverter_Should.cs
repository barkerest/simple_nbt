using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		[InlineData("")]
		[InlineData("   ")]
		[InlineData("└ Hello ┐")]
		public void ConvertStringValues(string value)
		{
			var tag = ObjectConverter.ConvertToTag("StringTest", value, Convention);
			Assert.IsType<StringTag>(tag);
			var result = ObjectConverter.ConvertFromTag<string>(tag, Convention);
			Assert.Equal(value, result);
		}
		
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		public void ConvertBooleanValues(bool value)
		{
			var tag = ObjectConverter.ConvertToTag("TestBoolean", value, Convention);
			Assert.IsType<ByteTag>(tag);
			var result = ObjectConverter.ConvertFromTag<bool>(tag, Convention);
			Assert.Equal(value, result);
		}

		[Theory]
		[InlineData((sbyte) 0)]
		[InlineData((sbyte) -1)]
		[InlineData((sbyte) 127)]
		public void ConvertSignedByteValues(sbyte value)
		{
			var tag = ObjectConverter.ConvertToTag("TestSByte", value, Convention);
			Assert.IsType<ByteTag>(tag);
			var result = ObjectConverter.ConvertFromTag<sbyte>(tag, Convention);
			Assert.Equal(value, result);
		}
		
		[Theory]
		[InlineData((byte) 0)]
		[InlineData((byte) 255)]
		[InlineData((byte) 127)]
		public void ConvertUnsignedByteValues(byte value)
		{
			var tag = ObjectConverter.ConvertToTag("TestUByte", value, Convention);
			Assert.IsType<ByteTag>(tag);
			var result = ObjectConverter.ConvertFromTag<byte>(tag, Convention);
			Assert.Equal(value, result);
		}

		[Theory]
		[InlineData((short) 0)]
		[InlineData((short) -1)]
		[InlineData((short) 32767)]
		public void ConvertSignedShortValues(short value)
		{
			var tag = ObjectConverter.ConvertToTag("TestSShort", value, Convention);
			Assert.IsType<ShortTag>(tag);
			var result = ObjectConverter.ConvertFromTag<short>(tag, Convention);
			Assert.Equal(value, result);
		}

		[Theory]
		[InlineData((ushort) 0)]
		[InlineData((ushort) 65535)]
		[InlineData((ushort) 32767)]
		public void ConvertUnsignedShortValues(ushort value)
		{
			var tag = ObjectConverter.ConvertToTag("TestUShort", value, Convention);
			Assert.IsType<ShortTag>(tag);
			var result = ObjectConverter.ConvertFromTag<ushort>(tag, Convention);
			Assert.Equal(value, result);
		}
		
		[Theory]
		[InlineData((int) 0)]
		[InlineData((int) -1)]
		[InlineData(int.MinValue)]
		[InlineData(int.MaxValue)]
		public void ConvertSignedIntValues(int value)
		{
			var tag = ObjectConverter.ConvertToTag("TestSInt", value, Convention);
			Assert.IsType<IntTag>(tag);
			var result = ObjectConverter.ConvertFromTag<int>(tag, Convention);
			Assert.Equal(value, result);
		}
		
		[Theory]
		[InlineData((uint) 0)]
		[InlineData(uint.MaxValue)]
		[InlineData((uint) 32767)]
		public void ConvertUnsignedIntValues(uint value)
		{
			var tag = ObjectConverter.ConvertToTag("TestUInt", value, Convention);
			Assert.IsType<IntTag>(tag);
			var result = ObjectConverter.ConvertFromTag<uint>(tag, Convention);
			Assert.Equal(value, result);
		}

		[Theory]
		[InlineData((long) 0)]
		[InlineData((long) -1)]
		[InlineData(long.MinValue)]
		[InlineData(long.MaxValue)]
		public void ConvertSignedLongValues(long value)
		{
			var tag = ObjectConverter.ConvertToTag("TestSLong", value, Convention);
			Assert.IsType<LongTag>(tag);
			var result = ObjectConverter.ConvertFromTag<long>(tag, Convention);
			Assert.Equal(value, result);
		}
		
		[Theory]
		[InlineData((ulong) 0)]
		[InlineData(ulong.MaxValue)]
		[InlineData((ulong) 32767)]
		public void ConvertUnsignedLongValues(ulong value)
		{
			var tag = ObjectConverter.ConvertToTag("TestULong", value, Convention);
			Assert.IsType<LongTag>(tag);
			var result = ObjectConverter.ConvertFromTag<ulong>(tag, Convention);
			Assert.Equal(value, result);
		}

		[Theory]
		[InlineData(0f)]
		[InlineData(0.1f)]
		[InlineData(1f)]
		[InlineData(-1f)]
		[InlineData(float.MinValue)]
		[InlineData(float.MaxValue)]
		[InlineData(float.PositiveInfinity)]
		[InlineData(float.NegativeInfinity)]
		[InlineData(float.NaN)]
		public void ConvertFloatValues(float value)
		{
			var tag = ObjectConverter.ConvertToTag("TestFloat", value, Convention);
			Assert.IsType<SingleTag>(tag);
			var result = ObjectConverter.ConvertFromTag<float>(tag, Convention);
			Assert.Equal(value, result);
		}
		
		[Theory]
		[InlineData(0.0)]
		[InlineData(0.1)]
		[InlineData(1.0)]
		[InlineData(-1.0)]
		[InlineData(double.MinValue)]
		[InlineData(double.MaxValue)]
		[InlineData(double.PositiveInfinity)]
		[InlineData(double.NegativeInfinity)]
		[InlineData(double.NaN)]
		public void ConvertDoubleValues(double value)
		{
			var tag = ObjectConverter.ConvertToTag("TestDouble", value, Convention);
			Assert.IsType<DoubleTag>(tag);
			var result = ObjectConverter.ConvertFromTag<double>(tag, Convention);
			Assert.Equal(value, result);
		}

		public static IEnumerable<object[]> DecimalValues() 
			=> new[] {0m, 1m, 0.1m, -0.1m, -1m, decimal.MinValue, decimal.MaxValue}
				.Select(x => new object[] {x});

		[Theory]
		[MemberData(nameof(DecimalValues))]
		public void ConvertDecimalValues(decimal value)
		{
			var tag = ObjectConverter.ConvertToTag("TestDecimal", value, Convention);
			Assert.IsType<IntArrayTag>(tag);
			var result = ObjectConverter.ConvertFromTag<decimal>(tag, Convention);
			Assert.Equal(value, result);
		}

		public static IEnumerable<object[]> DateTimeValues()
			=> new[] {DateTime.Today, DateTime.Now, DateTime.MinValue, DateTime.MaxValue, new DateTime(1981, 11, 25)}
				.Select(x => new object[] {x});

		[Theory]
		[MemberData(nameof(DateTimeValues))]
		public void ConvertDateTimeValues(DateTime value)
		{
			var tag = ObjectConverter.ConvertToTag("TestDateTime", value, Convention);
			Assert.IsType<LongTag>(tag);
			var result = ObjectConverter.ConvertFromTag<DateTime>(tag, Convention);
			Assert.Equal(value, result);
		}

		public static IEnumerable<object[]> GuidValues()
			=> new[] {Guid.Empty, Guid.NewGuid(), Guid.NewGuid()}
				.Select(x => new object[] {x});

		[Theory]
		[MemberData(nameof(GuidValues))]
		public void ConvertGuidValues(Guid value)
		{
			var tag = ObjectConverter.ConvertToTag("TestGuid", value, Convention);
			Assert.IsType<StringTag>(tag);
			var result = ObjectConverter.ConvertFromTag<Guid>(tag, Convention);
			Assert.Equal(value, result);
		}

		public static IEnumerable<object[]> ByteArrayValues()
			=> new[] {Encoding.ASCII.GetBytes("Hello World"), new byte[] {0xb0, 0x15, 0xc0, 0x01}}
				.Select(x => new object[] {x});

		[Theory]
		[MemberData(nameof(ByteArrayValues))]
		public void ConvertByteArrayValues(byte[] value)
		{
			var tag = ObjectConverter.ConvertToTag("TestByteArray", value, Convention);
			Assert.IsType<ByteArrayTag>(tag);
			var result = ObjectConverter.ConvertFromTag<byte[]>(tag, Convention);
			Assert.True(value.SequenceEqual(result));
		}

		public static IEnumerable<object[]> IntArrayValues()
			=> new[] {new int[] {5, 10, 15, 20}, new int[] {-5, -10, -15, -20}}
				.Select(x => new object[] {x});
		
		[Theory]
		[MemberData(nameof(IntArrayValues))]
		public void ConvertIntArrayValues(int[] value)
		{
			var tag = ObjectConverter.ConvertToTag("TestIntArray", value, Convention);
			Assert.IsType<IntArrayTag>(tag);
			var result = ObjectConverter.ConvertFromTag<int[]>(tag, Convention);
			Assert.True(value.SequenceEqual(result));
		}

		public static IEnumerable<object[]> LongArrayValues()
			=> new[] {new long[] {5, 10, 15, 20}, new long[] {-5, -10, -15, -20}}
				.Select(x => new object[] {x});
		
		[Theory]
		[MemberData(nameof(LongArrayValues))]
		public void ConvertLongArrayValues(long[] value)
		{
			var tag = ObjectConverter.ConvertToTag("TestLongArray", value, Convention);
			Assert.IsType<LongArrayTag>(tag);
			var result = ObjectConverter.ConvertFromTag<long[]>(tag, Convention);
			Assert.True(value.SequenceEqual(result));
		}


	}
}
