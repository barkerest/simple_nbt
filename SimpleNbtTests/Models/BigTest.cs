using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SimpleNbtTests.Models
{
	public class BigTest
	{
		public BigTestContents Level { get; set; }
		
		/*
		public static readonly BigTest ValidResult = new BigTest()
		{
			Level = new BigTestContents()
			{
				NestedCompoundTest = new NestedCompoundTest()
				{
					Egg = new NameValuePair(){ Name = "Eggbert", Value = 0.5f},
					Ham = new NameValuePair(){ Name = "Hampus", Value = 0.75f}
				},
				IntTest = 2147483647,
				ByteTest = 127,
				ShortTest = 32767,
				StringTest = "HELLO WORLD THIS IS A TEST STRING ÅÄÖ!",
				DoubleTest = 0.49312871321823148,
				FloatTest = 0.49823147058486938f,
				LongTest = 9223372036854775807L,
				LongListTest = {11,12,13,14,15},
				CompoundListTest =
				{
					new CompoundListEntry(){ Name = "Compound tag #0", CreatedOn = new DateTime(1264099775885L)},
					new CompoundListEntry(){ Name = "Compound tag #1", CreatedOn = new DateTime(1264099775885L)},
				},
				ByteArrayTest = Enumerable.Range(0, 1000).Select(n => (byte)(((n * n * 255) + (n * 7)) % 100)).ToArray()
			}
		};
		*/
		
	}
}