using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using HockeyApp;
using System.Threading.Tasks;
using System.IO;

namespace HockeyAppSampleiOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		const string HOCKEYAPP_APPID = "YOUR-HOCKEYAPP-APP-ID";

		UINavigationController navController;
		HomeViewController homeViewController;
		UIWindow window;

		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			
			InitHockeyApp();
			
			window = new UIWindow (UIScreen.MainScreen.Bounds);

			homeViewController = new HomeViewController ();
			navController = new UINavigationController (homeViewController);
			window.RootViewController = navController;
			
			// make the window visible
			window.MakeKeyAndVisible ();

			return true;
		}
		
		
		public class CustomCrashDelegate:BITCrashManagerDelegate
		{
			//Called at the next restart after a crash, the content of the file will be visible 
			//in the HockeyApp dashboard under the "Description" tab
			public override string ApplicationLogForCrashManager (BITCrashManager crashManager)
			{
				return File.ReadAllText("temp.log");
			}
		}
		
		private void InitHockeyApp()
		{
			Setup.EnableCustomCrashReporting (() => {

				//Get the shared instance
				var manager = BITHockeyManager.SharedHockeyManager;

				//Configure it to use our APP_ID
				manager.Configure (HOCKEYAPP_APPID);
				CustomCrashDelegate crashDelegate = new CustomCrashDelegate();
				
				manager.CrashManager.Delegate = crashDelegate;
				manager.CrashManager.CrashManagerStatus = BITCrashManagerStatus.AutoSend;
				
				//Start the manager
				manager.StartManager ();

				
				
				//Authenticate (there are other authentication options)
				manager.Authenticator.AuthenticateInstallation ();

				//Rethrow any unhandled .NET exceptions as native iOS 
				// exceptions so the stack traces appear nicely in HockeyApp
				AppDomain.CurrentDomain.UnhandledException += (sender, e) => 
					Setup.ThrowExceptionAsNative(e.ExceptionObject);

				TaskScheduler.UnobservedTaskException += (sender, e) => 
					Setup.ThrowExceptionAsNative(e.Exception);
			});
		}
		
		
	}
}

