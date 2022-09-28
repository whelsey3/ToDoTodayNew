using System;
using System.IO;
using System.Windows;
using System.Linq;
using BuildSqliteCF.Entity;

namespace Planner
{
    public partial class App : Application
    {
        private void AppSpecificSetUp()
        {
            SetUpDataStorage();   // sets App.Current.Properties["destFilePath"] 
            App.Current.Properties["dbContext"] = new TDTDbContext();
            //    db = new TDTDbContext();
            db = (TDTDbContext)App.Current.Properties["dbContext"];

            mLogger.AddLogMessage("<=== Checking AdHoc Folder ID ===>");

            int? theAdHocFolderID = CheckAdHoc();
            App.Current.Properties["AdHocFolderID"] = theAdHocFolderID;

            int? theRoutineTasksFolderID = CheckRoutineTasks();
            App.Current.Properties["RoutineTasksFolderID"] = theRoutineTasksFolderID;

            App.Current.Properties["priorView"] = null;
            var theProperties = App.Current.Properties;
            var r = App.Current.Properties["RoutineTasksFolderID"];
        }

        #region AdHoc
        private int CheckAdHoc()
        {
            string theFileName = (string)App.Current.Properties["destFilePath"];
            mLogger.AddLogMessage("Initial context setup using ->" + theFileName);

            //var theConnectString = new SQLiteConnection()
            //{
            //    //ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "D:\\Databases\\SQLiteWithEF.db", ForeignKeys = true }.ConnectionString
            //    ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "C:\\Users\\be3so\\AppData\\Local\\TDT\\TDTDb.sqlite", ForeignKeys = true }.ConnectionString
            //};
            //string ConnectionString0 = new SQLiteConnectionStringBuilder() { DataSource = "C:\\Users\\be3so\\AppData\\Local\\TDT\\TDTDb.sqlite", ForeignKeys = true }.ConnectionString;

            int adHocFolderID = 0;
            //       TDTDbContext db = (TDTDbContext)App.Current.Properties["theData"];
            try
            {
                adHocFolderID = (from c in db.Folders
                                 where c.FolderName == "AdHoc"
                                 select c.FolderID).FirstOrDefault();

                if (adHocFolderID == 0)
                {
                    // Need to create AdHoc Folder
                    Folder newAdHocFolder = AddAdHocFolder();
                    db.Folders.Add(newAdHocFolder);
                    int n = db.SaveChanges();
                    newAdHocFolder.FolderID = newAdHocFolder.FolderID;
                    adHocFolderID = newAdHocFolder.FolderID;
                }
            }
            catch (System.Exception e)
            {
                //adHocFolderID = 0;
                if (e.InnerException != null)
                {
                    mLogger.AddLogMessage("CheckAdHoc Exception: " + e.InnerException.GetBaseException().ToString());
                }
                else
                {
                    mLogger.AddLogMessage("CheckAdHoc Exception: " + e.Message.ToString());
                }
                mLogger.AddLogMessage("Problem in CheckAddHoc!!");
                App.Current.Shutdown();
            }
            finally
            {
                mLogger.AddLogMessage("AdHocFolderID is " + adHocFolderID);
            }
            return adHocFolderID;
        }

        private Folder AddAdHocFolder()
        {
            Folder newFolder = new Folder("AdHoc");
            newFolder.FPartNum = "987000000";
            newFolder.FSortOrder = "987000000";
            newFolder.DetailedDesc = "Projects handled as AdHocs.";
            //newFolder.RespPerson = "ELS";
            newFolder.Hide = false;
            newFolder.DispLevel = "1";

            newFolder.DateModified = DateTime.Now;
            if (newFolder.DateCreated == DateTime.MinValue)
            {
                newFolder.DateCreated = DateTime.Now;
            }

            return newFolder;
        }
        #endregion AdHoc

        #region RoutineTasks
        private int CheckRoutineTasks()
        {
            string theFileName = (string)App.Current.Properties["destFilePath"];
            mLogger.AddLogMessage("Initial context setup using ->" + theFileName);

            //var theConnectString = new SQLiteConnection()
            //{
            //    //ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "D:\\Databases\\SQLiteWithEF.db", ForeignKeys = true }.ConnectionString
            //    ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = "C:\\Users\\be3so\\AppData\\Local\\TDT\\TDTDb.sqlite", ForeignKeys = true }.ConnectionString
            //};
            //string ConnectionString0 = new SQLiteConnectionStringBuilder() { DataSource = "C:\\Users\\be3so\\AppData\\Local\\TDT\\TDTDb.sqlite", ForeignKeys = true }.ConnectionString;

            int RoutineTasksFolderID = 0;
            //       TDTDbContext db = (TDTDbContext)App.Current.Properties["theData"];
            try
            {
                RoutineTasksFolderID = (from c in db.Folders
                                        where c.FolderName == "Routine Tasks"
                                        select c.FolderID).FirstOrDefault();

                if (RoutineTasksFolderID == 0)
                {
                    // Need to create RoutineTasks Folder
                    Folder newRoutineTasksFolder = AddRoutineTasksFolder();
                    db.Folders.Add(newRoutineTasksFolder);
                    int n = db.SaveChanges();
                    newRoutineTasksFolder.FolderID = newRoutineTasksFolder.FolderID;
                    RoutineTasksFolderID = newRoutineTasksFolder.FolderID;
                }
            }
            catch (System.Exception e)
            {
                //RoutineTasksFolderID = 0;
                if (e.InnerException != null)
                {
                    mLogger.AddLogMessage("CheckRoutineTasks Exception: " + e.InnerException.GetBaseException().ToString());
                }
                else
                {
                    mLogger.AddLogMessage("CheckRoutineTasks Exception: " + e.Message.ToString());
                }
                mLogger.AddLogMessage("Problem in CheckRoutineTasks!!");
                App.Current.Shutdown();
            }
            finally
            {
                mLogger.AddLogMessage("RoutineTasksFolderID is " + RoutineTasksFolderID);
            }
            return RoutineTasksFolderID;
        }

        private Folder AddRoutineTasksFolder()
        {
            Folder newFolder = new Folder("Routine Tasks");
            newFolder.FPartNum = "980000000";
            newFolder.FSortOrder = "980000000";
            newFolder.DetailedDesc = "Projects handled as Routine Taskss.";
            //newFolder.RespPerson = "ELS";
            newFolder.Hide = false;
            newFolder.DispLevel = "1";

            newFolder.DateModified = DateTime.Now;
            if (newFolder.DateCreated == DateTime.MinValue)
            {
                newFolder.DateCreated = DateTime.Now;
            }

            return newFolder;
        }
        #endregion RoutineTasks

        public string destFilePath = "";

        private void GetLogDirectory(string logDirectory)
        {
            // Change to use AppData for UWP requirement
            //string localDrive = Path.GetPathRoot(Environment.CurrentDirectory);
            string localDrive = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            logDirectory = Path.Combine(localDrive, logDirectory);
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);  // C:\Users\be3so\AppData\Local\Temp\TDTnew

            SetUpLogging(logDirectory);

            mLogger.AddLogMessage("Logging running! ++++++++++++++++++++++++++++");
        }

        private void SetUpDataStorage()
        {
            ////string DataFile = @"BillWork.db";
            string DataFile = (string)App.Current.Properties["dbFile"];

            // Data directory for data file ========================
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string userFilePath = Path.Combine(localAppData, "TDT");
            mLogger.AddLogMessage("userFilePath-> " + userFilePath);
            // Check for data directory and create if necessary
            if (!Directory.Exists(userFilePath))
                Directory.CreateDirectory(userFilePath);

           // string dd = "|DataDirectory|";
            //  Getting source of data file (should be in same dir as exe)
            string fullExeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            //  "file:///C:/Projects/Working/ToDoToday/bin/Debug/T3.exe"
            //    fullExeName.Replace(@"file:///", "");
            fullExeName = fullExeName.Substring(8, fullExeName.Length - 8);
            string exeDirectory = System.IO.Path.GetDirectoryName(fullExeName);

            string sourceFilePath = Path.Combine(System.Windows.Forms.Application.StartupPath, DataFile);

            string actualDataDir = Path.Combine(destFilePath, DataFile);
            destFilePath = Path.Combine(userFilePath, DataFile);  // "BillWork2.db");
                                                                  //if (!File.Exists(Path.Combine(destFilePath, DataFile)))
            App.Current.Properties["destFilePath"] = destFilePath;
            if (!File.Exists(destFilePath))
            {
                mLogger.AddLogMessage("New deployment of Data file."); 
                //copy the file from the deployment location to the folder
                mLogger.AddLogMessage("sourceFilePath-> " + sourceFilePath);
                mLogger.AddLogMessage("destFilePath-> " + destFilePath);
                //   if (!File.Exists(destFilePath))
                File.Copy(sourceFilePath, destFilePath);
                // Check if new version of program
            }
            else
            {
                mLogger.AddLogMessage(" Data File already in place");
            }
            mLogger.AddLogMessage("----------- End SetUpDataStorage ---------------");

            LogVersion(fullExeName);
        }

        private void LogVersion(string ExeName)
        {
            string fullExeName = ExeName;
            //string fullExeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase;
            //fullExeName.Replace("file:///", "");
            System.IO.FileInfo fi = new FileInfo(fullExeName);

            string BuildDate = "Planner" + string.Format("  -  (Build Timestamp: {0})", fi.LastWriteTimeUtc.ToString("MM-dd-yyyy HH:mm"));
            string AppName = "     Version:           " + typeof(Planner.ShellView).Assembly.GetName().Version.ToString();
            string SuiteVersion2 = "  CodeFirst Library Version: " + typeof(BuildSqliteCF.Entity.Folder).Assembly.GetName().Version.ToString();
            string SuiteVersion3 = "  Logging Library Version:   " + typeof(LoggerLib.Logger).Assembly.GetName().Version.ToString();
            string cDataSource = System.Configuration.ConfigurationManager.ConnectionStrings["TDTDbContext"].ConnectionString;
            var cDataSource0 = System.Configuration.ConfigurationManager.ConnectionStrings["TDTDbContext"];
            var cProviderName = "     ProviderName: " + System.Configuration.ConfigurationManager.ConnectionStrings["TDTDbContext"].ProviderName;
            var cName = "     Name:         " + System.Configuration.ConfigurationManager.ConnectionStrings["TDTDbContext"].Name;
            mLogger.AddLogMessage("========================");
            mLogger.AddLogMessage(BuildDate);
            mLogger.AddLogMessage(fullExeName);
            mLogger.AddLogMessage(AppName);
            mLogger.AddLogMessage(SuiteVersion2);
            mLogger.AddLogMessage(SuiteVersion3);
            mLogger.AddLogMessage(cDataSource);
            mLogger.AddLogMessage(cName);
            mLogger.AddLogMessage(cProviderName);
            //mLogger.AddLogMessage(fi.DirectoryName);
            mLogger.AddLogMessage("========================");

        }

    }
}
