using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClashRoyale.Extensions.Utils;
using ClashRoyale.Utilities.Utils;

namespace ClashRoyale.Files
{
    public class UpdateManager
    {
        public const string BaseDir = "GameAssets/";
        public const string PatchDir = BaseDir + "update/";
        public const string TempDir = PatchDir + "temp/";

        public UpdateManager()
        {
            if (!Resources.Configuration.UseContentPatch) return;
            if (!AssetsChanged) return;

            Logger.Log("Assets have been updated. Creating patch...", GetType());

            CreatePatch();

            Logger.Log($"Fingerprint updated to v.{Resources.Fingerprint.GetVersion}", GetType());
        }

        /// <summary>
        ///     Returns true wether the files have been updated
        /// </summary>
        public bool AssetsChanged
        {
            get
            {
                var files = Resources.Fingerprint.Files;
                return Directory.GetDirectories(BaseDir).Where(d => !d.Contains("update")).Any(dir =>
                    (from file in Directory.GetFiles(dir)
                        let sha = ServerUtils.GetChecksum(file.Contains("sc")
                            ? File.ReadAllBytes(file)
                            : CompressionUtils.CompressData(File.ReadAllBytes(file)))
                        let name = file.Replace(BaseDir, string.Empty).Replace('\\', '/')
                        let index = files.FindIndex(x => x.File == name)
                        where index > -1
                        let check = files[index]
                        where check.Sha != sha
                        select sha).Any());
            }
        }

        /// <summary>
        ///     Creates a new patch if the files have been updated
        /// </summary>
        public void CreatePatch()
        {
            if (!Directory.Exists(PatchDir)) Directory.CreateDirectory(PatchDir);

            var files = new List<Asset>();
            var fingerprint = Resources.Fingerprint;

            foreach (var dir in Directory.GetDirectories(BaseDir))
            {
                if (dir.Contains("update")) continue;

                var newDir = dir.Replace(BaseDir, TempDir) + "/";

                if (!Directory.Exists(newDir))
                    Directory.CreateDirectory(newDir);

                foreach (var updatedFile in Directory.GetFiles(dir))
                {
                    var data = updatedFile.Contains("sc")
                        ? File.ReadAllBytes(updatedFile)
                        : CompressionUtils.CompressData(File.ReadAllBytes(updatedFile)); // only compress csv
                    var name = Path.GetFileName(updatedFile);
                    var newPath = newDir + name;

                    files.Add(new Asset
                    {
                        File = dir.Split('/').Last() + "/" + name,
                        Sha = ServerUtils.GetChecksum(data)
                    });

                    File.WriteAllBytes(newPath, data);
                }
            }

            fingerprint.Files = files;
            fingerprint.Version[2]++;

            fingerprint.Sha = ServerUtils.GetChecksum(fingerprint.GetVersion);
            fingerprint.Save();

            Directory.Move(TempDir, PatchDir + fingerprint.Sha);
        }
    }
}