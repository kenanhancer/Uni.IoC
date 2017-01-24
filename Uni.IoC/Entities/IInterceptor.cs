namespace Uni.IoC
{
    public interface IInterceptor
    {
        void Intercept(IInvocation invocation);
    }
}