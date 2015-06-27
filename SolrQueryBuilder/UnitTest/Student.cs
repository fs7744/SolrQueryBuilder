using System;

namespace UnitTest
{
    public class Student
    {
        [SolrField("A")]
        public string Name { get; set; }

        [SolrField("B")]
        public int Age { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SolrFieldAttribute : Attribute
    {
        public SolrFieldAttribute()
        {
        }

        public SolrFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public float Boost { get; set; }

        public string FieldName { get; set; }
    }
}