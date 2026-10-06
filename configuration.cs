using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Microsoft.SharePoint.Client;

using System.IO;
using System.Xml.Serialization;

namespace CamulosSharePointUpload
{
    class Configuration
    {
        public static string o365SiteURL = "";
        public static string coreSiteURL = "";
        public static string o365UserName = "";
        public static string o365Password = "";
        public static DateTime dt = new DateTime(1900, 1, 1);
        public static string o365List = "";
        public static string localSource = "";
        public static string remoteSourceList = "";
        public static string remoteSourceSubSite = "";
        public static string initialdir = "";
        public static bool confirm = true;
        public static string listGUID = "";
        public static Boolean overwrite = false;
        public static string[] badfiles = { "desktop.ini", "thumbs.db"};
        public static string[] badfolders = { };

        public static string logFileName = "Log.txt";
        public static string errorFileName = "Errors.txt";
        public static string errorFileTooLongFileName = "FileTooLongError.txt";
        public static string errorUnknownAuthor = "ErrorUnknownAuthor.txt";
        public static string excludeFileName = "";
        public static string excludeFolderFileName = "";
        public static string translateUsernameFile = "";
        public static List<string> badfileRegex;
        public static List<string> badfolderRegex;
        public static string[] tusernames = { };
        public static string[] tusernamesreplace = { };
        public static string startfrom = "";
        public static Boolean resumed = true;
        public static int timeoutValue = 0;
        public static string batchid = "";
        public static string sourcePSList = "";
        public static string sourceListGUID = "";
        public static string sourceSPList_DocLibraryFieldName = "documentLibrary";
        public static string sourceSPList_SubSiteLocationFieldName = "subsiteLocation";
        public static string sourceSPList_SourceLocation = "localsourceLocation";
        public static bool cleanLog = false;
        public static string o365subsite = "";
        public static string metadatafoldersfieldlist = "";
        public static string[] metadatafolderfields = { };
        public static bool metadatatofoldermode = false;
        public static bool skiproot = true;
        public static int runmode = 0;
        public static int custom = 0;
        public static string customexe = "";
        public static int skipfoldercreate = 0;
        public static int limitdepth = 0;
        public static string csvfile = "";
        public static bool setEditDate = false;
        public static bool setAuthor = false;
        public static bool createsiteifnotexists = false;
        
        


        public static string migrationsdatafile = ""; 
        public static MigrationsDatabase Migrationdb;
        public static MigrationsDatabase LoadMigrationDatabase(string FileName)
        {
            if (System.IO.File.Exists(FileName))
            {
                using (var stream = System.IO.File.OpenRead(FileName))
                {
                    var serializer = new XmlSerializer(typeof(MigrationsDatabase));
                    return serializer.Deserialize(stream) as MigrationsDatabase;
                }
            }
            else
            {
                return new MigrationsDatabase();
            }

        }

        public static ClientContext GetUserContext()
        {
            var o365Password = new SecureString();
                foreach(char c in Configuration.o365Password)
            {
                o365Password.AppendChar(c);
            }
            var o365Credentials = new SharePointOnlineCredentials(Configuration.o365UserName, o365Password);
            string url = "";
            if (Configuration.o365subsite.Length > 0)
            {
                url = Configuration.o365SiteURL.Substring(0, (Configuration.o365SiteURL.Length - Configuration.o365subsite.Length));

            }else
            {
                url = Configuration.o365SiteURL;
            }
            var o365Context = new ClientContext(Configuration.o365SiteURL);
            //var o365Context = new ClientContext(url);
            o365Context.Credentials = o365Credentials;
            return o365Context;
        }

        public static ClientContext GetUserContext(string siteURL)
        {
            var o365Password = new SecureString();
            foreach (char c in Configuration.o365Password)
            {
                o365Password.AppendChar(c);
            }
            var o365Credentials = new SharePointOnlineCredentials(Configuration.o365UserName, o365Password);
            var o365Context = new ClientContext(siteURL);
            o365Context.Credentials = o365Credentials;
            return o365Context;
        }
       
    }
}
