using System;

namespace SimpleNbt.Internal
{
	internal interface IPropOrFieldHelper
	{
		/// <summary>
		/// Gets the name of the property or field.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the explicit name for the property or field, if one is set.
		/// </summary>
		string ExplicitName { get; }
		
		/// <summary>
		/// Gets the data type for the property or field.
		/// </summary>
		Type DataType { get; }
		
		/// <summary>
		/// Gets the value of this property or field from the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		object GetValue(object entity);
		
		/// <summary>
		/// Sets the value of this property or field to the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="value"></param>
		void SetValue(object entity, object value);

	}
}
