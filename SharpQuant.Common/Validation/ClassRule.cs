using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.Validation
{
    public class ClassRule<T>
    {
        public Predicate<T> Rule { get; set; }
        public string Message { get; set; }

        public ClassRule(Predicate<T> rule, string message)
        {
            Rule = rule; Message = message;
        }

        public string GetMessage(T entity)
        {
            return !Rule(entity) ? string.Empty : Message;
        }
    }
}
