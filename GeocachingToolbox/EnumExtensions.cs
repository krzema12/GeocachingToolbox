using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace GeocachingToolbox
{
    public static class EnumExtensions
    {
        //TODO : Revoir l'extraction des attribut car la classe dessous fait quasiment la même chose
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            FieldInfo fi = value.GetType().GetRuntimeField(value.ToString());

            T[] attributes =
                (T[])fi.GetCustomAttributes(
                typeof(T),
                false);


            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0];
            return null;
        }
    }

    public static class EnumExtensionMethod
    {
        public static T GetEnumFromDescriptionAttribute<T>(this T EnumWithDescriptions, string Description) where T : struct
        {
            Array enumValArray = GetValues<T>().ToArray();

            foreach (int val in enumValArray)
            {
                Enum o = (Enum)Enum.Parse(EnumWithDescriptions.GetType(), val.ToString(), true);
                string desc = GetEnumDescription(o);
                if (desc.ToLower() == Description.ToLower())
                    return (T)Convert.ChangeType(o, typeof(T), CultureInfo.InvariantCulture);
            }
            throw new KeyNotFoundException("No description attribute matching " + Description + " in enum " + EnumWithDescriptions.GetType());
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetRuntimeField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static IEnumerable<T> GetValues<T>()
        {
            return GetValues_impl<T>();
        }

        private static IEnumerable<T> GetValues_impl<T>()
        {
            return from field in typeof(T).GetRuntimeFields()
                   where field.IsLiteral && !string.IsNullOrEmpty(field.Name)
                   select (T)field.GetValue(null);
        }

    }
}
