using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.CLI
{
    public static class AppStarter
    {
        public static async Task RunAsync()
        {
            using var cts = new CancellationTokenSource();

            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
                Console.WriteLine("\nCancellation requested. Exiting...");
            };

            var provider = DIContainer.Build();
            var menu = provider.GetRequiredService<Menu>();

            await menu.RunAsync(cts.Token);
        }
    }
}
