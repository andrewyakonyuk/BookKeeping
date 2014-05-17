using System;

namespace BookKeeping.Infrastructure
{
    public interface IAbstractFactory
    {
        T Create<T>();

        bool CanCreate<T>();
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
            throw new NotSupportedException(string.Format("The type {0} is not supported", typeof(T).FullName));
        }

        public bool CanCreate<T>()
        {
            return this is IFactoryMethod<T>;
        }
    }
}
