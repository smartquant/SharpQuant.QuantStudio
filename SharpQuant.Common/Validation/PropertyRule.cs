using System;


namespace SharpQuant.Common.Validation
{
    public class PropertyRule<T>
    {
        public Func<T, object, bool> Rule { get; set; }
        public string Message { get; set; }

        public PropertyRule(Func<T, object, bool> rule, string message)
        {
            Rule = rule; Message = message;
        }

        public string GetMessage(T entity, object fieldValue)
        {
            return !Rule(entity,fieldValue) ? string.Empty : Message;
        }
    }

}
