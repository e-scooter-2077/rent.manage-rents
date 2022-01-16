using System;
using System.Net.Http;
using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.Rent.ManageRents
{
    public record RentConfirmed(Guid RentId, Guid CustomerId, Guid ScooterId, DateTime Timestamp);

    public record RentCancelledOrStopped(Guid RentId, Guid CustomerId, Guid ScooterId);

    public static partial class ManageRents
    {
        private static readonly HttpClient _httpClient = new();

        private static DigitalTwinsClient CreateDigitalTwinsClient()
        {
            var digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            var credential = new DefaultAzureCredential();
            return new DigitalTwinsClient(new Uri(digitalTwinUrl), credential, new DigitalTwinsClientOptions
            {
                Transport = new HttpClientTransport(_httpClient)
            });
        }

        [Function("add-rent")]
        public static async void AddRent([ServiceBusTrigger("%TopicName%", "%AddSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg, FunctionContext context)
        {
            var logger = context.GetLogger("add-rent");
            var message = JsonConvert.DeserializeObject<RentConfirmed>(mySbMsg);
            var client = CreateDigitalTwinsClient();
            await DTUtils.CreateRentRelationship(
                message.RentId,
                message.CustomerId,
                message.ScooterId,
                message.Timestamp,
                client);

            logger.LogInformation($"Add Rent: {mySbMsg}");
        }

        [Function("remove-rent")]
        public static async void RemoveRent([ServiceBusTrigger("%TopicName%", "%RemoveSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg, FunctionContext context)
        {
            var logger = context.GetLogger("remove-rent");
            var message = JsonConvert.DeserializeObject<RentCancelledOrStopped>(mySbMsg);
            var client = CreateDigitalTwinsClient();

            await DTUtils.RemoveRentRelationship(
                message.RentId,
                message.CustomerId,
                client);

            logger.LogInformation($"Remove rent: {mySbMsg}");
        }
    }
}
