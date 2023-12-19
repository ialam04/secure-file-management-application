using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using BlazorAuth.Models;

public class FileService
{
    private readonly AuditService _auditService;
    private readonly string _fileStoragePath;

    public FileService(AuditService auditService, IOptions<FileServiceOptions> options)
    {
        _auditService = auditService;
        _fileStoragePath = options.Value.FileStoragePath;
    }

    public Task<List<string>> GetFilesAsync()
    {
        var files = Directory.GetFiles(_fileStoragePath)
                             .Select(Path.GetFileName)
                             .ToList();
        return Task.FromResult(files);
    }

    public async Task UploadFileAsync(string username, IBrowserFile file)
    {
        if (file == null || file.Size == 0)
        {
            throw new ArgumentException("File is empty", nameof(file));
        }

        var filePath = Path.Combine(_fileStoragePath, file.Name);
        using (var stream = file.OpenReadStream()) 
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream); 
            }
        }

        await _auditService.LogActionAsync(username, "Upload", $"File uploaded: {file.Name}");
    }

    public async Task DeleteFileAsync(string username, string fileName)
    {
        String deleteFile = Path.Combine(_fileStoragePath, fileName);
        if (File.Exists(deleteFile))
        {
            File.Delete(deleteFile);
            await _auditService.LogActionAsync(username, "Delete", $"File deleted: {fileName}");
        }
        else
        {
            throw new FileNotFoundException("File not found", fileName);
        }
    }

    public async Task<byte[]> DownloadFileAsync(string username, string fileName)
    {
        string filePath = Path.Combine(_fileStoragePath, fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        byte[] fileBytes;
        using (var memoryStream = new MemoryStream())
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memoryStream);
            }
            fileBytes = memoryStream.ToArray();
        }

        await _auditService.LogActionAsync(username, "Download", $"File downloaded: {fileName}");
        return fileBytes;
    }

    public async Task<Stream> GetFileAsync(string fileName)
    {
        string filePath = Path.Combine(_fileStoragePath, fileName);
        if (!File.Exists(filePath))
        {
            return null;
        }

        return new FileStream(filePath, FileMode.Open, FileAccess.Read);
    }
}

