using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sidekick.Infrastructure.Poe
{
    public interface IPoeTradeClient
    {
        Task<List<TReturn>> Fetch<TReturn>(string path, bool useDefaultLanguage = false);

        JsonSerializerOptions Options { get; }
    }
}
