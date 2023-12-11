using CloudyBot.Functions;
using CloudyBot.Models;
using CloudyBot.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CloudyBotTest
{
    public class InteractionFuncTest
    {
        [Fact]
        public async Task HttpTrigger_ShouldReturnAckPing_WhenTypeIsOne()
        {
            Dictionary<string, int> ackPing = new() { { "Type", 1 } };
            var payload = JsonConvert.SerializeObject(ackPing);

            var mockCommand = new Mock<ICommand>();
            mockCommand.Setup(x => x.Type)
                .Returns(CommandType.Ping);
            mockCommand.Setup(x => x.AcknowledgePing())
                .Returns(payload);

            var mockHttpRequest = new Mock<HttpRequest>();
            var mockCommandService = new Mock<ICommandService>();
            mockCommandService.Setup(x => x.ParseHttpCommandAsync(mockHttpRequest.Object))
                .ReturnsAsync(mockCommand.Object);

            var mockCollector = new Mock<ICollector<string>>();
            var mockLogger = new Mock<ILogger>();

            var function = new InteractionFunc(mockCommandService.Object);

            var response = await function.Run(mockHttpRequest.Object, mockCollector.Object, mockLogger.Object);

            var json = (JsonContentResult)response;
            Assert.True(json.StatusCode == 200, "Failed to return good ping");
            Assert.True(json.Content == payload, "Payload does not match");
        }
    }
}