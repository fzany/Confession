using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.IO;

namespace Backend.Helpers
{
    public class Cloud
    {
        private static Cloudinary Connect()
        {
            Account account = new Account(
      Environment.GetEnvironmentVariable("CloudName") ?? "booksrite",
      Environment.GetEnvironmentVariable("CloudApiKey") ?? "543541641548544",
      Environment.GetEnvironmentVariable("CloudApiSecret") ?? "5t4owZPalb0KrE5X9CtOrX-gGhA");
            return new Cloudinary(account);
        }

        public static async Task<string> SaveChatImageAsync(Stream data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            Cloudinary cloudinary = Connect();
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(Logic.GetGuid(), data),
                PublicId = Logic.GetGuid(),
                // Transformation = new Transformation().Crop("limit").Width(180).Height(240),
                Format = "png",
                Tags = "chat"
            };
            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUri.AbsoluteUri;
        }

        public static async Task<string> SaveByteArrayAsync(byte[] data)
        {
            if (data == null)
            {
                return string.Empty;
            }

            Stream stream = new MemoryStream(data);
            Cloudinary cloudinary = Connect();
            ImageUploadParams uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(Logic.GetGuid(), stream),
                PublicId = Logic.GetGuid()
            };
            ImageUploadResult uploadResult = await cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUri.AbsoluteUri;

        }
    }

}
