using Android.Content;
using Android.Provider;

namespace Bourse.ForAndroid
{
    public partial class FileSaver
    {
        public static  async Task<string?> SaveToDownloadsAsync(string filename, byte[] data)
        {
            var context = Platform.AppContext;

            ContentValues values = new ContentValues();
            values.Put(MediaStore.IMediaColumns.DisplayName, filename);
            values.Put(MediaStore.IMediaColumns.MimeType, "application/octet-stream");
            values.Put(MediaStore.IMediaColumns.RelativePath, "Download/");

            using (var resolver = context.ContentResolver)
            {
                if(resolver != null)
                {
                    var uri = resolver.Insert(MediaStore.Downloads.ExternalContentUri, values);
                    if (uri != null)
                    {
                        using (var stream = resolver.OpenOutputStream(uri))
                        {
                            if(stream != null)
                            {
                                await stream.WriteAsync(data);
                                return uri.ToString();
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }

        public static bool Download(string source)
        {
            var result = false;
            var file = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            if (file!=null)
            {
                string dest = Path.Combine(file.AbsolutePath, Path.GetFileName(source));
                File.Copy(source, dest, true);
                result = true;
            }
            return result;
        }
    }
}


