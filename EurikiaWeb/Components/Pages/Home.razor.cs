using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using EukairiaWeb.Services;
using EukairiaWeb.Data.Models;

namespace EukairiaWeb.Components.Pages
{
    [IgnoreAntiforgeryToken]
    public partial class Home : ComponentBase
    {
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }


        private User User { get; set; }



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var result = await SessionStorage.GetAsync<Guid>("UserId");
            if (!result.Success || (!result.Value.Equals(Guid.Empty)))
            {
                UserId = result.Value;
                User = UsersService.GetUserById(UserId);
            }
            tracks = await TimeTrackingService.GetTodaysTracksAsync(UserId);
            TimeTrackingSummary = await TimeTrackingService.GetTimeTrackingSummary(UserId);
            LeaveRequestSummary = await LeaveRequestService.CalculateLeaveRequestSummaryThisYearAsync(UserId);

            try
            {
                var result2 = await SessionStorage.GetAsync<bool>("isAuthenticated");
                if (!result2.Success || !result2.Value)
                {
                    NavigationManager.NavigateTo("/");

                }
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error al obtener isAuthenticated: {ex.Message}");
                // Considera manejar el error de manera adecuada aquí
            }

            StateHasChanged();
        }

    }
}