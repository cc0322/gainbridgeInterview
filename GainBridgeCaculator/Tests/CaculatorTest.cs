using GainBridgeCaculator.Pages;

namespace GainBridgeCaculator.Tests
{
    [TestFixture]
    public class CaculatorTest : TestBase
	{
        //set,get
        //expect to boolean

        [Test, Category("Smoke"), Category("E2E")] //purely black-box, can be run on any environment
        public async Task PurchaseSteadyPaceCaculatorTest()
        {
            var page = await Browser.NewPageAsync();
            await page.GotoAsync(BaseUrl);
            
            var calculatorPage = new GrowthCalculatorPage(page);
            var investmentRange = await calculatorPage.GetInvestmentAmountRangeAsync();
            await calculatorPage.FillinInvestmentAmountAsync(new Random().Next(investmentRange.Key, investmentRange.Value).ToString());
            await calculatorPage.VerifyInterestRate();
            var durationRange = await calculatorPage.GetDurationRangeAsync();
            await calculatorPage.UpdateInvestmentDuration(new Random().Next(durationRange.Key, durationRange.Value));
            await calculatorPage.VerifyProjectedValue();
            await calculatorPage.ClickPurchaseButton();
            await Expect(Page).ToHaveURLAsync(""); //validate URL redirects to the successful purchase page.
        }

        [Test, Category("Regression")] //purely black-box, can be run on any environment
        public async Task GotoLearnMoreTest()
        {
            var page = await Browser.NewPageAsync();
            await page.GotoAsync(BaseUrl);

            var calculatorPage = new GrowthCalculatorPage(page);
            await calculatorPage.GotoLearnMoreAsync();

            var steadyPaceInfoPage = new SteadyPaceInfoPage(page);
            await Expect(steadyPaceInfoPage.Title).ToBeVisibleAsync();
        }

        [Test, Category("Regression")]
        public async Task PurchaseSteadyPaceCaculatorMissingFieldTest()
        {
            var page = await Browser.NewPageAsync();
            await page.GotoAsync(BaseUrl);

            var calculatorPage = new GrowthCalculatorPage(page);
            //missing investment amount by design
            await calculatorPage.UpdateInvestmentDuration();
            await calculatorPage.ClickPurchaseButton();
            Assert.IsTrue(await calculatorPage.VerifyInvestmentAmountError("Missing investment amount"));
        }

        [Test, Category("Regression")]
        public async Task PurchaseSteadyPaceCaculatorWithDefaultDurationTest()
        {
            var page = await Browser.NewPageAsync();
            await page.GotoAsync(BaseUrl);

            var calculatorPage = new GrowthCalculatorPage(page);
            var investmentRange = await calculatorPage.GetInvestmentAmountRangeAsync();
            await calculatorPage.FillinInvestmentAmountAsync(new Random().Next(investmentRange.Key, investmentRange.Value).ToString());
            await calculatorPage.ClickPurchaseButton();
            await Expect(Page).ToHaveURLAsync(""); //validate URL redirects to the successful purchase page.
        }

        [Category("Functional")]
        [TestCase("2000", "2000")]//Happy path
        [TestCase("1000001", "Error:Cannot larger than max")]//invest amount > max
        [TestCase("999", "Error:Cannot smaller than min")]//invest amount < min
        [TestCase("1000", "1000")]//invest amount = min
        [TestCase("1000000", "1000000")]//invest amount = max
        [TestCase("-1", "Error:Cannot smaller than min")]//invest amount negative
        [TestCase("0", "Error:Cannot smaller than min")]//invest amount 0
        [TestCase("02000", "2000")]//invest amount starts with 0
        [TestCase("100,000", "10000")]//invest amount with comma
        [TestCase("299.99", "299.99")]//invest amount with decimal
        [TestCase("1000,00", "100,000")]//invest amount with invalid comma
        [TestCase("299.99293", "299.992")]//invest amount with more than two digit decimals
        [TestCase("299.9", "299.9")]//invest amount with one digit decimals
        [TestCase("$2000", "2000")]//invest amount with special characters:$
        [TestCase("1(230)*&^%$#", "Error:Invalid amount")]//invest amount with invalid strings
        public async Task InvestmentAmountFunctionalTest(string inputAmount, string expected)
        {
            var page = await Browser.NewPageAsync();
            await page.GotoAsync(BaseUrl);

            var calculatorPage = new GrowthCalculatorPage(page);
            await calculatorPage.FillinInvestmentAmountAsync(inputAmount);
            if (expected.StartsWith("Error:"))
            {
                var errorMsg = expected.Split(":")[1];
                Assert.IsTrue(await calculatorPage.VerifyInvestmentAmountError(errorMsg));
            }
            else
            {
                Assert.That(expected, Is.EqualTo(await calculatorPage.ReadInvestmentAmountAsync()));
            }
        }


        [Category("Functional")]
        [TestCase(3, 3)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        public async Task InvestmentDurationDragAndDrop(int duration, int expectedYears)
        {
            var page = await Browser.NewPageAsync();
            await page.GotoAsync(BaseUrl);

            var calculatorPage = new GrowthCalculatorPage(page);
            await calculatorPage.UpdateInvestmentDuration(duration);
            Assert.IsTrue(await calculatorPage.VerifyInvestmentDuration(expectedYears));
        }
    }
}

