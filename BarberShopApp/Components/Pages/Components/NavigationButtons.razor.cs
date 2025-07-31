using Microsoft.AspNetCore.Components;

namespace BarberShopApp.Components.Pages.Components
{
    public partial class NavigationButtons
    {
        [Parameter]
        public int CurrentStep { get; set; } = 1;

        [Parameter]
        public int TotalSteps { get; set; } = 4;

        [Parameter]
        public bool CanProceed { get; set; } = false;

        [Parameter]
        public bool IsLoading { get; set; } = false;

        [Parameter]
        public EventCallback OnPreviousStep { get; set; }

        [Parameter]
        public EventCallback OnNextStep { get; set; }

        [Parameter]
        public EventCallback OnFinish { get; set; }
    }
} 