using Machine.Specifications;

namespace GeocachingToolbox.UnitTests
{
    public class EqualStrings
    {
        private static float _result;

        Because of = () =>
            _result = TextComparisonTools.NormalizedEditDistance("some string", "some string");

        It should_return_0_for_equal_strings = () =>
            _result.ShouldEqual(0.0f);
    }

    public class OneStringEmpty
    {
        private static float _result;

        Because of = () =>
            _result = TextComparisonTools.NormalizedEditDistance("some string", "");

        It should_return_two_if_one_string_empty = () =>
            _result.ShouldEqual(2.0f);
    }
}
