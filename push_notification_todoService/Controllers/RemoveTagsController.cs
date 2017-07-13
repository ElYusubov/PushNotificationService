using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace push_notification_todoService.Controllers
{
    [MobileAppController]
    public class RemoveTagsController : ApiController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
        }

        // POST api/RemoveTags/Id
        [HttpPost]
        public async Task<HttpResponseMessage> RemoveTagsFromInstallation(string Id)
        {           
            // Define a collection of PartialUpdateOperations. 
            // Note that only one '/tags' path is permitted in a given collection.
            var updates = new List<PartialUpdateOperation>();

            // Add a update operation for the tag.
            updates.Add(new PartialUpdateOperation
            {
                Operation = UpdateOperationType.Remove,
                Path = "/tags"               
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
