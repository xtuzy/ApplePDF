using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace PDFiumCore
{
    /// <summary>
    /// 从流读取
    /// </summary>
    public unsafe partial class fpdfview
    {
        //外部调用
        public static FpdfDocumentT FPDF_LoadCustomDocument(Stream stream, string password,int id)
        {
            var getBlock = _getBlockDelegate;
            //https://github.com/pvginkel/PdfiumViewer/blob/master/PdfiumViewer/NativeMethods.Pdfium.cs
            var access = new FPDF_FILEACCESS
            {
                MFileLen = (uint)stream.Length,
                MGetBlock = getBlock,
                MParam = (IntPtr)id
            };
            FpdfDocumentT __result0 = FPDF_LoadCustomDocument(access, password);
            return __result0;
        }
        private static readonly unsafe Delegates.Func_int___IntPtr_uint_bytePtr_uint _getBlockDelegate = FPDF_GetBlock;

        /// <summary>
        /// CHANGED
        /// </summary>
        /// <param name="param"></param>
        /// <param name="position"></param>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static unsafe int FPDF_GetBlock(IntPtr param, uint position, byte* buffer, uint size)
        {
            var stream = StreamManager.Get((int)param);
            if (stream == null)
                return 0;
            byte[] managedBuffer = new byte[size];

            stream.Position = position;
            int read = stream.Read(managedBuffer, 0, (int)size);
            if (read != size)
                return 0;
            
            Marshal.Copy(managedBuffer, 0, new IntPtr(buffer), (int)size);
            return 1;
        }
    }
}
