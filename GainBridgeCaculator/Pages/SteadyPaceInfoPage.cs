using System;
using Microsoft.Playwright;

namespace GainBridgeCaculator.Pages
{
	public class SteadyPaceInfoPage
	{
        private readonly IPage _page;
        private readonly string _url = "";
        public readonly ILocator Title;

        public SteadyPaceInfoPage(IPage page)
        {
            _page = page;
            Title = page.Locator("");
            page.WaitForURLAsync(_url);
        }
    }
}

