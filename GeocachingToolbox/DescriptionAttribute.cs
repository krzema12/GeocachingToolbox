using System;

namespace GeocachingToolbox
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DescriptionAttribute : System.Attribute
    {
        public string Description { get; }

        public DescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }
}
