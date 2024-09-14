using Microsoft.Extensions.DependencyInjection;

namespace IMS.Infrastructure.Extensions
{
    public static class ServiceLocator
    {
        private static IServiceProvider _serviceProvider;

        public static void SetServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static T GetService<T>()
            where T : class
        {
            return _serviceProvider.GetService<T>();
        }
    }
}
