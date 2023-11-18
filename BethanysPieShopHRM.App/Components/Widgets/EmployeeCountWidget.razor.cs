using BethanysPieShopHRM.App.Models;
using BethanysPieShopHRM.App.Services;
using Microsoft.AspNetCore.Components;

namespace BethanysPieShopHRM.App.Components.Widgets
{
    public partial class EmployeeCountWidget
    {
        public int EmployeeCounter { get; set; }

        [Inject]
        IEmployeeDataService? EmployeeDataService { get; set; }
        //protected override void OnInitialized()
        //{
        //    EmployeeCounter = MockDataService.Employees.Count;
        //}
        protected override async Task OnInitializedAsync()
        {
            EmployeeCounter = (await  EmployeeDataService.GetAllEmployees()).ToList().Count;
        }
    }
}
