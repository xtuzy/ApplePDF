using System.Reflection;

namespace ApplePDF.Demo.Mac
{
    internal class AssetsManager
    {
        static string ReadEmbedAssetPath(string resourcePath = "foler.fileName.extention", Type type = null)
        {
            if (type == null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                List<string> resourceNames = new List<string>(assembly.GetManifestResourceNames());
                resourcePath = resourcePath.Replace(@"/", ".");
                resourcePath = resourceNames.FirstOrDefault(r => r.Contains(resourcePath));
            }
            else
            {
                List<string> resourceNames = new List<string>(type.Assembly.GetManifestResourceNames());
                resourcePath = resourcePath.Replace(@"/", ".");
                resourcePath = resourceNames.FirstOrDefault(r => r.Contains(resourcePath));
            }

            if (resourcePath == null)
                throw new FileNotFoundException("Resource not found");
            return resourcePath;
        }

        static Stream OpenEmbeddedAssetStream(string name) => typeof(AssetsManager).Assembly.GetManifestResourceStream(name);
        public static byte[] ReadEmbedAssetBytes(string resourcePath = "foler.fileName.extention")
        {
            resourcePath = ReadEmbedAssetPath(resourcePath);
            using (Stream stream = OpenEmbeddedAssetStream(resourcePath))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }
    }
}
