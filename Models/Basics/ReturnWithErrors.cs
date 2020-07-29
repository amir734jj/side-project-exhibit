using System.Collections.Generic;

namespace Models.Basics
{
    public class ReturnWithErrors<T>
    {
        public T  ReturnValue { get; }
        
        public List<string> Errors { get; }

        public ReturnWithErrors(T returnValue)
        {
            ReturnValue = returnValue;
        }

        public ReturnWithErrors(T returnValue, List<string> errors) : this(returnValue)
        {
            Errors = errors;
        }
    }
}