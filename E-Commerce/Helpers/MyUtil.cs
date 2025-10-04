namespace E_Commerce.Helpers
{
    public class MyUtil
    {
        public static string UploadHinh(IFormFile Hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "hinh", folder, Hinh.FileName);

                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myfile);
                }

                return Hinh.FileName;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
