using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace nuT3  //LoggingSample
{
    // https://www.codeproject.com/articles/21338/a-c-central-logging-mechanism-using-the-observer-a
    class FileLogger: ILogger
    {
        #region Data
        private string mFileName;
        private StreamWriter mLogFile;

        public string FileName
        {
            get { return mFileName; }
        }
        #endregion

        #region Constructor
        public FileLogger(string fileName)
        {
            mFileName = fileName;
        }
        #endregion

        #region Public methods
        public void Init()
        {
            mLogFile = new StreamWriter(mFileName);
        }

        public void Terminate()
        {
            mLogFile.Close();
        }
        #endregion

        #region ILogger Members

        public void ProcessLogMessage(string logMessage)
        {
            // FileLogger implements the ProcessLogMessage method by writing the incoming
            // message to a file.
            mLogFile.WriteLine(logMessage);
        }
        #endregion
    }
}
