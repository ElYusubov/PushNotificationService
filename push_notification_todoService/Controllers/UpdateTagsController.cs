using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace push_notification_todoService.Controllers
{
    [MobileAppController]
    public class UpdateTagsController : ApiController
    {

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }

        // GET api/UpdateTags/Id
        [HttpGet]
        public async Task<List<string>> GetTagsByInstallationId(string Id)
        {
            try
            {
                HttpConfiguration config;
                NotificationHubClient hub;
                SetHubClient(out config, out hub);

                // Return the installation for the specific ID.
                var installation = await hub.GetInstallationAsync(Id);
                return installation.Tags as List<string>;
            }
            catch (MessagingException ex)
            {
                throw ex;
            }
        }


        // POST api/UpdateTags/Id
        [HttpPost]
        public async Task<HttpResponseMessage> AddTagsToInstallation(string Id)
        {
            // Note: Id is a installationId

            // TODO: Get tag Json array from config 
            var message = await this.Request.Content.ReadAsStringAsync();

            // Validate the submitted tags.
            if (string.IsNullOrEmpty(message) || message.Contains("sid:"))
            {
                // We can't trust users to submit their own user IDs.
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // Verify that the tags are a valid JSON array.
            var tags = JArray.Parse(message);

            // Define a collection of PartialUpdateOperations. Note that 
            // only one '/tags' path is permitted in a given collection.
            var updates = new List<PartialUpdateOperation>();

            // Add a update operation for the tag.
            updates.Add(new PartialUpdateOperation
            {
                Operation = UpdateOperationType.Add,
                Path = "/tags",
                Value = tags.ToString()
            });

            try
            {
                HttpConfiguration config;
                NotificationHubClient hub;
                SetHubClient(out config, out hub);

                // Add the requested tag to the installation.
                await hub.PatchInstallationAsync(Id, updates);

                // Return success status.
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (MessagingException)
            {
                // When an error occurs, return a failure status.
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }


        private void SetHubClient(out HttpConfiguration config, out NotificationHubClient hub)
        {
            string notificationHubName;
            string notificationHubConnection;

            // Get the settings for the server project.
            config = this.Configuration;
            MobileAppSettingsDictionary settings =
            this.Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                //TODO: Get from config file 
                notificationHubName = "notification-hub-17";
                notificationHubConnection = 
                    "Endpoint=sb://ns-notification-hub-17.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=7JgMPUZd4coxOv17V0gGQ5Pyd62adkc8Uw5Sa5Kc0XY=";
            }
            else
            {
                // Get the Notification Hubs credentials for the Mobile App.
                notificationHubName = settings.NotificationHubName;
                notificationHubConnection = settings
                    .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;
            }                       

            // Create a new Notification Hub client.
            hub = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);
        }

    }
}