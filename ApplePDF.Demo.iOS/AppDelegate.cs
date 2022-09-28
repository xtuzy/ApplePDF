using System.Runtime.InteropServices;
using System.Security;

namespace ApplePDF.Demo.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow? Window
        {
            get;
            set;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // create a new window instance based on the screen size
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            // create a UIViewController with a single UILabel
            var vc = new UIViewController();
            vc.View!.AddSubview(new UILabel(Window!.Frame)
            {
                BackgroundColor = UIColor.SystemBackground,
                TextAlignment = UITextAlignment.Center,
                Text = "Hello, iOS!",
                AutoresizingMask = UIViewAutoresizing.All,
            });
            Window.RootViewController = vc;
            try
            {
                FPDF_InitLibrary();
            }
            catch (DllNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            // make the window visible
            Window.MakeKeyAndVisible();

            return true;
        }
        [SuppressUnmanagedCodeSecurity, DllImport("__Internal", EntryPoint = "FPDF_InitLibrary", CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern void FPDF_InitLibrary();
    }
}