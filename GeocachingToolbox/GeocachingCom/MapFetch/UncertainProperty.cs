namespace GeocachingToolbox.GeocachingCom.MapFetch
{
    public class UncertainProperty<T>
    {
        private readonly T _value;
        private readonly int _certaintyLevel;
        private const int ZOOMLEVEL_MAX = 18;

        public static implicit operator T(UncertainProperty<T> obj)
        {
            return obj._value;
        }

        public UncertainProperty(T value, int certaintyLevel = ZOOMLEVEL_MAX)
        {
            _value = value;
            _certaintyLevel = certaintyLevel;
        }

        public T getValue()
        {
            return _value;
        }

        private UncertainProperty<T> getMergedProperty(UncertainProperty<T> other)
        {
            if (other == null || other._value == null)
            {
                return this;
            }
            if (_value == null || other._certaintyLevel > _certaintyLevel)
            {
                return other;
            }

            return this;
        }


        public static UncertainProperty<T> getMergedProperty(UncertainProperty<T> property, UncertainProperty<T> otherProperty)
        {
            return property == null ? otherProperty : property.getMergedProperty(otherProperty);
        }

        public static bool equalValues(UncertainProperty<T> property, UncertainProperty<T> otherProperty)
        {
            if (property == null || otherProperty == null)
            {
                return property == null && otherProperty == null;
            }
            if (property._value == null || otherProperty._value == null)
            {
                return property._value == null && otherProperty._value == null;
            }
            return property._value.Equals(otherProperty._value);
        }


    }
}