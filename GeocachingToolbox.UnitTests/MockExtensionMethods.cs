using KellermanSoftware.CompareNetObjects;
using Machine.Specifications;
using Machine.Specifications.Annotations;
using Rhino.Mocks.Interfaces;
using System;
using System.IO;
using System.Reflection;

namespace GeocachingToolbox.UnitTests
{
    static class MspecExtensionMethods
    {
        public static IMethodOptions<string> ReturnContentOf(this IMethodOptions<string> subject, string filePath)
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

            return subject.Return(content);
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
            var result = (new CompareLogic()).Compare(subject, compareWith);
            result.AreEqual.ShouldBeTrue(result.DifferencesString);
        }
    }
}
