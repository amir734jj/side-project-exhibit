using System;
using System.Threading.Tasks;

namespace Models
{
    public class DisposableResult<T> : IAsyncDisposable
    {
        private readonly Func<ValueTask> _action;
        public T Result { get; }

        public DisposableResult(T result, Func<ValueTask> action)
        {
            _action = action;
            Result = result;
        }

        public ValueTask DisposeAsync()
        {
            return _action();
        }
    }
}