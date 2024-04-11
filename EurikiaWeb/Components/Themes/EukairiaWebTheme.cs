using MudBlazor;

namespace EukairiaWeb.Theme


    public class EukairiaTheme : MudTheme
    {
        public EukairiaTheme()
        {
                        Palette = new PaletteLight()
            {
                Primary = "#594AE2",
                AppbarBackground = "#594AE2"
            },
            PaletteDark = new PaletteDark()
            {
                Primary = "#594AE2",
                AppbarBackground = "#594AE2"
            }
        }
    }

}