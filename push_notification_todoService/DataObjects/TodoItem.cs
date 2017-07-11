using Microsoft.Azure.Mobile.Server;

namespace push_notification_todoService.DataObjects
{
    public class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}