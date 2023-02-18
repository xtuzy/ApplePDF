using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace PDFiumCore
{

    /// <summary>
    /// Flags:
    /// 1 - Incremental
    /// 2 - NoIncremental
    /// 3 - RemoveSecurity.
    /// </summary>
    internal class fpdfsave
    {
        /// <summary>
        /// <para>Function: FPDF_SaveAsCopy</para>
        /// <para>Saves the copy of specified document in custom way.</para>
        /// <para>Parameters:</para>
        /// <para>document        -   Handle to document, as returned by FPDF_LoadDocument() or FPDF_CreateNewDocument().</para>
        /// <para>pFileWrite      -   A pointer to a custom file write structure.</para>
        /// <para>flags           -   The creating flags.</para>
        /// <para>Return value:</para>
        /// <para>TRUE for succeed, FALSE for failed.</para>
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        [DllImport(Setting.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FPDF_SaveAsCopy")]
        internal static extern int FPDF_SaveAsCopy(IntPtr document, FpdfStreamWriter pFileWrite, uint flags);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Setting.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FPDF_SaveWithVersion")]
        private static extern int FPDF_SaveWithVersion(IntPtr document, FpdfStreamWriter pFileWrite, uint flags, int fileVersion);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate bool StreamWriteHandler(IntPtr writerPtr, IntPtr data, int size);

        [StructLayout(LayoutKind.Sequential)]
        internal class FpdfStreamWriter
        {
            public int version = 1;

            [MarshalAs(UnmanagedType.FunctionPtr)]
            public readonly StreamWriteHandler Handler;

            public FpdfStreamWriter(StreamWriteHandler handler)
            {
                Handler = handler;
            }
        }

        public static bool FPDF_SaveAsCopy(FpdfDocumentT document, Stream stream, PdfSaveFlag saveFlag = PdfSaveFlag.NoIncremental)
        {
            byte[] buffer = null;

            var fileWrite = new FpdfStreamWriter((writerPtr, data, size) =>
            {
                if (buffer == null || buffer.Length < size)
                {
                    buffer = new byte[size];
                }

                Marshal.Copy(data, buffer, 0, size);

                stream.Write(buffer, 0, size);

                return true;
            });

            var result = FPDF_SaveAsCopy(document.__Instance, fileWrite, (uint)saveFlag);

            GC.KeepAlive(fileWrite);

            return result == 1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class CustomFPDF_FILEWRITE
        {
            public int version;
            public IntPtr WriteBlock;
            public IntPtr stream;
        }
    }

    /// <summary>
    /// CHANGED,模仿PdfiumViewer项目从流读取
    /// </summary>
    public partial class fpdfview
    {
        public partial struct __Internal
        {
            /// <summary>
            /// <para>Function: FPDF_LoadCustomDocument</para>
            /// <para>Load PDF document from a custom access descriptor.</para>
            /// <para>Parameters:</para>
            /// <para>pFileAccess -   A structure for accessing the file.</para>
            /// <para>password    -   Optional password for decrypting the PDF file.</para>
            /// <para>Return value:</para>
            /// <para>A handle to the loaded document, or NULL on failure.</para>
            /// <para>Comments: The application must keep the file resources |pFileAccess| points to valid until the returned FPDF_DOCUMENT is closed. |pFileAccess| itself does not need to outlive the FPDF_DOCUMENT.</para>
            /// <para>The loaded document can be closed with FPDF_CloseDocument().</para>
            /// <para>See the comments for FPDF_LoadDocument() regarding the encoding for |password|.</para>
            /// <para>Notes: If PDFium is built with the XFA module, the application should call FPDF_LoadXFA() function after the PDF document loaded to support XFA fields defined in the fpdfformfill.h file.</para>
            /// </summary>
            [SuppressUnmanagedCodeSecurity]
            [DllImport(Setting.DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "FPDF_LoadCustomDocument")]
            internal static extern IntPtr FPDF_LoadStreamDocument([MarshalAs(UnmanagedType.LPStruct)] CustomFPDF_FILEACCESS pFileAccess, [MarshalAs(UnmanagedType.LPStr)] string password);
        }

        public static FpdfDocumentT FPDF_LoadDocument(Stream stream, string password, int id)
        {
            var getBlock = Marshal.GetFunctionPointerForDelegate(_getBlockDelegate);
            var access = new CustomFPDF_FILEACCESS
            {
                m_FileLen = (uint)stream.Length,
                m_GetBlock = getBlock,
                m_Param = (IntPtr)id
            };
            //var __arg0 = ReferenceEquals(pFileAccess, null) ? IntPtr.Zero : pFileAccess.__Instance;
            var __ret = __Internal.FPDF_LoadStreamDocument(access, password);
            FpdfDocumentT __result0;
            if (__ret == IntPtr.Zero)
                __result0 = null;
            else if (FpdfDocumentT.NativeToManagedMap.ContainsKey(__ret))
                __result0 = (FpdfDocumentT)FpdfDocumentT
                    .NativeToManagedMap[__ret];
            else
                __result0 = FpdfDocumentT.__CreateInstance(__ret);
            return __result0;
        }

        /// <summary>
        /// CppSharp自动转换的不能读取文档
        /// https://github.com/pvginkel/PdfiumViewer/blob/master/PdfiumViewer/NativeMethods.Pdfium.cs
        /// </summary>
        /// <param name="param"></param>
        /// <param name="position"></param>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int FPDF_GetBlockDelegate(IntPtr param, uint position, IntPtr buffer, uint size);

        private static readonly FPDF_GetBlockDelegate _getBlockDelegate = FPDF_GetBlock;

        /// <summary>
        /// CHANGED
        /// </summary>
        /// <param name="param"></param>
        /// <param name="position"></param>
        /// <param name="buffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static int FPDF_GetBlock(IntPtr param, uint position, IntPtr buffer, uint size)
        {
            var stream = StreamManager.Get((int)param);
            if (stream == null)
                return 0;
            byte[] managedBuffer = new byte[size];

            stream.Position = position;
            int read = stream.Read(managedBuffer, 0, (int)size);
            if (read != size)
                return 0;

            Marshal.Copy(managedBuffer, 0, buffer, (int)size);
            return 1;
        }

        [StructLayout(LayoutKind.Sequential)]
        public class CustomFPDF_FILEACCESS
        {
            public uint m_FileLen;
            public IntPtr m_GetBlock;
            public IntPtr m_Param;
        }

    }

    public partial class fpdf_annot
    {
        /// <summary>
        /// 连续内存
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Size = 8)]
        public struct FS_POINTF_Fix
        {
            public float X;
            public float Y;
        }

        /// <summary>
        /// see <see cref="FPDFAnnotAddInkStroke(FpdfAnnotationT, FS_POINTF_, ulong)"/>
        /// </summary>
        /// <param name="annot"></param>
        /// <param name="points"></param>
        /// <param name="point_count"></param>
        /// <returns></returns>
        public static int FPDFAnnotAddInkStrokeTryFix(global::PDFiumCore.FpdfAnnotationT annot, ref FS_POINTF_Fix[] points, ulong point_count)
        {
            var __arg0 = annot is null ? IntPtr.Zero : annot.__Instance;
            //var __arg1 = points is null ? __IntPtr.Zero : points.__Instance;
            GCHandle pinned = GCHandle.Alloc(points, GCHandleType.Pinned);
            IntPtr __arg1 = pinned.AddrOfPinnedObject();
            var __ret = __Internal.FPDFAnnotAddInkStroke(__arg0, __arg1, point_count);
            pinned.Free();
            return __ret;
        }

        public static uint FPDFAnnotGetInkListPathTryFix(global::PDFiumCore.FpdfAnnotationT annot, uint path_index, ref FS_POINTF_Fix[] points, uint length)
        {
            var __arg0 = annot is null ? IntPtr.Zero : annot.__Instance;
            //var __arg2 = buffer is null ? __IntPtr.Zero : buffer.__Instance;
            GCHandle pinned = GCHandle.Alloc(points, GCHandleType.Pinned);
            IntPtr __arg2 = pinned.AddrOfPinnedObject();
            var __ret = __Internal.FPDFAnnotGetInkListPath(__arg0, path_index, __arg2, length);
            pinned.Free();
            return __ret;
        }
    }
}
