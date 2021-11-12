using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EScooter.Rent.ManageRents
{
    public record RentConfirmed(Guid RentId, Guid CustomerId, Guid ScooterId, DateTime Timestamp);

    public record RentCancelledOrStopped(Guid RentId, Guid CustomerId, Guid ScooterId);

    public static class ManageRents
    {
        [Function("add-rent")]
        public static async void AddRent([ServiceBusTrigger("%TopicName%", "%AddSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg, FunctionContext context)
        {
            var logger = context.GetLogger("add-rent");
            string digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            var credential = new DefaultAzureCredential();
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(digitalTwinUrl), credential);

            var message = JsonConvert.DeserializeObject<RentConfirmed>(mySbMsg);

            try
            {
                await DTUtils.CreateRentRelationship(
                    message.RentId,
                    message.CustomerId,
                    message.ScooterId,
                    message.Timestamp,
                    digitalTwinsClient);

                logger.LogInformation($"Add Rent: {mySbMsg}");
            }
            catch (RequestFailedException e)
            {
                logger.LogInformation($"Failed to add Rent: {e.Message}");
            }
        }

        [Function("remove-rent")]
        public static async void RemoveRent([ServiceBusTrigger("%TopicName%", "%RemoveSubscription%", Connection = "ServiceBusConnectionString")] string mySbMsg, FunctionContext context)
        {
            var logger = context.GetLogger("remove-rent");
            string digitalTwinUrl = "https://" + Environment.GetEnvironmentVariable("AzureDTHostname");
            var credential = new DefaultAzureCredential();
            var digitalTwinsClient = new DigitalTwinsClient(new Uri(digitalTwinUrl), credential);

            var message = JsonConvert.DeserializeObject<RentCancelledOrStopped>(mySbMsg);

            try
            {
                await DTUtils.RemoveRentRelationship(
                    message.RentId,
                    message.CustomerId,
                    digitalTwinsClient);

                logger.LogInformation($"Remove rent: {mySbMsg}");
            }
            catch (RequestFailedException e)
            {
                logger.LogInformation($"Failed to add Rent: {e.Message}");
            }
        }

        internal static class DTUtils
        {
            private const string RelationshipName = "is_riding";

            public static async Task CreateRentRelationship(Guid rentId, Guid customerId, Guid scooterId, DateTime timeStamp, DigitalTwinsClient digitalTwinsClient)
            {
                var relationship = new BasicRelationship
                {
                    TargetId = scooterId.ToString(),
                    Name = RelationshipName,
                    Properties = new Dictionary<string, object>()
                    {
                        { "start", timeStamp }
                    }
                };
                await digitalTwinsClient.CreateOrReplaceRelationshipAsync(customerId.ToString(), rentId.ToString(), relationship);
            }

            public static async Task RemoveRentRelationship(Guid rentId, Guid customerId, DigitalTwinsClient digitalTwinsClient)
            {
                await digitalTwinsClient.DeleteRelationshipAsync(customerId.ToString(), rentId.ToString());
            }
        }
    }
}
