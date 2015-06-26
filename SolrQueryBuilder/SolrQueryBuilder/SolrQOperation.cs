using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrQueryBuilder
{
    public class SolrQOperation
    {
        private static List<string> m_SpecialSign = new List<string>()
        {
            "\\","\""," ","*",":","?","%","~","^","-","+","(",")","[","]","{","}","!","/","&&","||"
        };

        #region private method

        private string GetNullOrValue(string value)
        {
            return string.IsNullOrEmpty(value) ? "\"\"" : ReplaceSpecialSign(value);
        }

        private string ReplaceSpecialSign(string value)
        {
            var result = new StringBuilder(value);
            foreach (var sign in m_SpecialSign)
            {
                result.Replace(sign, "\\" + sign);
            }

            return result.ToString();
        }

        private Func<string, string> Not(Func<string, string> func)
        {
            return query => string.Format("!{0}", func(query));
        }

        #endregion private method

        public Func<string, string> Equal(string value)
        {
            return field => string.Format("{0}:{1}", field, GetNullOrValue(value));
        }

        public Func<string, string> NotEqual(string value)
        {
            return Not(Equal(value));
        }

        public Func<string, string> Between(string from, string to)
        {
            return field => string.Format("{0}:[{1} TO {2}]", field, GetNullOrValue(from), GetNullOrValue(to));
        }

        public Func<string, string> NotBetween(string from, string to)
        {
            return Not(NotBetween(from, to));
        }

        public Func<string, string> In(params string[] param)
        {
            return field => string.Format("{0}:({1})", field, string.Join(" ", param.Select(i => GetNullOrValue(i)).ToArray()));
        }

        public Func<string, string> NotIn(params string[] param)
        {
            return Not(NotIn(param));
        }

        public Func<string, string> Less(string value)
        {
            return field => string.Format("{0}:{* TO {1}}", field, GetNullOrValue(value));
        }

        public Func<string, string> Greater(string value)
        {
            return field => field + ":{" + GetNullOrValue(value) + " TO *}";
        }

        public Func<string, string> LessOrEqual(string value)
        {
            return field => string.Format("{0}:[* TO {1}]", field, GetNullOrValue(value));
        }

        public Func<string, string> GreaterOrEqual(string value)
        {
            return field => string.Format("{0}:[{1} TO *]", field, GetNullOrValue(value));
        }

        public Func<string, string> NotNull()
        {
            return field => string.Format("{0}:['' TO *]", field);
        }

        public Func<string, string> Like(string value)
        {
            return field => string.Format("{0}:*{1}*", field, GetNullOrValue(value));
        }

        public Func<string, string> NotLike(string value)
        {
            return Not(Like(value));
        }

        public Func<string, string> LeftLike(string value)
        {
            return field => string.Format("{0}:{1}*", field, GetNullOrValue(value));
        }

        public Func<string, string> NotLeftLike(string value)
        {
            return Not(LeftLike(value));
        }

        public Func<string, string> RightLike(string value)
        {
            return field => string.Format("{0}:*{1}", field, GetNullOrValue(value));
        }

        public Func<string, string> NotRightLike(string value)
        {
            return Not(RightLike(value));
        }
    }
}