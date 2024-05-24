using Microsoft.Playwright;

namespace GainBridgeCaculator.Pages
{
    public class GrowthCalculatorPage : TestBase
    {
        private readonly IPage _page;
        private readonly string _url = "";

        #region component
        private readonly ILocator _sliderTracker;
        public ILocator SliderTracker()
        {
            return _page.GetByRole(AriaRole.Slider);
        }
        #endregion

        public GrowthCalculatorPage(IPage page)
        {
            _page = page;
            _page.WaitForURLAsync(_url);
            _sliderTracker = SliderTracker();
        }

        #region actions
        //Go to learn more page by link
        public async Task GotoLearnMoreAsync()
        {
            //find link, click
            await _page.GotoAsync("");
        }
        #endregion

        #region component actions
        //get investment amount range from UI and return min, and max value
        public async Task<KeyValuePair<int, int>> GetInvestmentAmountRangeAsync()
        {
            //ToDo
            return new KeyValuePair<int, int>();
        }

        //Fill in investment amount
        public async Task FillinInvestmentAmountAsync(string amount)
        {
            //ToDo
        }

        //Read the value at the investment amount textfield
        public async Task<string> ReadInvestmentAmountAsync()
        {
            //ToDo
            return "";
        }

        //get investment amount range from UI and return min and max value pair
        public async Task<KeyValuePair<int, int>> GetDurationRangeAsync()
        {
            try
            {
                var minDuration = _sliderTracker.GetAttributeAsync("aria-valuemin");
                var maxDuration = _sliderTracker.GetAttributeAsync("aria-valuemax");
                return new KeyValuePair<int, int>(Convert.ToInt32(minDuration), Convert.ToInt32(maxDuration));
            }
            catch(Exception e)
            {
                Assert.Fail($"Cannot get duration range successfully, {e}");
            }
            return new KeyValuePair<int, int>();
        }

        //if use default, then targetYears == -1
        public async Task UpdateInvestmentDuration(int targetYears = -1)
        {
            try
            {
                if (targetYears == -1)
                {
                    return;
                }

                //From the screenshot, I cannot see the sliderbox element,(the given span's parent).
                //So just put an empty locator to refer to the box
                var sliderBox = await _page.Locator("").BoundingBoxAsync();
                if(sliderBox == null)
                {
                    throw new Exception("Cannot locate slider box");
                }

                var years = await GetDurationRangeAsync();
                var offSetYears = years.Key;

                var finalX = sliderBox.Width / (years.Value - offSetYears) * (targetYears - offSetYears) + sliderBox.X;

                await _sliderTracker.HoverAsync();
                await _page.Mouse.DownAsync();
                await _page.Mouse.MoveAsync(finalX, sliderBox.Y);
                await _page.Mouse.UpAsync();

            }
            catch (Exception)
            {
                Assert.Fail("Cannot update the investment duration");
            }
            
        }

        public async Task ClickPurchaseButton()
        {
            //ToDo
        }
        #endregion

        #region component verification
        //Verify if the text field is not valid, error message displays with correct text
        public async Task<bool> VerifyInvestmentAmountError(string errorMesssage)
        {
            //ToDo
            return false;
        }

        //interest rate (APY) verify percentage (with database)
        public async Task<bool> VerifyInterestRate()
        {
            //verify percentage
            //verify with correct number (API or database)
            return false;
        }

        //investment duration verify tips
        public async Task<bool> VerifyInvestmentDuration(int targetYears)
        {
            try
            {
                await Expect(_sliderTracker.Locator("div")).ToHaveTextAsync($"{targetYears} years");
            }catch(Exception)
            {
                return false;
            }

            //In non-smoke test category, if possible, verify the value with database or API after save
            return true;
        }

        //projected account value verify > invest amount
        //projected account value verify if there's a formula or API call 
        public async Task<bool> VerifyProjectedValue()
        {
            //ToDo
            return false;
        }

        #endregion
    }
}