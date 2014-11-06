using System;
namespace SharpQuant.Common.Validation
{
    public interface IValidationProvider
    {
        /// <summary>
        /// Can return null if no validator is registered
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IValidator<T> GetValidator<T>() where T : class;
    }
}
