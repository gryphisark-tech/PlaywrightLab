using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightLab;

[TestFixture]
public class TodoTests : PageTest
{
    [SetUp]
    public async Task GoToApp()
    {
        await Page.GotoAsync("https://demo.playwright.dev/todomvc");
    }

    [Test]
    public async Task CanAddATodo()
    {
        var newTodo = Page.GetByPlaceholder("What needs to be done?");

        await newTodo.FillAsync("Buy an apple");
        await newTodo.PressAsync("Enter");

        await Expect(Page.GetByTestId("todo-title")).ToHaveCountAsync(1);
        await Expect(Page.GetByTestId("todo-title")).ToHaveTextAsync("Buy an apple");  
    }

    [Test]
    public async Task CanCompleteATodo()
    {
        var newTodo = Page.GetByPlaceholder("What needs to be done?");
        var todo = Page.GetByTestId("todo-item");

        await newTodo.FillAsync("Wash the Car");
        await newTodo.PressAsync("Enter");

        await todo.GetByRole(AriaRole.Checkbox).CheckAsync();

        await Expect(todo.GetByRole(AriaRole.Checkbox)).ToBeCheckedAsync();
        await Expect(Page.GetByText("0 items left")).ToBeVisibleAsync();       
    }

[Test]
    public async Task CanFilterByStatus()
    {
        
    }

[Test]
    public async Task CanClearCompletedTodos()
    {
        
    }

    [Test]
    public async Task CanEditATodo()
    {
        
    }
}

