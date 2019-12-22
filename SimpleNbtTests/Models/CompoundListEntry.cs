using System;
using SimpleNbt.Attributes;

namespace SimpleNbtTests.Models
{
	public class CompoundListEntry
	{
		public string Name { get; set; }
		
		[NamedBinaryProperty("created-on")]
		public DateTime CreatedOn { get; set; }
	}
}