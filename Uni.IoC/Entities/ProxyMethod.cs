using System;
using System.Reflection;

namespace Uni.IoC
{
    public class ProxyMethod
    {
        public MethodInfo Method { get; set; }
        public Func<object[], object> MethodInvokerDelegate { get; set; }
    }
}