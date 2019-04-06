using System.IO;
using System.Linq;
using ClashRoyale.Extensions.Utils;

namespace ClashRoyale.Files
{
    public class UpdateManager
    {
        public const string BaseDir = "GameAssets\\";
        public const string PatchDir = BaseDir + "update\\";
        public const string TempDir = PatchDir + "temp\\";

        public UpdateManager()
        {
            if (!AssetsChanged) return;

            Logger.Log("Assets have been updated. Creating patch...",GetType());

            CreatePatch();
        }

        public void CreatePatch()
        {
            if (!Directory.Exists(PatchDir)) Directory.CreateDirectory(PatchDir);

            /*foreach (var dir in Directory.GetDirectories(BaseDir))
            {
                if (dir.Contains("update")) continue;

                var newDir = dir.Replace(BaseDir, TempDir) + "\\";

                if (!Directory.Exists(newDir))
                    Directory.CreateDirectory(newDir);

                foreach (var updatedFile in Directory.GetFiles(dir))
                {
                    var data = ServerUtils.CompressData(File.ReadAllBytes(updatedFile));
                    var name = Path.GetFileName(updatedFile);
                    var newPath = newDir + name;

                    File.WriteAllBytes(newPath, data);
                }
            }*/

            // TODO: update Fingerprint & rename temp dir
        }

        public bool AssetsChanged =>
            Directory.GetDirectories(BaseDir).Any(dir =>
                (from file in Directory.GetFiles(dir)
                    let name = file.Replace(BaseDir, string.Empty).Replace('\\', '/')
                    let data = ServerUtils.CompressData(File.ReadAllBytes(file))
                    let sha = ServerUtils.GetChecksum(data)
                    select Resources.Fingerprint.Files.FindIndex(a => a.File == name)).Any(index => index > -1));
    }
}