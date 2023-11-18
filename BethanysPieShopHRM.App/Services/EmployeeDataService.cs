using BethanysPieShopHRM.App.Helper;
using BethanysPieShopHRM.Shared.Domain;
using Blazored.LocalStorage;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml;

namespace BethanysPieShopHRM.App.Services
{
    public class EmployeeDataService : IEmployeeDataService
    { 
        private readonly HttpClient? _httpClient;
        private readonly ILocalStorageService _localStorageService;

        public EmployeeDataService(ILocalStorageService localStorageService, HttpClient? httpClient)
        {
            _localStorageService = localStorageService;
            _httpClient = httpClient;
        }

        public async Task<Employee> AddEmployee(Employee employee)
        {
            var employeeJson = new StringContent(JsonSerializer.Serialize(employee),Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/employee",employeeJson);
            if(response.IsSuccessStatusCode)
            {
               return await JsonSerializer.DeserializeAsync<Employee>(await response.Content.ReadAsStreamAsync());
            }
            return null;
        }

        public async Task DeleteEmployee(int employeeId)
        {
            await _httpClient.DeleteAsync($"api/employee/{employeeId}");
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees(bool refreshRequired = false)
        {
            if(refreshRequired)
            {
                bool employeeExpirationExists = await _localStorageService.ContainKeyAsync(LocalStorageConstants.EmployeesListExpirationKey);
                if(employeeExpirationExists)
                {
                    DateTime employeeListExpiration = await _localStorageService.GetItemAsync<DateTime>(LocalStorageConstants.EmployeesListExpirationKey);
                    if(employeeListExpiration> DateTime.Now)
                    {
                        if (await _localStorageService.ContainKeyAsync(LocalStorageConstants.EmployeesListKey)) {
                           return await _localStorageService.GetItemAsync<List<Employee>>(LocalStorageConstants.EmployeesListKey);
                        }
                    }
                }
            }
            var list = await JsonSerializer.DeserializeAsync<IEnumerable<Employee>>
                (utf8Json: await _httpClient.GetStreamAsync($"api/employee"), options: new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            
            await _localStorageService.SetItemAsync<IEnumerable<Employee>>(LocalStorageConstants.EmployeesListKey, list);
            await _localStorageService.SetItemAsync<DateTime>(LocalStorageConstants.EmployeesListExpirationKey,DateTime.Now.AddMinutes(1));
            return list;
        }

        public async Task<Employee> GetEmployeeDetails(int employeeId)
        {
           return await JsonSerializer.DeserializeAsync<Employee>
                (utf8Json: await _httpClient.GetStreamAsync($"api/employee/{employeeId}"), options: new JsonSerializerOptions() { PropertyNameCaseInsensitive=true }); 
        }

        public async Task UpdateEmployee(Employee employee)
        {
            var employeeJson = new StringContent(JsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");
            await _httpClient.PutAsync("api/employee", employeeJson);
        }
    }
}
