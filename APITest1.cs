using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Interfaces;
using System.Text.Json.Nodes;

namespace PlaywrightLab;

[TestFixture]
public class ApiTests : PlaywrightTest
{
    private IAPIRequestContext _request = null!;

    [SetUp]
    public async Task SetUpApi()
    {
        _request = await Playwright.APIRequest.NewContextAsync(new()
        {
            BaseURL = "https://jsonplaceholder.typicode.com"
        });
    }

    [TearDown]
    public async Task TearDownApi()
    {
         await _request.DisposeAsync();
    }

    [Test]
    public async Task GetASingleTodo()
    {
        var response = await _request.GetAsync("/todos/1");

        Assert.That(response.Status, Is.EqualTo(200));

        var body = await response.JsonAsync();

        Assert.That(body?.GetProperty("id").GetInt32(), Is.EqualTo(1));
        Assert.That(body?.GetProperty("title").GetString(), Is.Not.Empty);
    }

    [Test]
    public async Task CreatesATodo()
    {
        var response = await _request.PostAsync("/todos", new()
        {
            DataObject = new { title = "Buy an apple", completed = false, userId = 1 }
        });

        Assert.That(response.Status, Is.EqualTo(201));

        var body = await response.JsonAsync();

        Assert.That(body?.GetProperty("title").GetString(), Is.EqualTo("Buy an apple"));
        Assert.That(body?.GetProperty("id").GetInt32(), Is.GreaterThan(0));
    }



}
