using BuildSqliteCF.Entity;
using GalaSoft.MvvmLight.Threading;
//using LoggerLib;
using nuT3;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Reflection;
using System.Threading;

namespace Planner
{
    public partial class App : Application
    {
        public Mutex mutex;  // reference unique Mutex

        Logger mLogger;
        FileLogger mFileLogger;

        public string dbFileName = "TDTDb.sqlite";
        public string logDirectory = "Temp\\TDTnew";

        public UserControl priorView = null;
        public TDTDbContext db;

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            mLogger.AddLogMessage("App_DispatcherUnhandledException");
            // Want WPF to handle in default way
            e.Handled = false;
        }

        void DispatcherUnhandledExceptionHandler(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            mLogger.AddLogMessage("<==== DispatcherUnhandledExceptionHandle ===>");
            mLogger.AddLogMessage(args.Exception.Message);
            mLogger.AddLogMessage(args.Exception.InnerException.Message);
            mLogger.AddLogMessage(args.Exception.StackTrace);
            args.Handled = true;
            // implement recovery
            // execution will now continue...
        }

        public App()
        {
            InitializeComponent();  // Added per StackOverFlow
            DispatcherHelper.Initialize();
            //   db = new TDTDbContext();

            // SingleInstanceCheck ---------------------------------------------
            var appName = Assembly.GetEntryAssembly().GetName().Name;
            var notAlreadyRunning = true;
            //using (var mutex = new Mutex(true, appName + "Singleton", out notAlreadyRunning))
            mutex = new Mutex(true, appName + "Singleton", out notAlreadyRunning);
            //   Should have initial ownership, mutex name,  was caller granted ownership
            if (notAlreadyRunning)
            {
                // got control of Mutex => nothing running, we are first

                //       System.AppDomain currentDomain = System.AppDomain.CurrentDomain;
                //       currentDomain.UnhandledException += new System.UnhandledExceptionEventHandler(currentDomain_UnhandledException);
                // http://stackoverflow.com/questions/4625825/catching-exceptions-in-wpf-at-the-framework-level

                    this.Properties["dbFile"] = dbFileName;  // need access in views for dbContext
                    this.Properties["dbFile2"] = dbFileName;  // need access in views for dbContext

                    GetLogDirectory(logDirectory);

                AppSpecificSetUp();
                //     GetData();  // get data all in memory at beginning.
            }
            else
            {
                //Already running.");
                Environment.Exit(0);
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(
                    CultureInfo.CurrentCulture.IetfLanguageTag)));
            Application.Current.DispatcherUnhandledException +=
                new DispatcherUnhandledExceptionEventHandler(DispatcherUnhandledExceptionHandler);


            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            int n = e.ApplicationExitCode;
            if (n > 1)
                mLogger.AddLogMessage("EXIT CODE was " + n);
            mLogger.AddLogMessage("<==== Exiting OnExit-App ===>");
            mFileLogger.Terminate();
            base.OnExit(e);
        }

        protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
        {
            mLogger.AddLogMessage("<==== Exiting OnSessionEnding-App ===>");
            mFileLogger.Terminate();
            base.OnSessionEnding(e);
        }

        #region Logging
        private void SetUpLogging(string logDirectory)
        {
            mLogger = Logger.Instance;
            string logName0 = System.IO.Path.Combine(logDirectory, "logTDTnew_" + System.DateTime.Now.ToString("yyyyMMdd_HH_mm_ss") + ".log");

            mFileLogger = new FileLogger(logName0);
            mFileLogger.Init();
            mLogger.RegisterObserver(mFileLogger);
            mLogger.AddLogMessage("logName0->" + logName0);
        }
        #endregion Logging
    }
}
