using Bourse.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bourse.Services
{
    public class AssetService : IAssetService
    {
        public async Task<string> EnsureAssetCopiedAsync(string assetName, string destPath)
        {
            string dbPathName = string.Empty;
            if (Directory.Exists(destPath))
            {
                using (var input = await FileSystem.OpenAppPackageFileAsync(assetName))
                {
                    dbPathName = Path.Combine(destPath, Path.GetFileName(assetName));
                    using (var output = File.Create(dbPathName))
                    {
                        await input.CopyToAsync(output);
                    }
                }
            }
            return dbPathName;
        }
    }

}
