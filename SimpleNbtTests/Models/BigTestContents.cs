using System.Collections.Generic;
using SimpleNbt.Attributes;

namespace SimpleNbtTests.Models
{
	public class BigTestContents
	{
		[NamedBinaryProperty("nested compound test")]
		public NestedCompoundTest NestedCompoundTest { get; set; }
		
		public int IntTest { get; set; }
		
		public sbyte ByteTest { get; set; }
		
		public short ShortTest { get; set; }
		
		public string StringTest { get; set; }
		
		public double DoubleTest { get; set; }
		
		public float FloatTest { get; set; }
		
		public long LongTest { get; set; }
		
		[NamedBinaryProperty("listTest (long)")]
		public List<long> LongListTest { get; set; }
		
		[NamedBinaryProperty("listTest (compound)")]
		public List<CompoundListEntry> CompoundListTest { get; set; }
		
		[NamedBinaryProperty("byteArrayTest (the first 1000 values of (n*n*255+n*7)%100, starting with n=0 (0, 62, 34, 16, 8, ...))")]
		public byte[] ByteArrayTest { get; set; }

		[NamedBinaryProperty(Ignore = true)]
		public int IgnoredField { get; set; } = 42;
	}
}