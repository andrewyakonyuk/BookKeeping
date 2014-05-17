using System;

namespace BookKeeping.Infrastructure
{
    public interface IAbstractFactory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
        T Create<T>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
        bool CanCreate<T>();
    }

    public interface IFactoryMethod<T>
    {
        T Create();
    }

    public abstract class AbstractFactory : IAbstractFactory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
        public T Create<T>()
        {
            IFactoryMethod<T> factoryMethod = this as IFactoryMethod<T>;
            if (factoryMethod != null)
            {
                return factoryMethod.Create();
            }
            throw new NotSupportedException(string.Format("The type {0} is not supported", typeof(T).FullName));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
        public bool CanCreate<T>()
        {
            return this is IFactoryMethod<T>;
        }
    }
}
