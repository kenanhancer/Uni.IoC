﻿using System;
using System.Reflection.Emit;

namespace Uni.IoC
{
    public interface IUniIoC : IDisposable
    {
        void Register(params IServiceDescription[] serviceDescriptions);

        void Register<TService, TImplementation>(Func<IServiceDescription, IServiceDescription> serviceDescriptionFunc = null) where TImplementation : class where TService : class;

        void UnRegister<TService>();

        void UnRegister(object serviceKey);

        TService Resolve<TService>(object name = null) where TService : class;

        object Resolve(object serviceKey);

        bool IsRegistered(object serviceKey);

        bool IsRegistered<TService>();
    }
}