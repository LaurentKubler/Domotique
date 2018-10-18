using Domotique.Database;

namespace Domotique.Service
{
    public interface IDBContextProvider
    {
        DomotiqueContext getContext();
    }
}