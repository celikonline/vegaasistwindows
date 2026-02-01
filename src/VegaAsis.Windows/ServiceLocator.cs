using System;
using Autofac;

namespace VegaAsis.Windows
{
    /// <summary>
    /// Global service locator for accessing DI container from forms.
    /// Use sparingly - prefer constructor injection when possible.
    /// </summary>
    public static class ServiceLocator
    {
        private static ILifetimeScope _scope;

        public static void Initialize(ILifetimeScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        public static T Resolve<T>()
        {
            if (_scope == null)
            {
                throw new InvalidOperationException("ServiceLocator has not been initialized. Call Initialize() first.");
            }
            return _scope.Resolve<T>();
        }

        public static bool IsInitialized
        {
            get { return _scope != null; }
        }
    }
}
