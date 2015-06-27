using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrQueryBuilder;

namespace UnitTest
{
    [TestClass]
    public class UnitTestQ
    {
        [TestMethod]
        public void TestSolrQ()
        {
            Assert.AreEqual("Name:a", SolrQ.Query<Student>(i => i.Name)(op => op.Equal("a")).ToString());
            Assert.AreEqual("Age:3", SolrQ.Query<Student>("Age")(op => op.Equal(3.ToString())).ToString());
            Assert.AreEqual("Name:C", SolrQ.QByString<Student>("Name:C").ToString());
            Assert.AreEqual("*:*", SolrQ.QueryAll<Student>().ToString());
        }

        [TestMethod]
        public void TestQuery()
        {
            var q = SolrQ.Query<Student>(i => i.Name)(op => op.Equal("a"))
                .And(i => i.Age, op => op.Equal("3"))
                .And(i => i.Name, op => op.LeftLike("g g"))
                .AndSub("Name:88 OR Name:66")
                .AndSub(new SolrQ<Student>("Age:77").OR("nAME", op => op.In("a", "b")));

            Assert.AreEqual("Name:a AND Age:3 AND Name:g\\ g* AND ( Name:88 OR Name:66 ) AND ( Age:77 OR nAME:(a b) )", q.ToString());
        }

        [TestMethod]
        public void TestGetSolrFiledNameFunc()
        {
            var func = Utils.GetSolrFiledNameFunc(typeof(Student), typeof(SolrFieldAttribute));
            Assert.AreEqual("A", func("Name"));
            Assert.AreEqual("B", func("Age"));
        }

        [TestMethod]
        public void TestGetPropertyName()
        {
            SolrQ.UpdateSolrFieldAttribute<SolrFieldAttribute>();
            Assert.AreEqual("A", SolrQ.GetPropertyName<Student>("Name"));
            Assert.AreEqual("B", SolrQ.GetPropertyName<Student>("Age"));
            Assert.AreEqual("A", SolrQ.GetPropertyName<Student>(i => i.Name));
            Assert.AreEqual("B", SolrQ.GetPropertyName<Student>(i => i.Age));

            SolrQ.ClearSolrFieldAttribute();
            Assert.AreEqual("Name", SolrQ.GetPropertyName<Student>("Name"));
            Assert.AreEqual("Age", SolrQ.GetPropertyName<Student>("Age"));
            Assert.AreEqual("Name", SolrQ.GetPropertyName<Student>(i => i.Name));
            Assert.AreEqual("Age", SolrQ.GetPropertyName<Student>(i => i.Age));
        }

        [TestMethod]
        public void TestUseSolrFieldAttribute()
        {
            SolrQ.UpdateSolrFieldAttribute<SolrFieldAttribute>();
            Assert.AreEqual("A:a AND B:3", SolrQ.Query<Student>(i => i.Name)(op => op.Equal("a"))
                .And(i => i.Age, op => op.Equal("3")).ToString());

            SolrQ.ClearSolrFieldAttribute();
            Assert.AreEqual("Name:a AND Age:3", SolrQ.Query<Student>(i => i.Name)(op => op.Equal("a"))
                .And(i => i.Age, op => op.Equal("3")).ToString());
        }
    }
}