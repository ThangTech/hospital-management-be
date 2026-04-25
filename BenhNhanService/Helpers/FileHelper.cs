namespace BenhNhanService.Helpers
{
    public static class FileHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly long MaxFileSize = 2 * 1024 * 1024; // 2MB

        public static async Task<string?> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            // Check extension
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                throw new Exception("Định dạng file không được hỗ trợ. Chỉ chấp nhận: jpg, jpeg, png, gif, webp");

            // Check size
            if (file.Length > MaxFileSize)
                throw new Exception("Kích thước file vượt quá 2MB");

            // Create folder if not exists
            var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);
            if (!Directory.Exists(webRoot))
                Directory.CreateDirectory(webRoot);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(webRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folderName}/{fileName}";
        }

        public static void DeleteFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}