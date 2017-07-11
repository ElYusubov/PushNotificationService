using System;
using System.Collections.Generic;
using System.Data.Entity;
using push_notification_todoService.DataObjects;
using push_notification_todoService.Models;

namespace push_notification_todoService
{
    public class push_notification_todoInitializer : CreateDatabaseIfNotExists<push_notification_todoContext>
    {
        protected override void Seed(push_notification_todoContext context)
        {
            List<TodoItem> todoItems = new List<TodoItem>
            {
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "First item", Complete = false },
                new TodoItem { Id = Guid.NewGuid().ToString(), Text = "Second item", Complete = false },
            };

            foreach (TodoItem todoItem in todoItems)
            {
                context.Set<TodoItem>().Add(todoItem);
            }

            base.Seed(context);
        }
    }
}

