using Appccelerate.EventBroker;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Vatsim.Vatis.Client.Atis;
using Vatsim.Vatis.Client.AudioForVatsim;
using Vatsim.Vatis.Client.Common;
using Vatsim.Vatis.Client.Config;
using Vatsim.Vatis.Client.Core;
using Vatsim.Vatis.Client.TextToSpeech;

namespace Vatsim.Vatis.Client
{
    static class Program
    {
        private static Container Container;
        private static IAppConfig AppConfig;
        private static string AppPath;

        [STAThread]
        static void Main()
        {
            if (!SingleAppInstance.Exists("vatis4.0"))
            {
                Application.CurrentCulture = new CultureInfo("en-US");
                Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

                AppPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

                Container = new Container();

                AutoRegisterWindowsForms(Container);

                Container.RegisterSingleton<IUserInterface, UserInterfaceFactory>();
                Container.RegisterSingleton<IEventBroker>(() => new EventBroker());
                Container.RegisterSingleton<IAppConfig, AppConfig>();
                Container.RegisterSingleton<IVersionCheck, VersionCheck>();
                Container.RegisterSingleton<INavaidDatabase, NavaidDatabase>();
                Container.RegisterSingleton<IAudioManager, AudioManager>();
                Container.RegisterSingleton<ITextToSpeechRequest, TextToSpeechRequest>();
                Container.RegisterSingleton<IAtisBuilder, AtisBuilder>();

                Container.Verify();

                AppConfig = Container.GetInstance<IAppConfig>();

                Application.Run(Container.GetInstance<ProfileList>());
            }
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                using (var fs = new FileStream(Path.Combine(AppPath, "AppExceptions.txt"), FileMode.Append, FileAccess.Write, FileShare.None, 1024, false))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(Assembly.GetEntryAssembly().FullName);
                        sw.WriteLine("============================================================");
                        sw.WriteLine("Culture     : " + CultureInfo.CurrentCulture.Name);
                        sw.WriteLine("OS          : " + Environment.OSVersion.ToString());
                        sw.WriteLine("Framework   : " + Environment.Version);
                        sw.WriteLine("Time        : " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"));
                        sw.WriteLine("------------------------------------------------------------");
                        sw.WriteLine("Details: " + ex.ToString());
                        sw.WriteLine("============================================================");
                        sw.Flush();
                        MessageBox.Show(null, "Unhandled exception: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }
                }
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            AppConfig.SaveConfig();
        }

        private static void AutoRegisterWindowsForms(Container container)
        {
            var formTypes = container.GetTypesToRegister<Form>(typeof(Program).Assembly);

            foreach (var type in formTypes)
            {
                if (type.IsDefined(typeof(IgnoreFormRegistration)))
                    continue;

                var registration = Lifestyle.Transient.CreateRegistration(type, container);

                registration.SuppressDiagnosticWarning(
                    DiagnosticType.DisposableTransientComponent,
                    "Forms should be disposed by app code; not by the container.");

                container.AddRegistration(type, registration);
            }
        }
    }
}
