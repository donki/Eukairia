using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using EukairiaWeb.Services;

namespace EukairiaWeb.Components.Pages
{
    [IgnoreAntiforgeryToken]
    public partial class Home : ComponentBase
    {
        [Inject]
        protected NavigationManager? NavigationManager { get; set; }
        [Inject]
        protected GlobalService? GlobalService { get; set; }



        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var result = await SessionStorage.GetAsync<Guid>("UserId");
            if (!result.Success || (!result.Value.Equals(Guid.Empty)))
            {
                UserId = result.Value;
            }
            tracks = await TimeTrackingService.GetTodaysTracksAsync(UserId);
            Summary = await TimeTrackingService.GetTimeTrackingSummary(UserId);

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