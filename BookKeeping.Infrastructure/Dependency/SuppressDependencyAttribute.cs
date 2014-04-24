using System;

namespace BookKeeping.Infrastructure.Dependency
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class SuppressDependencyAttribute : Attribute
    {
        public string FullyQualifiedName { get; private set; }

        public string Assembly { get; private set; }

        public SuppressDependencyAttribute(string fullyQualifiedName, string assembly)
        {
            this.FullyQualifiedName = fullyQualifiedName.Trim();
            this.Assembly = assembly.Replace(".dll", "").Trim();
        }
    }
}
