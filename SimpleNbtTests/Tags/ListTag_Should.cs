using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using SimpleNbt;
using SimpleNbt.Tags;
using Xunit;
using Xunit.Abstractions;

namespace SimpleNbtTests.Tags
{
	public class ListTag_Should
	{
		#region TestData

		public interface ITestData
		{
			INamedListBinaryTag CreateList();

			INamedListBinaryTag CreateEmptyList();
			
			TagType ListType { get; }
			void TestSequence(INamedListBinaryTag decoded);
		}
		
		private class TestSimpleData<TList, TPayload> : ITestData where TList : class, INamedBinaryTag, new()
		{
			protected readonly PropertyInfo Payload;
			
			public TestSimpleData(params TPayload[] testValues)
			{
				ListType = new TList().Type;
				TestValues = testValues;
				Payload = typeof(TList).GetProperty("Payload");
			}
			
			public virtual INamedListBinaryTag CreateList()
			{
				var ret = new ListTag<TList>();
				
				foreach (var value in TestValues)
				{
					var item = new TList();
					Payload.SetValue(item, value);
					ret.Add(item);
				}

				return ret;
			}

			public INamedListBinaryTag CreateEmptyList() => new ListTag<TList>();

			public TagType ListType { get; }

			protected TPayload[] TestValues { get; }
			
			public virtual void TestSequence(INamedListBinaryTag decoded)
			{
				var list = decoded as IList<TList>;
				Assert.NotNull(list);

				Assert.Equal(TestValues.Length, list.Count);
				
				for (var i = 0; i < TestValues.Length; i++)
				{
					var value = Payload.GetValue(list[i]);
					Assert.Equal(TestValues[i], value);
				}
			}
		}

		private class TestArrayData<TList, TPayload, TValue> : TestSimpleData<TList, TPayload> where TList : class, INamedBinaryTag, new() where TPayload : class, IEnumerable<TValue>
		{
			public TestArrayData(params TPayload[] testValues) : base(testValues)
			{
			}
			
			public override void TestSequence(INamedListBinaryTag decoded)
			{
				var list = decoded as IList<TList>;
				Assert.NotNull(list);

				Assert.Equal(TestValues.Length, list.Count);
				
				for (var i = 0; i < TestValues.Length; i++)
				{
					var known = TestValues[i];
					var test = Payload.GetValue(list[i]) as TPayload;

					Assert.NotNull(test);
					
					Assert.True(known.SequenceEqual(test));
				}
			}
		}

		private class TestCompoundData : TestSimpleData<CompoundTag, CompoundTag>
		{
			private static CompoundTag FromParam((string, int) p)
			{
				var ret = new CompoundTag();
				ret["name"] = new StringTag("name"){Payload = p.Item1};
				ret["value"] = new IntTag("value"){Payload = p.Item2};
				return ret;
			}

			public TestCompoundData(params (string, int)[] testValues) : base(testValues.Select(FromParam).ToArray())
			{
				
			}

			public override INamedListBinaryTag CreateList()
			{
				var ret = new ListTag<CompoundTag>();

				foreach (var value in TestValues)
				{
					ret.Add(value);
				}
				
				return ret;
			}

			public override void TestSequence(INamedListBinaryTag decoded)
			{
				var list = decoded as IList<CompoundTag>;
				Assert.NotNull(list);

				Assert.Equal(TestValues.Length, list.Count);
				
				for (var i = 0; i < TestValues.Length; i++)
				{
					var known = TestValues[i];
					var value = list[i];
					
					Assert.NotNull(value);
					Assert.Equal(known.Count, value.Count);

					var knownName = known["name"] as StringTag;
					var knownValue = known["value"] as IntTag;
					Assert.NotNull(knownName);
					Assert.NotNull(knownValue);

					var testName = value["name"] as StringTag;
					var testValue = value["value"] as IntTag;
					Assert.NotNull(testName);
					Assert.NotNull(testValue);
					
					Assert.Equal(knownName.Payload, testName.Payload);
					Assert.Equal(knownValue.Payload, testValue.Payload);
				}
			}
		}
		
		#endregion

		private readonly ITestOutputHelper _output;
		
		public ListTag_Should(ITestOutputHelper output)
		{
			_output = output ?? throw new ArgumentNullException();
		}
		
		private static readonly ITestData[] Tests =
		{
			new TestSimpleData<ByteTag, sbyte>(1, 2, 3, 4, 5, sbyte.MinValue, sbyte.MaxValue),
			new TestSimpleData<ShortTag, short>(1, 2, 3, 4, 5, short.MinValue, short.MaxValue),
			new TestSimpleData<IntTag, int>(1, 2, 3, 4, 5, int.MinValue, int.MaxValue),
			new TestSimpleData<LongTag, long>(1, 2, 3, 4, 5, long.MinValue, long.MaxValue),
			new TestSimpleData<SingleTag, float>(0f, 1f, -1f, 0.1f, -0.1f, float.MinValue, float.MaxValue, float.NegativeInfinity, float.PositiveInfinity, float.NaN),
			new TestSimpleData<DoubleTag, double>(0, 1, -1, 0.1, -0.1, double.MinValue, double.MaxValue, double.NegativeInfinity, double.PositiveInfinity, double.NaN),
			new TestSimpleData<StringTag, string>("", "  ", "a", "A", "abc123", "⌡⌠"),
			new TestArrayData<ByteArrayTag, byte[], byte>(new byte[0], new byte[]{1,2,3}, new byte[]{byte.MinValue, byte.MaxValue}),
			new TestArrayData<IntArrayTag, int[], int>(new int[0], new int[]{1,2,3,-1,-2,-3}, new int[]{int.MinValue, int.MaxValue}),
			new TestArrayData<LongArrayTag, long[], long>(new long[0], new long[]{1,2,3,-1,-2,-3}, new long[]{long.MinValue, long.MaxValue}),
			new TestCompoundData(("alpha", 1), ("bravo", -2), ("charlie", 300000000)), 
		};

		public static IEnumerable<object[]> TheoryData => Tests.Select(x => new object[] {x});
		
		[Theory]
		[MemberData(nameof(TheoryData))]
		public void HaveTheCorrectTypes(ITestData test)
		{
			var tag = test.CreateEmptyList();
			Assert.Equal(TagType.List, tag.Type);
			Assert.Equal(test.ListType, tag.ListType);
		}

		[Theory]
		[MemberData(nameof(TheoryData))]
		public void EncodeDecodeCorrectly(ITestData test)
		{
			var strm = new MemoryStream();
			
			var tag = test.CreateList();
			test.TestSequence(tag);	// ensure the initial object is correct.

			tag.EncodePayload(strm);
			strm.Position = 0;

			tag = test.CreateEmptyList();
			tag.DecodePayload(strm);
		
			test.TestSequence(tag);	// ensure the decoded object is correct.
		}
		
	}
}