using FluentValidation;

namespace Api.Framework
{
    public interface IHandler
    {
        public Task<object> RunAsync(object v);
    }

    public abstract class Handler<TIn, TOut> : IHandler
        where TIn : IRequest<TOut>
    {
        public abstract Task<TOut> Run(TIn v);

        public async Task<object> RunAsync(object v)
        {
            var result = await Run((TIn)v);
            return result;
        }
    }
}
