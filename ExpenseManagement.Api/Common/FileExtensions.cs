using System.Text.RegularExpressions;

namespace ExpenseManagement.Api.Common
{
    public static class FileExtensions
    {
        public const int ImageMinimumBytes = 512;
        public static bool IsImage(this IFormFile fileName) => IsImage(new List<IFormFile>() { fileName });

        public static bool IsImage(this string fileName)
        {
            var imgExtentions = new List<string>() { ".png", ".jpg", ".jpeg" };
            var fileExtention = Path.GetExtension(fileName);
            if (imgExtentions.Contains(fileExtention.ToLower()))
                return true;
            return false;
        }

        public static bool IsImage(this IEnumerable<IFormFile> postedFiles)
        {
            foreach (var postedFile in postedFiles)
            {
                //-------------------------------------------
                //  Check the image extension
                //-------------------------------------------
                if (Path.GetExtension(postedFile.FileName).ToLower() != ".jpg"
                    && Path.GetExtension(postedFile.FileName).ToLower() != ".png"
                    && Path.GetExtension(postedFile.FileName).ToLower() != ".jpeg")
                {
                    return false;
                }

                //-------------------------------------------
                //  Check the image mime types
                //-------------------------------------------
                if (postedFile.ContentType.ToLower() != "image/jpg" &&
                            postedFile.ContentType.ToLower() != "image/jpeg" &&
                            postedFile.ContentType.ToLower() != "image/pjpeg" &&
                            postedFile.ContentType.ToLower() != "image/x-png" &&
                            postedFile.ContentType.ToLower() != "image/png")
                {
                    return false;
                }

                //-------------------------------------------
                //  Attempt to read the file and check the first bytes
                //-------------------------------------------
                try
                {
                    if (!postedFile.OpenReadStream().CanRead)
                    {
                        return false;
                    }
                    //------------------------------------------
                    //check whether the image size exceeding the limit or not
                    //------------------------------------------ 
                    if (postedFile.Length < ImageMinimumBytes)
                    {
                        return false;
                    }

                    byte[] buffer = new byte[ImageMinimumBytes];
                    postedFile.OpenReadStream().Read(buffer, 0, ImageMinimumBytes);
                    string content = System.Text.Encoding.UTF8.GetString(buffer);
                    if (Regex.IsMatch(content, @"<script|<html|<head|<title|<body|<pre|<table|<a\s+href|<img|<plaintext|<cross\-domain\-policy",
                        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Multiline))
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                //-------------------------------------------
                //  Try to instantiate new Bitmap, if .NET will throw exception
                //  we can assume that it's not a valid image
                //-------------------------------------------

                //try
                //{
                //    using (var bitmap = new System.Drawing.Bitmap(postedFile.OpenReadStream()))
                //    {
                //    }
                //}
                //catch (Exception)
                //{
                //    return false;
                //}
                //finally
                //{
                //    postedFile.OpenReadStream().Position = 0;
                //}
            }
            return true;
        }

        public static bool IsPdf(this IEnumerable<IFormFile> postedFiles)
        {
            return postedFiles.All(x => Path.GetExtension(x.FileName).ToLower() == ".pdf");
        }

        public static string ToBase64String(string fileName)
        {
            using var ms = new MemoryStream();

            // Convert Image to byte[]
            byte[] imagesBytes = System.IO.File.ReadAllBytes(fileName);

            // Convert byte[] to Base64 String
            string base64String = Convert.ToBase64String(imagesBytes);

            return base64String;
        }
    }
}