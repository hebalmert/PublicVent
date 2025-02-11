using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Vent.Helpers;

public class FileStorage : IFileStorage
{
    private readonly string connectionString;

    public FileStorage(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("AzureStorage")!;
    }

    //Para Manejo de Imagenes en AZURE Containers

    public async Task RemoveFileAsync(string path, string containerName)
    {
        var client = new BlobContainerClient(connectionString, containerName);
        await client.CreateIfNotExistsAsync();
        var fileName = Path.GetFileName(path);
        var blob = client.GetBlobClient(fileName);
        await blob.DeleteIfExistsAsync();
    }

    public async Task<string> SaveFileAsync(byte[] content, string extention, string containerName)
    {
        var client = new BlobContainerClient(connectionString, containerName);
        await client.CreateIfNotExistsAsync();
        client.SetAccessPolicy(PublicAccessType.Blob);
        var fileName = $"{Guid.NewGuid()}{extention}";
        var blob = client.GetBlobClient(fileName);

        using (var ms = new MemoryStream(content))
        {
            await blob.UploadAsync(ms);
        }
        //Es para obtener la url completa junto con el archivo
        //return blob.Uri.ToString();
        return fileName;
    }

    public async Task<string> UploadImage(IFormFile imageFile, string ruta, string guid)
    {
        var file = guid;
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            ruta,
            file);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        return $"{file}";
    }

    public async Task<string> UploadImage(byte[] imageFile, string ruta, string guid)
    {
        var file = guid;
        var path = Path.Combine(
            Directory.GetCurrentDirectory(),
            ruta,
            file);

        var NIformFile = new MemoryStream(imageFile);
        using (var stream = new FileStream(path, FileMode.Create))
        {
            await NIformFile.CopyToAsync(stream);
        }

        return $"{file}";
    }

    public bool DeleteImage(string ruta, string guid)
    {
        string path;
        path = Path.Combine(
            Directory.GetCurrentDirectory(),
            ruta,
            guid);

        File.Delete(path);

        return true;
    }
}