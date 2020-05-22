using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace AcceptanceTests
{
    [Binding]
    public class Steps
    {
        [When("the (.*) function is triggered")]
        public async Task TheFunctionIsTriggered(string endpoint)
        {
            HttpClient client = new HttpClient();
            var content = new StringContent("{}", UnicodeEncoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await client.PostAsync($"http://localhost:7071/admin/functions/{endpoint}", content);
            responseMessage.EnsureSuccessStatusCode();
            Thread.Sleep(18000);
        }
    }
}
