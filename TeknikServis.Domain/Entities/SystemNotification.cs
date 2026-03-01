using System;
using TeknikServis.Domain.Common;

namespace TeknikServis.Domain.Entities
{
    public class SystemNotification : BaseEntity, IAggregateRoot
    {
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string Color { get; private set; } // Bildirimin rengi (bg-red, bg-blue vs.)

        private SystemNotification() { }

        public static SystemNotification Create(string title, string message, string color = "bg-primary")
        {
            return new SystemNotification
            {
                Title = title,
                Message = message,
                Color = color,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}