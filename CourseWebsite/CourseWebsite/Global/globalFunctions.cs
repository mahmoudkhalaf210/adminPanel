using System.Text.RegularExpressions;

namespace CourseWebsite.Global
{
    public class globalFunctions
    {
        public static bool IsPhotoFile(string fileExtension)
        {
            string[] photoExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            return photoExtensions.Contains(fileExtension);
        }

        public static bool IsPdfFile(string fileExtension)
        {
            string[] pdfExtensions = { ".pdf" };
            return pdfExtensions.Contains(fileExtension);
        }

        public static bool IsVideoFile(string fileExtension)
        {
            string[] videoExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };
            return videoExtensions.Contains(fileExtension);
        }
        /*
                public static string GenerateUniqueFileName(string postHeader, string fileExtension)
                {
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    string randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 6);
                    return $"{postHeader}_{timestamp}_{randomSuffix}{fileExtension}";
                }
        */

        public static string GenerateUniqueFileName(string postHeader, string fileExtension)
        {
            // Remove any unwanted special characters from the postHeader using a regular expression
            string sanitizedPostHeader = Regex.Replace(postHeader, @"[^a-zA-Z0-9\s]", "");

            // Generate timestamp and random suffix
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 6);

            // Return the sanitized postHeader with a unique file name
            return $"{sanitizedPostHeader}_{timestamp}_{randomSuffix}{fileExtension}";
        }




        public static void DeleteFile(string fileName, string folderName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                string filePath = Path.Combine(folderName, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }



        // 
        public static string addFileandDeleteOldIfHas(IFormFile file, string header, string path, string oldpdfname, string typeFile)
        {

            if (file != null)
            {
                string fileExtension = Path.GetExtension(file.FileName).ToLower();

                if (typeFile == "pdf")
                {
                    if (!globalFunctions.IsPdfFile(fileExtension)) { return "Invalid file format or extension."; }
                }
                else
                {
                    if (!globalFunctions.IsPhotoFile(fileExtension)) { return "Invalid file format or extension."; }
                }
                string uniqueFileName = globalFunctions.GenerateUniqueFileName(header, fileExtension);
                string folderName = path;       //newPost.posttype == 1 ? "PostData/images" : "PostData/videos";
                string filePath = Path.Combine(folderName, uniqueFileName);

                // Ensure the directory exists
                System.IO.Directory.CreateDirectory(folderName);

                // check if has old and delete 
                string oldpdfnamepath = !string.IsNullOrEmpty(oldpdfname) ? Path.Combine(folderName, oldpdfname) : "";
                if (!string.IsNullOrEmpty(oldpdfnamepath) && File.Exists(oldpdfnamepath))
                {
                    File.Delete(oldpdfnamepath);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
                return uniqueFileName;

            }
            return "";
        }




    }
}
