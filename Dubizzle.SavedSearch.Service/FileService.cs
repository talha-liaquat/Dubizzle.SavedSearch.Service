using Dubizzle.SavedSearch.Contracts;
using Dubizzle.SavedSearch.Dto;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dubizzle.SavedSearch.Service
{
    public class FileService : INotificationService<EmailMessageDto>
    {
        private const string rootDir = @"D:\Dubizzle.Subscriptions";
        public FileService()
        {
            if (!Directory.Exists(rootDir))
                Directory.CreateDirectory(rootDir);
        }
        public async Task SendNotificationAsync(EmailMessageDto message)
        {
            var filePath = $@"{rootDir}\{message.CorrelationId}.html";
            await File.WriteAllTextAsync(filePath, message.Body);
            Console.WriteLine($"Processed {filePath}");
        }
    }
}