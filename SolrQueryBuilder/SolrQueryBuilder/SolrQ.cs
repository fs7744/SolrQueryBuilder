using System;
using System.Linq.Expressions;
using System.Text;

namespace SolrQueryBuilder
{
    public class SolrQ<T>
    {
        private SolrQOperation m_Operation = new SolrQOperation();
        private StringBuilder m_SB = new StringBuilder();

        #region ctor

        public SolrQ(Expression<Func<T, object>> expr, Func<SolrQOperation, Func<string, string>> opFunc)
        {
            AddCondition(Utils.GetPropertyName(expr), opFunc);
        }

        public SolrQ(string field, Func<SolrQOperation, Func<string, string>> opFunc)
        {
            AddCondition(field, opFunc);
        }

        public SolrQ(string query)
        {
            m_SB.Append(query);
        }

        internal SolrQ()
        {
            m_SB.Append("*:*");
        }

        #endregion ctor

        private void AddCondition(string field, Func<SolrQOperation, Func<string, string>> opFunc)
        {
            m_SB.Append(opFunc(m_Operation)(field));
        }

        public SolrQ<T> And(Expression<Func<T, object>> expr, Func<SolrQOperation, Func<string, string>> opFunc)
        {
            return And(Utils.GetPropertyName(expr), opFunc);
        }

        public SolrQ<T> And(string field, Func<SolrQOperation, Func<string, string>> opFunc)
        {
            m_SB.Append(" AND ");
            AddCondition(field, opFunc);
            return this;
        }

        public SolrQ<T> AndSub(string query)
        {
            m_SB.AppendFormat(" AND ( {0} )", query);
            return this;
        }

        public SolrQ<T> AndSub(SolrQ<T> query)
        {
            AndSub(query.ToString());
            return this;
        }

        public SolrQ<T> OR(Expression<Func<T, object>> expr, Func<SolrQOperation, Func<string, string>> opFunc)
        {
            return OR(Utils.GetPropertyName(expr), opFunc);
        }

        public SolrQ<T> OR(string field, Func<SolrQOperation, Func<string, string>> opFunc)
        {
            m_SB.Append(" OR ");
            AddCondition(field, opFunc);
            return this;
        }

        public SolrQ<T> ORSub(string query)
        {
            m_SB.AppendFormat(" OR ( {0} )", query);
            return this;
        }

        public SolrQ<T> ORSub(SolrQ<T> query)
        {
            ORSub(query.ToString());
            return this;
        }

        public SolrQ<T> SelfNot()
        {
            m_SB.Insert(0, "!( ");
            m_SB.Append(" )");
            return this;
        }

        public override string ToString()
        {
            return m_SB.ToString();
        }

        public SolrQ<T> Copy()
        {
            return new SolrQ<T>(ToString());
        }
    }
}