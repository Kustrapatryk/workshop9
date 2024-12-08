using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Task1_ObjectCreator;
//public static class TypeCrafter
//{
//        public static T TypeCraft<T>() where T : new()
//        {
//            T instance = new T();

//            PropertyInfo[] properties = typeof(T).GetProperties();
//            foreach (var property in properties)
//            {
//                Console.WriteLine($"Enter value for {property.Name} ({property.PropertyType.Name}):");
//                string input = Console.ReadLine();

//                // Obsługa prostych typów danych
//                if (TrySetPropertyValue(property, instance, input))
//                {
//                    continue;
//                }

//                Console.WriteLine($"Failed to set property {property.Name}. Skipping...");
//            }

//            return instance;
//        }

//        private static bool TrySetPropertyValue<T>(PropertyInfo property, T instance, string value)
//        {
//            var propertyType = property.PropertyType;
      
//            if (propertyType == typeof(string))
//            {
//                property.SetValue(instance, value);
//                return true;
//            }

//            if (propertyType == typeof(int))
//            {
//                if (int.TryParse(value, out int intValue))
//                {
//                    property.SetValue(instance, intValue);
//                    return true;
//                }
//                else
//                {
//                throw new ParseException($"Invalid value '{value}' for property {property.Name}. Expected an integer.");
//                }
//        }


//            throw new ParseException($"Property {property.Name} of type {propertyType.Name} is not supported.");
//    }
//}


public static class TypeCrafter
{
    public static T TypeCraft<T>()
    {
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();
        ConstructorInfo? constructor = type.GetConstructor(Type.EmptyTypes);

        if(constructor == null)
        {
            throw new InvalidOperationException($"Type {type.FullName} has no parameterless constructor");
        }

        var obj = (T)constructor.Invoke(null);
        foreach( var property in properties)
        {
            Type propertyType = property.PropertyType;
            Type parsableType = typeof(IParsable<>);
            var parsable = propertyType.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == parsableType &&
                t.GetGenericArguments()[0] == propertyType);

            if (propertyType == typeof(string))
            {
                string input = AskForInput(property.Name, propertyType.Name);
                property.SetValue(obj, input, null);

            }
            else if (parsable)
            {
                   
                string input = AskForInput(property.Name, propertyType.Name);
                MethodInfo? parseMethod = propertyType.GetMethod("TryParse",
                    BindingFlags.Public | BindingFlags.Static,
                    types: [typeof(string), typeof(IFormatProvider), propertyType.MakeByRefType()]
                    );
                if (parseMethod == null)
                {
                    throw new Exception();
                }


                object?[] args = { input, null, null };
                bool status = (bool)parseMethod.Invoke(obj, args);
                if (status)
                {
                    property.SetValue(obj, args[2]);
                }
                else
                {
                    throw new ParseException($"Couldn't parse '{input}' to {propertyType}.");
                }
            }
            else
            {
                Console.WriteLine($"Type of property '{property.PropertyType} {property.Name}' is not parsable. Attempting to ");
                var craftMethod = typeof(TypeCrafter).GetMethod(nameof(TypeCraft), BindingFlags.Public | BindingFlags.Static);
                var genericMethod = craftMethod?.MakeGenericMethod(propertyType);
                var complexProperty = genericMethod?.Invoke(null, null);
                property.SetValue(obj, complexProperty);
            }

        }
        return obj;

    }

    private static string AskForInput(string propertyName, string type)
    {
        Console.WriteLine($"Providae a variable {propertyName} of type {type}:");
        if(Console.ReadLine() is { } line)
        {
            return line;
        }
   
        return null;
    }
}