using System;
using System.Collections.Generic;

namespace Microsoft.Bot.Framework.Builder.Witai
{
    public interface IWitLocator
    {
        T Resolve<T>() where T : class;
        void Register<T>(T service) where T : class;
    }

    public sealed class WitLocator : IWitLocator
    {
        private static readonly Lazy<WitLocator> instance = new Lazy<WitLocator>(() => new WitLocator());
        private static readonly IDictionary<Type, object> InstantiatedServices = new Dictionary<Type, object>();

        private static readonly object Locker = new object();

        private WitLocator() { }

        public static WitLocator Instance => instance.Value;

        public T Resolve<T>() where T : class
        {
            lock (Locker)
            {
                if (!InstantiatedServices.ContainsKey(typeof(T)))
                    throw new Exception("The requested service is not registered");
                
                return (T)InstantiatedServices[typeof(T)];
            }
        }

        public void Register<T>(T service) where T : class
        {
            lock (Locker)
            {
                if (!InstantiatedServices.ContainsKey(typeof(T)))
                {
                    lock (Locker)
                    {
                        InstantiatedServices.Add(typeof(T), service);
                    }
                }
            }
        }
    }
}
