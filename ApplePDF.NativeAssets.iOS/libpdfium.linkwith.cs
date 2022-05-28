using System;
using System.Collections.Generic;
using System.Linq;
using System;
using ObjCRuntime;

[assembly: LinkWith("libpdfium.a", LinkTarget.Simulator64 | LinkTarget.ArmV7 | LinkTarget.Arm64, SmartLink = true, ForceLoad = true)]
