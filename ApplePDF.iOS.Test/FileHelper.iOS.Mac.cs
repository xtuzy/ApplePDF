﻿#if __IOS__ || __MACOS__
using Foundation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
#if __IOS__
using UIKit;
#else
using AppKit;
#endif
namespace Helper.Files
{
    public static partial class FileHelper
    {
        /// <summary>
        /// 从Resources获取文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static FileStream FromResources(string name)
        {
            var path = NSBundle.MainBundle.PathForResource(name, null);
            return File.Open(path, FileMode.Open);
        }
    }
}
#endif