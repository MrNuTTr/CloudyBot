using CloudyBot.Models;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CloudyBot.Services
{
    public interface ICommandService
    {
        public Task<ICommand> ParseHttpCommandAsync(HttpRequest req);
        public Task<ICommand> ParseJsonStringCommandAsync(string jsonString);
    }
}
