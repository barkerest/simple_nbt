using System;

namespace SimpleNbt.Attributes
{
	/// <summary>
	/// Default implementation of the named binary property attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class NamedBinaryPropertyAttribute : Attribute, INamedBinaryPropertyAttribute
	{
		private Type _tagType = null;

		public NamedBinaryPropertyAttribute(string name = null)
		{
			Name = name;
		}

		/// <inheritdoc />
		public string Name { get; set; }

		/// <inheritdoc />
		public bool Ignore { get; set; } = false;

		/// <inheritdoc />
		public Type TagType
		{
			get => _tagType;
			set
			{
				if (value is null)
				{
					_tagType = null;
				}
				else
				{
					var ti = typeof(INamedBinaryTag);
					
					if (!ti.IsAssignableFrom(value))
					{
						throw new InvalidCastException();
					}

					_tagType = value;
				}
			} 
		}
		
		
	}
}