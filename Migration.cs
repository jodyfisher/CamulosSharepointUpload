using System;
namespace CamulosSharePointUpload
{
    public class Migration
    {
        private string _Reference = "";
        private string _SharepointSite = "";
        private string _Username = "";
        private string _Password = "";
        private string _DocLibraryName = "";
        private string _DocLibraryGUiD = "";
        private string _DocSource = "";
        private string _StartFolder = "";
        private string _Resume = "";
        private string _Timeout = "";
        private string _LogFileName = "";
        private string _ErrorFileName = "";
        private string _ExcludeFiles = "";
        private string _ExcludeFolders = "";
        private string _ID = "";

        public string Reference
        {
            get { return _Reference; }
            set { _Reference = value; }
        }
        public string SharepointSite
        {
            get { return _SharepointSite; }
            set { _SharepointSite = value; }
        }
        public string Username
        {
            get { return _Username; }
            set { _Username = value; }
        }
        public string Password
        {
            get { return _Password; }
            set { _Password = value; }
        }
        public string DocLibraryName
        {
            get { return _DocLibraryName; }
            set { _DocLibraryName = value; }
        }
        public string DocLibraryGUiD
        {
            get { return _DocLibraryGUiD; }
            set { _DocLibraryGUiD = value; }
        }
        public string DocSource
        {
            get { return _DocSource; }
            set { _DocSource = value; }
        }
        public string StartFolder
        {
            get { return _StartFolder; }
            set { _StartFolder = value; }
        }
        public string Resume
        {
            get { return _Resume; }
            set { _Resume = value; }
        }
        public string Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        public string LogFileName
        {
            get { return _LogFileName; }
            set { _LogFileName = value; }
        }
        public string ErrorFileName
        {
            get { return _ErrorFileName; }
            set { _ErrorFileName = value; }
        }
        public string ExcludeFiles
        {
            get { return _ExcludeFiles; }
            set { _ExcludeFiles = value; }
        }
        public string ExcludeFolders
        {
            get { return _ExcludeFolders; }
            set { _ExcludeFolders = value; }
        }


    }
}
