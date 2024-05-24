using Microsoft.Playwright.NUnit;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Configuration;

namespace GainBridgeCaculator;

[TestFixture]
public class TestBase : PageTest
{
    public IConfigurationRoot Configuration { get; set; }
    public string BaseUrl = "";
    public int DefaultTimeout = 30; //by seconds
    public bool SmokeTest = false; //get smoke tag, and avoid database/API/etc. check when smoke is true

    [OneTimeSetUp]
    public async Task SetupTestRun()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        Configuration = builder.Build();
        //read appsettings.json for configuration information
        var baseUrl = Configuration.GetSection("ConnectionStrings")["BaseUrl"];
        if (!string.IsNullOrEmpty(baseUrl))
        {
            BaseUrl = baseUrl;
        }
        var defaultTimeout = Configuration.GetSection("ConnectionStrings")["DefaultTimeout"];
        if (!string.IsNullOrEmpty(defaultTimeout))
        {
            DefaultTimeout = Convert.ToInt32(defaultTimeout);
        }
        await TestContext.Out.WriteLineAsync($"Test run starts with timestamp: {DateTime.Now}");
    }

    [OneTimeTearDown]
    public async Task TearDownTestRun()
    {
        try
        {
            await Browser.CloseAsync();
        }
        catch (Exception e)
        {
            await TestContext.Out.WriteLineAsync($"Failed to close browser, exception: {e}");
        }
        await TestContext.Out.WriteLineAsync($"Test run ends with timestamp: {DateTime.Now}");
    }
}