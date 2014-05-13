using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookKeeping.App.Infrastructure
{
    public interface IAbstractFactory
    {
        T Create<T>();

        bool IsProduct<T>();
    }

    public interface IFactoryMethod<T>
    {
        T Create();
    }

    public abstract class AbstractFactory : IAbstractFactory
    {
        public T Create<T>()
        {
            IFactoryMethod<T> factoryMethod = this as IFactoryMethod<T>;
            if (factoryMethod != null)
            {
                return factoryMethod.Create();
            }
            // Здесь можно вызвать Exception, так как
            // интерфейс IFactoryMethod<T> не реализован 
            return default(T);
        }

        public bool IsProduct<T>()
        {
            return this is IFactoryMethod<T>;
        }
    }

}
