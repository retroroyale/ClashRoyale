using System.IO;
using System.Threading.Tasks;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Utilities.Utils;
using SharpRaven.Data;

namespace ClashRoyale.Files
{
    public class UpdateManager
    {
        public const string BaseDir = "GameAssets/";
        public const string PatchDir = BaseDir + "update/";
        public const string TempDir = PatchDir + "temp/";

        public async Task Initialize()
        {
            if (!Resources.Configuration.UseContentPatch) return;
            var assetsChanged = await CheckForChanges();
            if (!assetsChanged) return;

            Logger.Log("Assets have been updated. Creating patch...", GetType(), ErrorLevel.Warning);

            await CreatePatch();

            Logger.Log($"Fingerprint updated to [v{Resources.Fingerprint.GetVersion}]", GetType());
        }

        /// <summary>
        /// This task checks if there have been made any changes to an asset for a new patch
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckForChanges()
        {
            var modified = false;

            foreach (var asset in Resources.Fingerprint.Files)
            {
                var hasChanged = await asset.HasFileChanged();
                if (!hasChanged) continue;

                modified = true;
                break;
            }

            return modified;
        }

        /// <summary>
        ///     Creates a new patch if the files have been updated
        /// </summary>
        public async Task CreatePatch()
        {
            if (!Directory.Exists(PatchDir)) Directory.CreateDirectory(PatchDir);

            var fingerprint = Resources.Fingerprint;

            foreach (var asset in fingerprint.Files)
            {
                var path = Path.Combine(BaseDir, asset.File);
                if (!File.Exists(path)) return;

                var expression = Path.GetExtension(asset.File).Replace(".", string.Empty);
                var newPath = Path.Combine(TempDir, asset.File);
                var newDir = Path.GetDirectoryName(newPath);

                if (!Directory.Exists(newDir)) 
                    Directory.CreateDirectory(newDir);

                switch (expression)
                {
                    case "csv":
                    {
                        var rawData = await File.ReadAllBytesAsync(path);
                        var compressedData = CompressionUtils.CompressData(rawData);
                        var sha = ServerUtils.GetChecksum(compressedData);

                        asset.Sha = sha;
                        await File.WriteAllBytesAsync(newPath, compressedData);
                        break;
                    }

                    case "sc":
                    {
                        var compressedData = await File.ReadAllBytesAsync(path);
                        var sha = ServerUtils.GetChecksum(compressedData);

                        asset.Sha = sha;
                        await File.WriteAllBytesAsync(newPath, compressedData);
                        break;
                    }

                    default:
                    {
                        Logger.Log($"Unknown file expression {expression}", GetType(), ErrorLevel.Warning);
                        break;
                    }
                }
            }

            fingerprint.Version[2]++;

            fingerprint.Sha = ServerUtils.GetChecksum(fingerprint.GetVersion);
            fingerprint.Save();

            Directory.Move(TempDir, Path.Combine(PatchDir, fingerprint.Sha));
            File.Copy(Path.Combine(BaseDir, "fingerprint.json"),
                Path.Combine(PatchDir, fingerprint.Sha, "fingerprint.json"), true);
        }
    }
}