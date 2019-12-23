using System;
using System.Linq;

namespace SimpleNbt.Internal
{
	internal static class TypeExtensions
	{
		public static bool Implements(this Type type, Type interfaceType)
		{
			if (!interfaceType.IsInterface) throw new ArgumentException("Interface type must be the type of an interface.");
			if (type == interfaceType) return true;
			return type.GetInterfaces().Any(x => x.Implements(interfaceType));
		}
		
		public static bool ImplementsGeneric(this Type type, Type genericInterfaceType, params Type[] genericArgs)
		{
			if (!genericInterfaceType.IsInterface) throw new ArgumentException("Interface type must be the type of an interface.");
            
			if (type.IsGenericType && type.GetGenericTypeDefinition() == genericInterfaceType)
			{
				return genericArgs is null || genericArgs.Length == 0 || type.GetGenericArguments().SequenceEqual(genericArgs);
			}

			return type.GetInterfaces().Any(x => x.ImplementsGeneric(genericInterfaceType, genericArgs));
		}

		public static Type GetGenericImplementationType(this Type type, Type genericInterfaceType, int argIndex = 0)
		{
			if (type is null) return null;
            
			if (!genericInterfaceType.IsInterface) throw new ArgumentException("Interface type must be the type of an interface.");

			if (type.IsGenericType && type.GetGenericTypeDefinition() == genericInterfaceType)
			{
				var args = type.GetGenericArguments();
				if (argIndex < 0 || args.Length < argIndex) throw new IndexOutOfRangeException();
				return args[argIndex];
			}

			return GetGenericImplementationType(
				type.GetInterfaces().FirstOrDefault(x => x.ImplementsGeneric(genericInterfaceType)),
				genericInterfaceType, argIndex);
		}
	}
}
