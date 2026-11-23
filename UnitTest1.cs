using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Interfaces;

namespace PlaywrightLab;

[TestFixture]
public class TodoTests : PageTest
{
    private ILocator NewTodoInput => Page.GetByPlaceholder("What needs to be done?");
    private ILocator Todos => Page.GetByTestId("todo-item");
    private ILocator TodoTitles => Page.GetByTestId("todo-title");
    private ILocator Todo(string text) => Todos.Filter(new() { HasText = text});

    [SetUp]
    public async Task GoToApp()
    {
        await Page.GotoAsync("https://demo.playwright.dev/todomvc");
    }

    private async Task AddTodoAsync(string text)
    {
       await NewTodoInput.FillAsync(text);
       await NewTodoInput.PressAsync("Enter"); 

    }

    [Test]
    public async Task CanAddATodo()
    {
        var newTodo = Page.GetByPlaceholder("What needs to be done?");

        await AddTodoAsync("Buy an apple");

        await Expect(TodoTitles).ToHaveTextAsync(new[] { "Buy an apple"});
        await Expect(Page.GetByText("1 item left")).ToBeVisibleAsync();
    }

    [Test]
    public async Task CanCompleteATodo()
    {
        await AddTodoAsync("Wash the car");

        await Todo("Wash the car").GetByRole(AriaRole.Checkbox).CheckAsync();

        await Expect(Todo("Wash the car").GetByRole(AriaRole.Checkbox)).ToBeCheckedAsync();
        await Expect(Page.GetByText("0 items left")).ToBeVisibleAsync();

    }

[Test]
    public async Task CanFilterByStatus()
    {
        await AddTodoAsync("Buy an apple");
        await AddTodoAsync("Wash the car");
        await AddTodoAsync("Call the doctor");

        await Todo("Wash the car").GetByRole(AriaRole.Checkbox).CheckAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Active" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("#/active$"));
        await Expect(TodoTitles).ToHaveTextAsync(new[] { "Buy an apple", "Call the doctor" });

        await Page.GetByRole(AriaRole.Link, new() { Name = "Completed" }).ClickAsync();
        await Expect(Page).ToHaveURLAsync(new Regex("#/completed$"));
        await Expect(TodoTitles).ToHaveTextAsync(new[] { "Wash the car" });

        await Page.GetByRole(AriaRole.Link, new() { Name = "All" }).ClickAsync();
        await Expect(TodoTitles).ToHaveTextAsync(new[] { "Buy an apple", "Wash the car", "Call the doctor" });
    }

[Test]
    public async Task CanClearCompletedTodos()
    {
        await AddTodoAsync("Buy an apple");
        await AddTodoAsync("Wash the car");
        await AddTodoAsync("Call the doctor");

        await Todo("Buy an apple").GetByRole(AriaRole.Checkbox).CheckAsync();
        await Todo("Call the doctor").GetByRole(AriaRole.Checkbox).CheckAsync();

        var clearCompleted = Page.GetByRole(AriaRole.Button, new() { Name = "Clear completed" });
        await Expect(clearCompleted).ToBeVisibleAsync();
        await clearCompleted.ClickAsync();

        await Expect(TodoTitles).ToHaveTextAsync (new[] { "Wash the car"});
        await Expect(clearCompleted).ToBeHiddenAsync();
        await Expect(Page.GetByText("1 item left")).ToBeVisibleAsync();
    }

    [Test]
    public async Task CanEditATodo()
    {
        await AddTodoAsync("Buy an apple");

        await TodoTitles.DblClickAsync();

        var editBox = Page.GetByRole(AriaRole.Textbox, new() { Name = "Edit"});
        await Expect(editBox).ToHaveValueAsync("Buy an apple");

        await editBox.FillAsync("Buy two apples");
        await editBox.PressAsync("Enter");

        await Expect(TodoTitles).ToHaveTextAsync(new[] {"Buy two apples" });
    }

      [Test]
    public async Task EscapeCancelsAnEdit()
    {
        await AddTodoAsync("Buy an apple");

        await TodoTitles.DblClickAsync();

        var editBox = Page.GetByRole(AriaRole.Textbox, new() { Name = "Edit"});
        await editBox.FillAsync("Buy two apples");
        await editBox.PressAsync("Escape");

        await Expect(TodoTitles).ToHaveTextAsync(new[] { "Buy an apple"});
    }
}

