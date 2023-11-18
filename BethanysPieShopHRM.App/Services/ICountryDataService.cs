using BethanysPieShopHRM.Shared.Domain;
using System.Diagnostics.Metrics;

namespace BethanysPieShopHRM.App.Services
{
    public interface ICountryDataService
    {
        Task<IEnumerable<Country>> GetAllCountries();
        Task<Country> GetCountryById(int id);
    }
}
