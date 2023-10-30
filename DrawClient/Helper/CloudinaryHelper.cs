using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace DrawClient.Helper
{
    public class CloudinaryHelper
    {
        private readonly Account account;
        private readonly Cloudinary cloudinary;
        public CloudinaryHelper(IConfiguration config)
        {
            account = new Account(
                config["Cloudinary:Cloud"],
               config["Cloudinary:APIKey"],
                config["Cloudinary:APISecret"]);

            cloudinary = new Cloudinary(account);
        }
        public async Task<string> UploadImageToCloudinaryAsync(IFormFile file)
        {
            string imgUrl;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                byte[] imgData = memoryStream.ToArray();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.Name, new MemoryStream(imgData))
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParams, type: "auto");
                imgUrl = uploadResult.SecureUrl.AbsoluteUri;
            }
            return imgUrl;
        }
    }
}
