using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.IO;
using System.Threading.Tasks;
namespace Mobile.Helpers
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

        public static string SaveChatImage(Stream data)
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
            ImageUploadResult uploadResult = cloudinary.Upload(uploadParams);
            return uploadResult.SecureUri.AbsoluteUri;
        }

        public static string SaveByteArray(byte[] data)
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
                PublicId = Logic.GetGuid(),
                Format = "png",
                Tags = "chat"
            };
            ImageUploadResult uploadResult = cloudinary.Upload(uploadParams);
            return uploadResult.SecureUri.AbsoluteUri;

        }
    }

}
