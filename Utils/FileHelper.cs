using Microsoft.AspNetCore.Http;
using System.IO;

namespace IdentityServerProject.Utils
{
    public static class FileHelper
    {
        public static bool IsValidExcelFile(IFormFile file)
        {
            string[] validFormats = { ".xls", ".xlsx" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            return Array.Exists(validFormats, format => format == extension);
        }
    }
}
