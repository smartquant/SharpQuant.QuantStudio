using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;


namespace SharpQuant.Common.Validation
{


    /*

     * Simple validator using the following strategies:
     * - Attribute based from System.ComponentModel.DataAnnotations for properties
     * - Rules for properties (one per property, but easily extensible)
     * - Rules for entire class
     * 
     * Tests performed on property level and class level.
     * 
     * 
     * 
    */


    public class ValidatorBase<T> : IValidator<T> where T : class
    {
        private Dictionary<string, PropertyRule<T>> _propertyRules;
        private List<ClassRule<T>> _classRules;
        private Dictionary<string, Func<T,object>> _accessors;

        protected Dictionary<string, Func<T, object>> Accessors { get { return _accessors ?? (_accessors = new Dictionary<string, Func<T, object>>()); } }
        protected Dictionary<string, PropertyRule<T>> PropertyRules { get { return _propertyRules ?? (_propertyRules = new Dictionary<string, PropertyRule<T>>()); } }
        protected List<ClassRule<T>> ClassRules { get { return _classRules ?? (_classRules = new List<ClassRule<T>>()); } }

        protected bool UseAttributes { get; set; }

        public ValidatorBase()
        {
            UseAttributes = true;
        }


        /// <summary>
        /// Some reflection and lambda wizardry to make this type safe...
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="property"></param>
        /// <param name="rule"></param>
        /// <param name="message"></param>
        protected void AddPropertyRule<TProperty>(Expression<Func<T, TProperty>> property, Func<T, TProperty, bool> rule, string message)
        {
            MemberExpression memberExp = property.Body as MemberExpression;
            var info = memberExp.Member;
            var pd = TypeDescriptor.CreateProperty(typeof(T), info.Name, typeof(TProperty));
            
            Func<T, object, bool> func = (entity, o) => rule(entity, (TProperty)pd.GetValue(entity));

            Accessors.Add(info.Name, e => pd.GetValue(e));
            PropertyRules.Add(info.Name, new PropertyRule<T>(func, message));
        }


        protected void AddClassRule(Predicate<T> rule, string message)
        {
            ClassRules.Add(new ClassRule<T>(rule, message));
        }

        //property level
        public bool IsValid(T entity, string fieldname)
        {
            bool isValid = true;

            if (UseAttributes)
            {
                var validationResults = new List<ValidationResult>();
                var vc = new ValidationContext(entity, null, null);
                vc.MemberName = fieldname;
                isValid = Validator.TryValidateProperty(Accessors[fieldname](entity), vc, validationResults);
            }
            if (!isValid) return false;

            if (_propertyRules != null && PropertyRules.ContainsKey(fieldname))
            {
                if (PropertyRules[fieldname].Rule(entity, Accessors[fieldname](entity)))
                    return false;
            }
            return isValid;
        }

        public IList<string> GetErrorMessages(T entity, string fieldname)
        {
            List<string> list = new List<string>();

            if (UseAttributes)
            {
                var validationResults = new List<ValidationResult>();
                var vc = new ValidationContext(entity, null, null);
                vc.MemberName = fieldname;
                var isValid = Validator.TryValidateProperty(Accessors[fieldname](entity), vc, validationResults);

                if (validationResults.Count > 0)
                {
                    list.AddRange(validationResults.Select(v=>v.ErrorMessage));
                }
            }

            if (_propertyRules != null && PropertyRules.ContainsKey(fieldname))
            {

                string msg = PropertyRules[fieldname].GetMessage(entity, Accessors[fieldname](entity));
                if (!string.IsNullOrEmpty(msg))
                {
                    list.Add(msg);
                }
            }

            return list;
        }


        //entity level

        public bool IsValid(T entity)
        {
            bool isvalid = true;

            if (UseAttributes)
            {
                var validationResults = new List<ValidationResult>();
                var vc = new ValidationContext(entity, null, null);
                var isValid = Validator.TryValidateObject(entity, vc, validationResults);
                if (!isValid) return false;
            }

            if (_propertyRules != null)
            {
                foreach (var rule in _propertyRules)
                {
                    if (rule.Value.Rule(entity, Accessors[rule.Key](entity)))
                        return false;
                }

            }

            if (_classRules != null)
            {
                foreach (var rule in _classRules)
                {
                    if (rule.Rule(entity)) return false;
                }
            }

            return isvalid;
        }

        public IList<string> GetErrorMessages(T entity)
        {
            List<string> list = new List<string>();

            if (UseAttributes)
            {
                var validationResults = new List<ValidationResult>();
                var vc = new ValidationContext(entity, null, null);
                var isValid = Validator.TryValidateObject(entity, vc, validationResults, true);

                list.AddRange(validationResults.Select(v => v.ErrorMessage));
            }

            if (_propertyRules != null)
            {

                foreach (var rule in _propertyRules)
                {
                    string msg = rule.Value.GetMessage(entity, Accessors[rule.Key](entity));
                    if (!string.IsNullOrEmpty(msg)) list.Add(msg);
                }

            }

            if (_classRules != null)
            {
                foreach (var rule in _classRules)
                {
                    string msg = rule.GetMessage(entity);
                    if (!string.IsNullOrEmpty(msg)) list.Add(msg);
                }
            }

            return list;
        }

    }




}

