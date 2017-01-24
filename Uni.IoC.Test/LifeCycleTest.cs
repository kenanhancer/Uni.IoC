using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uni.IoC.Test
{
    [TestClass]
    public class LifeCycleTest
    {
        [TestMethod]
        public void When_container_disposed_Then_factory_call_should_throw()
        {
            UniIoC container = new UniIoC();

            container.Register<IDisposableService, DisposableService>(f => f.Dependencies(new { name = "Testtt" }));

            IDisposableService factory = container.Resolve<IDisposableService>();

            container.Dispose();
        }
    }

    interface IDisposableService { }

    class DisposableService : IDisposableService, IDisposable
    {
        public bool IsDisposed;
        public readonly string Name;

        public DisposableService(string name)
        {
            Name = name;
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
