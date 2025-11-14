using Microsoft.Extensions.DependencyInjection;
using Reading_List.CLI;

class Program
{
    static async Task Main()
    {
        var provider = DIContainer.Build();
        var menu = provider.GetRequiredService<Menu>();
        await menu.RunAsync();
    }
}
