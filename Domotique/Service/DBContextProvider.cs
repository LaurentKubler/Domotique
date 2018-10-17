using Domotique.Database;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Domotique.Service
{
    public class DBContextProvider
    {
        IServiceProvider _serviceProvider;
        public DBContextProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public DomotiqueContext getContext() => _serviceProvider.GetService<DomotiqueContext>();
    }
}
