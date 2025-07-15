using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

public class CloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<List<string>> UploadImagesAsync(List<IFormFile> files)
    {
        var photoUrls = new List<string>();

        foreach (var file in files)
        {
            using (var fileStream = file.OpenReadStream())  // Open the file stream
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, fileStream),
                    PublicId = $"uploads/{Guid.NewGuid()}_{file.FileName}"  // Unique PublicId for each file
                };

                try
                {
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    if (uploadResult?.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        photoUrls.Add(uploadResult.SecureUrl.AbsoluteUri);  // Add image URL to the list
                    }
                    else
                    {
                        throw new Exception($"Error uploading image: {uploadResult?.Error?.Message}");
                    }
                }
                catch (Exception ex)
                {
                    // Log the error or throw it
                    throw new Exception($"An error occurred while uploading the image: {ex.Message}");
                }
            }
        }

        return photoUrls;
    }
}
