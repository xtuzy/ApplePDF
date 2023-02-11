using ApplePDF.PdfKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplePDF.ApplePdfKit.Test.Tests
{
    internal static class PdfSaveExtension
    {
        /// <summary>
        /// 选择一个合适的平台文件夹存储，能在设备上查看
        /// </summary>
        /// <param name="fullName"></param>
        internal static void Save(ILib lib, PdfDocument doc , string fullName)
        {
#if Android || IOS
            var basisFolder = Yang.Maui.Helper.File.CommonFolder.AppPublicPersistentDataFolder;
#else
            var basisFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
#endif
            var finalFolderPath = Path.Combine(basisFolder, "test_result");
            if(!Directory.Exists(finalFolderPath))
                Directory.CreateDirectory(finalFolderPath);
            var filePath = Path.Combine(finalFolderPath, fullName);
            if(!File.Exists(filePath))
                File.Create(filePath).Dispose();
            lib.Save(doc , filePath);
        }
    }
}
