using KellermanSoftware.CompareNetObjects;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using Rhino.Mocks.Interfaces;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using KellermanSoftware.CompareNetObjects.TypeComparers;

namespace GeocachingToolbox.UnitTests
{
    public static class MspecExtensionMethods
    {
        public static IMethodOptions<Task<HttpContent>> ReturnContentOf(this IMethodOptions<Task<HttpContent>> subject, string filePath)
        {
            HttpContent content = new ByteArrayContent(ReadContentasByteArray(filePath));
            return subject.Return(Task.FromResult(content));
        }
        public static IMethodOptions<Task<string>> ReturnContentOf(this IMethodOptions<Task<string>> subject, string filePath)
        {
            return subject.Return(Task.FromResult(ReadContent(filePath)));// ReturnContentOf(subject, filePath);
        }
        public static IMethodOptions<Task<byte[]>> ReturnContentAsByteArrayOf(this IMethodOptions<Task<byte[]>> subject, string filePath)
        {
            return subject.Return(Task.FromResult(ReadContentasByteArray(filePath)));// ReturnContentOf(subject, filePath);
        }

        public static byte[] ReadContentasByteArray(string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dottedFilePath = filePath.Replace('\\', '.');
            var resourceName = "GeocachingToolbox.UnitTests." + dottedFilePath;
            byte[] content;

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                //using (StreamReader reader = new StreamReader(stream))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        content = memoryStream.ToArray();
                    }
                }

            }
            catch (ArgumentNullException)
            {
                throw new FileNotFoundException("Make sure that the file '" + filePath
                                                + "' exists and has 'Build Action' set to 'Embedded Resource'.");
            }
            return content;
        }

        public static string ReadContent(string filePath)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dottedFilePath = filePath.Replace('\\', '.');
            var resourceName = "GeocachingToolbox.UnitTests." + dottedFilePath;
            string content = "";

            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    content = reader.ReadToEnd();
                }
            }
            catch (ArgumentNullException)
            {
                throw new FileNotFoundException("Make sure that the file '" + filePath
                                                + "' exists and has 'Build Action' set to 'Embedded Resource'.");
            }
            return content;
        }

        [AssertionMethod]
        public static void ShouldBeTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] this bool condition, string message)
        {
            if (!condition)
                throw new SpecificationException(message);
        }

        [AssertionMethod]
        public static void ShouldEqualRecursively([AssertionCondition(AssertionConditionType.IS_TRUE)] this object subject, object compareWith)
        {
            CompareLogic logic = new CompareLogic();
            logic.Config.CustomComparers.Add(new StringComparer(RootComparerFactory.GetRootComparer()));
            logic.Config.CustomComparers.Add(new DecimalComparer(RootComparerFactory.GetRootComparer(),0.000001M));
            var result = logic.Compare(subject, compareWith);
            result.AreEqual.ShouldBeTrue(result.DifferencesString);
        }

        public class StringComparer : BaseTypeComparer
        {
            public StringComparer(RootComparer rootComparer) : base(rootComparer)
            {
            }

            /// <summary>
            /// Returns true if both objects are a string or if one is a string and one is a a null
            /// </summary>
            /// <param name="type1">The type of the first object</param>
            /// <param name="type2">The type of the second object</param>
            /// <returns></returns>
            public override bool IsTypeMatch(Type type1, Type type2)
            {
                return (TypeHelper.IsString(type1) && TypeHelper.IsString(type2))
                       || (TypeHelper.IsString(type1) && type2 == null)
                       || (TypeHelper.IsString(type2) && type1 == null);
            }

            /// <summary>
            /// Compare two strings
            /// </summary>
            public override void CompareType(CompareParms parms)
            {
                if (parms.Config.TreatStringEmptyAndNullTheSame
                    && ((parms.Object1 == null && parms.Object2 != null && parms.Object2.ToString() == string.Empty)
                        || (parms.Object2 == null && parms.Object1 != null && parms.Object1.ToString() == string.Empty)))
                {
                    return;
                }

                if (OneOfTheStringsIsNull(parms)) return;

                string string1 = parms.Object1 as string;
                string string2 = parms.Object2 as string;

                string1 = string1.Replace("\r\n", "\n");
                string2 = string2.Replace("\r\n", "\n");

                if (string1 != string2)
                {
                    AddDifference(parms);
                }
            }

            private bool OneOfTheStringsIsNull(CompareParms parms)
            {
                if (parms.Object1 == null || parms.Object2 == null)
                {
                    AddDifference(parms);
                    return true;
                }
                return false;
            }
        }

        public class DecimalComparer : BaseTypeComparer
        {
            private decimal m_Precision;
            /// <summary>
            /// Constructor that takes a root comparer
            /// </summary>
            /// <param name="rootComparer"></param>
            public DecimalComparer(RootComparer rootComparer,decimal precision) : base(rootComparer)
            {
                m_Precision = precision;
            }

            /// <summary>
            /// Returns true if both types are double
            /// </summary>
            /// <param name="type1"></param>
            /// <param name="type2"></param>
            /// <returns></returns>
            public override bool IsTypeMatch(Type type1, Type type2)
            {
                return type1 == typeof(decimal) && type2 == typeof(decimal);
            }

            /// <summary>
            /// Compare two doubles
            /// </summary>
            /// <param name="parms"></param>
            public override void CompareType(CompareParms parms)
            {
                //This should never happen, null check happens one level up
                if (parms.Object1 == null || parms.Object2 == null)
                    return;

                decimal double1 = (decimal)parms.Object1;
                decimal double2 = (decimal)parms.Object2;

                decimal difference = Math.Abs(double1 * m_Precision);

                if (Math.Abs(double1 - double2) > difference)
                    AddDifference(parms);
            }
        }


    }
}
