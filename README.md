# SolrQueryBuilder

本项目是以在c#中简化拼接solr语句麻烦，尽量让写法优雅一些为目的而实现的

## 例子 

```
var q = SolrQ.Query<Student>(i => i.Name)(op => op.Equal("a"))
                .And(i => i.Age, op => op.Equal("3"))
                .And(i => i.Name, op => op.LeftLike("g g"))
                .AndSub("Name:88 OR Name:66")
                .AndSub(new SolrQ<Student>("Age:77").OR("nAME", op => op.In("a", "b")));

Assert.AreEqual("Name:a AND Age:3 AND Name:g\\ g* AND ( Name:88 OR Name:66 ) AND ( Age:77 OR nAME:(a b) )", q.ToString());
```

如果solr的定义与c#中属性不同，请使用如下方式：
```
//使用solr定义名
SolrQ.UpdateSolrFieldAttribute<SolrFieldAttribute>();
Assert.AreEqual("A:a AND B:3", SolrQ.Query<Student>(i => i.Name)(op => op.Equal("a"))
	.And(i => i.Age, op => op.Equal("3")).ToString());

//使用c#属性名
SolrQ.ClearSolrFieldAttribute();
Assert.AreEqual("Name:a AND Age:3", SolrQ.Query<Student>(i => i.Name)(op => op.Equal("a"))
	.And(i => i.Age, op => op.Equal("3")).ToString());
```

## Nuget

https://www.nuget.org/packages/SolrQueryBuilder
