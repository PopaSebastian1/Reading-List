using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Abstractions
{
    public interface ICommand
    {
        string Key { get; }
        string Description { get; }

        Task<string> ExecuteAsync(CancellationToken ct = default);

    }
}
