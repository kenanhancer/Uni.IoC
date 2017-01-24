using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uni.IoC
{
    public class InnerInterceptor : IInterceptor
    {
        Action<Uni.IoC.IInvocation> interceptor;

        public InnerInterceptor(Action<Uni.IoC.IInvocation> interceptor)
        {
            this.interceptor = interceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            if (interceptor != null)
                interceptor(invocation);
            else
                invocation.Proceed();
        }
    }
}