using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using System.IO;
using System.Text.RegularExpressions;
using camulosTools;
using System.Data.OleDb;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using static System.Net.WebRequestMethods;
using System.Security.AccessControl;
using System.IO.Pipes;
using System.Security.Principal;
using System.Reflection;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;

// Example usage: -source "/srv/documents" -user "user@example.com" -password "<password>" -site "https://example.sharepoint.com" -list "Shared Documents" -confirm "no" -overwrite "no"

namespace CamulosSharePointUpload
{
    class Program
    {

        private const string curVersion = "1";
        private const string licAgreement = "https://apps.camulos.com.au/licensing/showlicense/fitos";
        private const int camulosAppId = 4;

        //private Microsoft.Office.Interop.Word.Application wordObject = null;
        //private Microsoft.Office.Interop.Excel.Application excelObject = null;
        //private Microsoft.Office.Interop.PowerPoint.Application pptObject = null;


        static void Main(string[] args)
        {
            string cmd = "";
            string site = "";
            string data = "";
            string filename = "";
            Boolean usingHelp = args.Length == 0;

            Configuration.batchid = Guid.NewGuid().ToString("N");

            foreach (string x in args)
            {
                if (cmd == "")
                {
                    cmd = x;
                    switch (cmd.ToLower())
                    {
                        case "/?":
                            usingHelp = true;
                            break;
                        case "/help":
                            usingHelp = true;
                            break;
                        case "-?":
                            usingHelp = true;
                            break;
                        case "-help":
                            usingHelp = true;
                            break;
                        case "help":
                            usingHelp = true;
                            break;
                        case "?":
                            usingHelp = true;
                            break;
                        case "/edit":
                        case "-edit":
                        case "/editor":
                        case "-editor":
                            Console.Error.WriteLine("The graphical editor has been removed. Use terminal arguments or -mode config -configfile <path>.");
                            Environment.ExitCode = 1;
                            return;
                    }
                }
                else
                {
                    data = x;
                    switch (cmd.ToLower())
                    {
                        case "-site":
                            site = data;
                            cmd = "";
                            break;
                        case "/site":
                            site = data;
                            cmd = "";
                            break;

                        case "/?":
                            usingHelp = true;
                            cmd = "";
                            break;

                        case "-?":
                            usingHelp = true;
                            cmd = "";
                            break;
                        case "-help":
                            usingHelp = true;
                            cmd = "";
                            break;
                        case "/help":
                            usingHelp = true;
                            cmd = "";
                            break;
                        case "/mode":
                        case "-mode":
                            switch (data.ToLowerInvariant())
                            {
                                case "0":
                                    Configuration.runmode = 0;
                                    break;
                                case "1":
                                case "configfile":
                                case "config":
                                    Configuration.runmode = 1;
                                    break;
                                default:
                                    Console.Error.WriteLine("Unsupported mode. Use 0 for terminal arguments or 1/config/configfile for a configuration file. Graphical editor modes have been removed.");
                                    Environment.ExitCode = 1;
                                    return;
                            }
                            break;
                        case "-configfile":
                            filename = data;
                            break;
                        case "/configfile":
                            filename = data;
                            break;

                    }
                    cmd = "";
                }
            }
            if (usingHelp)
            {
                Configuration.runmode = 0;
            }

            if (Configuration.runmode == 1)
            {
                if (string.IsNullOrWhiteSpace(filename) || !System.IO.File.Exists(filename))
                {
                    Console.Error.WriteLine("Specify an existing configuration file with -mode config -configfile <path>.");
                    Environment.ExitCode = 1;
                    return;
                }
                runfromconfigfile(filename);
                return;
            }

            if (Configuration.runmode == 0)
            {
                /// standard run mode fro migrations

                Console.WriteLine("~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*");
                Console.WriteLine(" Camulos Uploader - File to SharePoint (FiToS). ");
                Console.WriteLine(" Version: " + curVersion);
                Console.WriteLine("~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*");
                Console.WriteLine(" ");

                if (usingHelp == false)
                {

                    string[] sited;
                    Boolean validurl = false;
                    string errormsg = "";
                    try
                    {
                        if (site.IndexOf("http") > 0)
                        {
                            validurl = false;
                            errormsg = "there is no https:// at the start of the site you have specified, please include it";
                        }
                        if (site.IndexOf("/") > 0)
                        {
                            validurl = true;
                            sited = site.Split("/".ToCharArray());
                            site = string.Format("{0}//{1}", sited[0], sited[2]);
                        }
                        else
                        {
                            validurl = false;
                            errormsg = "something strange about your site, is it a valid sharepoint site?  eg https://mycompany.sharepoint.com";
                        }
                    }
                    catch (Exception ex)
                    {
                        validurl = false;
                        errormsg = "Something didn't work - there is something strange about your site, is it a valid sharepoint site?  eg https://mycompany.sharepoint.com, did you include the https://?";
                    }

                    if (validurl)
                    {
                        //Console.WriteLine("checking for license for: " + site + "...");

                        camulosLicensing l = new camulosLicensing();
                        camulosLicensing.license li = new camulosLicensing.license();

                        //li = l.checkLicense(site, camulosAppId);
                        li.currentLicense = true;
                        li.licenseType = 1;
                        //Console.WriteLine(li.message);

                        if (li.currentLicense)
                        {
                            if (li.licenseType == 1)
                            {
                                Console.WriteLine("Site license found for: " + site + " - No expiry");
                            }
                            else
                            {
                                Console.WriteLine("Site license found for: " + site + " - Expires: " + li.licenseExpiry);
                            }
                            /// license is current so off we go.
                            go(args);
                        }
                        else
                        {
                            Console.Read();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error 1001: not a valid sharepoint site:" + site);
                        Console.WriteLine(errormsg);
                        Console.Read();
                    }
                }
                else
                {
                    Console.WriteLine("~~~~~Usage:~~~~~~~");
                    Console.WriteLine("Terminal only: use upload arguments below, or -mode config -configfile <path> for an existing configuration file.");
                    Console.WriteLine("-user:       Required. Office 365/sharepoint Username for the sharepoint site.");
                    Console.WriteLine("-password:   Required. Office 365/sharepoint Password for the sharepoint site.");
                    Console.WriteLine("-site:       Required. site url eg https://yoursite.sharepoint.com");
                    Console.WriteLine("-list:       Required. Name of the target List in sharepoint - should be the root of the folder structure - eg Shared Documents");
                    Console.WriteLine("-source:     Required. location of documents on your local computer/network to be copied - ie c:\\documentsforupload");
                    Console.WriteLine("-listguid:   Optional. GUID of list");
                    Console.WriteLine("-initialdir: Optional. Initial directory ie listname / initial directory - this option enables you to target folders within a list");
                    Console.WriteLine("-confirm:    Optional. default is yes. yes/y or no/n - yes means confirms start and finish - user intervention required");
                    Console.WriteLine("-overwrite:  Optional. default is no. yes/y or no/n - yes mean you want to overwrite files already on the site, no means that you do not want to overwrite any files");
                    Console.WriteLine("-dt:         Optional. format is 0000-00-00 this option is used for incremental uploads - if you specific the date, it will ignore any files older and not upload them");
                    Console.WriteLine("-log:        Optional. specify the name of the log file.  eg logfile1.txt.  If left blank it will create a file in the same directory as exe with the dateTime and Log.txt eg 201807011011Log.txt");
                    Console.WriteLine("-elog:       Optional. specify the name of the Errors log file.  eg Errorsfile1.txt.  If left blank it will create a file in the same directory as exe with the dateTime and Error.txt eg 201807011011Error.txt");
                    Console.WriteLine("-eftlog:     Optional. specify the name of the File to long errors log file.  eg ErrorsFileNameTooLong.txt.  If left blank it will create a file in the same directory as exe with the dateTime and ErrorsFileNameTooLong.txt eg 201807011011ErrorsFileNameTooLong.txt");
                    Console.WriteLine("-excfile:    Optional. specify the name of the File containing names of files to exclude.  eg exclusions.txt.  If left blank it will exclude desktop.ini and thumbs.db only.  Contents of the file must be a new filename on each line, should not include path only name of file.  You add filenames or valid regex if using regex, put a [re] in front of the entry eg [re]\\.(txt|doc|pdf) - this would exclude all files with extensions txt, doc or pdf");
                    Console.WriteLine("-excfilefolders: Optional. Specify the name of a file containing names of folders to exclude.  eg folderexclusions.txt.  if left blank it will not exclude and folders. file should contain a folder on each new line.");
                    Console.WriteLine("-resume:     Optional. specify a folder path to resume from.  eg -rusume \"c:\\documentsforUpload\\folder220\"");
                    Console.WriteLine("-timeout:    Optional. specify the timeout value for http post request to sharepoint");
                    Console.WriteLine("-cleanlog:    Optional. default is no. yes/y or no/n - yes means start with clean log file by deleting previous content.  Log.txt only and does not affect the errors log or the filetolong log");
                    Console.WriteLine("-seteditdate:    Optional. default is no. yes/y or no/n - if set to yes will attempt to set the file create and edit dates - note this has overhead and will slow things down");

                    Console.WriteLine("-sourcesplist:						Optional.specify a SharePoint list to use for targetsiteurl, localSourceLocation.these must be the names of the fields.the targetsiteurl must use the same authentication as provided to get to this list.");
                    Console.WriteLine("-createsiteifnotexists:						    Optional. if using sourcesplist, if there is a value in targetsiteurl field and sub site does not exist, set this to 'yes' to create it if not exists..");
                    Console.WriteLine("-splistdoclibraryfieldname:			Optional. default value is \"documentLibrary\". if this field does not exist or has not data in it, the - list flag above is used. this is used in conjunction with sourcesplist and defines the name of the field being used for the document library name on the target subsite.");
                    Console.WriteLine("-splistsubsitelocationfieldname:	    Optional. default value is \"subsiteLocation\".used in conjunction with sourcesplist and defines the location of the target subsite.Data in this field must be full site location eg https://mycompany.sharepoint.com/subsite.");
                    Console.WriteLine("-splistlocalsourcelocationfieldname: Optional. default value is \"localsourceLocation\".used in conjunction with sourcesplist and defines the location of the target subsite.Data in this field must be full site location eg https://mycompany.sharepoint.com/subsite.");
                    //Console.WriteLine("-configurationfile:    Optional. specify a config file in xml format");
                    Console.WriteLine("-metadatafoldersfieldlist: Optional. IF used with sourcesplist, this will enable the metadata to folder migration process.");
                    Console.WriteLine("-skiproot:                 Optional. Default is Yes,  yes/y or no/n,  IF used with metadatafoldersfieldlist, stop files being copied to the root folder.");


                    Console.WriteLine("~~~~~~~~~~~~~~~~~");
                    Console.WriteLine("Press any key to exit");
                    Console.Read();
                }
            }
        }

        public static void go(string[] args)
        {
            // GetSiteLists();

            string cmd = "";
            string data = "";
            string[] dx;
            DateTime dt;

            foreach (string x in args)
            {
                if (cmd == "")
                {
                    cmd = x;
                }
                else
                {
                    data = x;
                    switch (cmd.ToLower())
                    {
                        case "-confirm":
                            if (data.ToLower() == "y") { Configuration.confirm = true; }
                            if (data.ToLower() == "yes") { Configuration.confirm = true; }
                            if (data.ToLower() == "n") { Configuration.confirm = false; }
                            if (data.ToLower() == "no") { Configuration.confirm = false; }
                            cmd = "";
                            break;
                        case "/confirm":
                            if (data.ToLower() == "y") { Configuration.confirm = true; }
                            if (data.ToLower() == "yes") { Configuration.confirm = true; }
                            if (data.ToLower() == "n") { Configuration.confirm = false; }
                            if (data.ToLower() == "no") { Configuration.confirm = false; }

                            cmd = "";
                            break;

                        case "-cleanlog":
                            if (data.ToLower() == "y") { Configuration.cleanLog = true; }
                            if (data.ToLower() == "yes") { Configuration.cleanLog = true; }
                            if (data.ToLower() == "n") { Configuration.cleanLog = false; }
                            if (data.ToLower() == "no") { Configuration.cleanLog = false; }
                            cmd = "";
                            break;
                        case "/cleanlog":
                            if (data.ToLower() == "y") { Configuration.cleanLog = true; }
                            if (data.ToLower() == "yes") { Configuration.cleanLog = true; }
                            if (data.ToLower() == "n") { Configuration.cleanLog = false; }
                            if (data.ToLower() == "no") { Configuration.cleanLog = false; }

                            cmd = "";
                            break;

                        case "-seteditdate":
                            if (data.ToLower() == "y") { Configuration.setEditDate = true; }
                            if (data.ToLower() == "yes") { Configuration.setEditDate = true; }
                            if (data.ToLower() == "n") { Configuration.setEditDate = false; }
                            if (data.ToLower() == "no") { Configuration.setEditDate = false; }
                            cmd = "";
                            break;
                        case "/seteditdate":
                            if (data.ToLower() == "y") { Configuration.setEditDate = true; }
                            if (data.ToLower() == "yes") { Configuration.setEditDate = true; }
                            if (data.ToLower() == "n") { Configuration.setEditDate = false; }
                            if (data.ToLower() == "no") { Configuration.setEditDate = false; }

                            cmd = "";
                            break;
                        case "-setauthor":
                            if (data.ToLower() == "y") { Configuration.setAuthor = true; }
                            if (data.ToLower() == "yes") { Configuration.setAuthor = true; }
                            if (data.ToLower() == "n") { Configuration.setAuthor = false; }
                            if (data.ToLower() == "no") { Configuration.setAuthor = false; }
                            cmd = "";
                            break;
                        case "/setauthor":
                            if (data.ToLower() == "y") { Configuration.setAuthor = true; }
                            if (data.ToLower() == "yes") { Configuration.setAuthor = true; }
                            if (data.ToLower() == "n") { Configuration.setAuthor = false; }
                            if (data.ToLower() == "no") { Configuration.setAuthor = false; }

                            cmd = "";
                            break;
                        case "-initialdir":
                            Configuration.initialdir = data;
                            cmd = "";
                            break;
                        case "/initialdir":
                            Configuration.initialdir = data;
                            cmd = "";
                            break;
                        //sourcesplist
                        case "-sourcesplist":
                            Configuration.sourcePSList = data;
                            cmd = "";
                            break;
                        case "/sourcesplist":
                            Configuration.sourcePSList = data;
                            cmd = "";
                            break;
                        case "-sourcedoclist":
                            Configuration.remoteSourceList = data;
                            cmd = "";
                            break;
                        case "/sourcedoclist":
                            Configuration.remoteSourceList = data;
                            cmd = "";
                            break;
                        case "-sourcesubsite":
                            Configuration.remoteSourceSubSite = data;
                            cmd = "";
                            break;
                        case "/sourcesubsite":
                            Configuration.remoteSourceSubSite = data;
                            cmd = "";
                            break;
                        case "-translateunamefile":
                            Configuration.translateUsernameFile = data;
                            cmd = "";
                            break;
                        case "/translateunamefile":
                            Configuration.translateUsernameFile = data;
                            cmd = "";
                            break;
                        case "-splistdoclibraryfieldname":
                            Configuration.sourceSPList_DocLibraryFieldName = data;
                            cmd = "";
                            break;
                        case "/splistdoclibraryfieldname":
                            Configuration.sourceSPList_DocLibraryFieldName = data;
                            cmd = "";
                            break;
                        case "-splistsubsitelocationfieldname":
                            Configuration.sourceSPList_SubSiteLocationFieldName = data;
                            cmd = "";
                            break;
                        case "/splistsubsitelocationfieldname":
                            Configuration.sourceSPList_SubSiteLocationFieldName = data;
                            cmd = "";
                            break;
                        case "-splistlocalsourcelocationfieldname":
                            Configuration.sourceSPList_SourceLocation = data;
                            cmd = "";
                            break;
                        case "/splistlocalsourcelocationfieldname":
                            Configuration.sourceSPList_SourceLocation = data;
                            cmd = "";
                            break;
                        case "/createsiteifnotexists":
                            if (data.ToLower() == "y") { Configuration.createsiteifnotexists = true; }
                            if (data.ToLower() == "yes") { Configuration.createsiteifnotexists = true; }
                            if (data.ToLower() == "n") { Configuration.createsiteifnotexists = false; }
                            if (data.ToLower() == "no") { Configuration.createsiteifnotexists = false; }
                            cmd = "";
                            break;
                        case "-createsiteifnotexists":
                            if (data.ToLower() == "y") { Configuration.createsiteifnotexists = true; }
                            if (data.ToLower() == "yes") { Configuration.createsiteifnotexists = true; }
                            if (data.ToLower() == "n") { Configuration.createsiteifnotexists = false; }
                            if (data.ToLower() == "no") { Configuration.createsiteifnotexists = false; }
                            cmd = "";
                            break;
                        case "-timeout":
                            Configuration.timeoutValue = int.Parse(data);
                            cmd = "";
                            break;
                        case "/timeout":
                            Configuration.timeoutValue = int.Parse(data);
                            cmd = "";
                            break;
                        case "-skiproot":
                            if (data.ToLower() == "y") { Configuration.skiproot = true; }
                           if (data.ToLower() == "yes") { Configuration.skiproot = true; }
                           if (data.ToLower() == "n") { Configuration.skiproot = false; }
                           if (data.ToLower() == "no") { Configuration.skiproot = false; }
                           cmd = "";
                           break;
                       case "/skiproot":
                            if (data.ToLower() == "y") { Configuration.skiproot = true; }
                           if (data.ToLower() == "yes") { Configuration.skiproot = true; }
                           if (data.ToLower() == "n") { Configuration.skiproot = false; }
                           if (data.ToLower() == "no") { Configuration.skiproot = false; }
                           cmd = "";
                           break;
                       case "-metadatafoldersfieldlist":
                           Configuration.metadatafoldersfieldlist = data;
                           cmd = "";
                           break;
                       case "/metadatafoldersfieldlist":
                           Configuration.metadatafoldersfieldlist = data;
                           cmd = "";
                           break;
                        case "-listguid":
                            Configuration.listGUID = data;
                            cmd = "";
                            break;
                        case "/listguid":
                            Configuration.listGUID = data;
                            cmd = "";
                            break;
                        case "-user":
                            Configuration.o365UserName = data;
                            cmd = "";
                            break;
                        case "/user":
                            Configuration.o365UserName = data;
                            cmd = "";
                            break;
                        case "-username":
                            Configuration.o365UserName = data;
                            cmd = "";
                            break;
                        case "/username":
                            Configuration.o365UserName = data;
                            cmd = "";
                            break;
                        case "-p":
                            Configuration.o365Password = data;
                            cmd = "";
                            break;
                        case "/p":
                            Configuration.o365Password = data;
                            cmd = "";
                            break;
                        case "-pass":
                            Configuration.o365Password = data;
                            cmd = "";
                            break;
                        case "/pass":
                            Configuration.o365Password = data;
                            cmd = "";
                            break;
                        case "-password":
                            Configuration.o365Password = data;
                            cmd = "";
                            break;
                        case "/password":
                            Configuration.o365Password = data;
                            cmd = "";
                            break;
                        case "-s":
                            Configuration.o365SiteURL = data;
                            cmd = "";
                            break;
                        case "/s":
                            Configuration.o365SiteURL = data;
                            cmd = "";
                            break;
                        case "-site":
                            Configuration.o365SiteURL = data;
                            cmd = "";
                            break;
                        case "/site":
                            Configuration.o365SiteURL = data;
                            cmd = "";
                            break;
                        case "-url":
                            Configuration.o365SiteURL = data;
                            cmd = "";
                            break;
                        case "/url":
                            Configuration.o365SiteURL = data;
                            cmd = "";
                            break;
                        case "-list":
                            Configuration.o365List = data;
                            cmd = "";
                            break;
                        case "/list":
                            Configuration.o365List = data;
                            cmd = "";
                            break;
                        case "-source":
                            Configuration.localSource = data;
                            cmd = "";
                            break;
                        case "/source":
                            Configuration.localSource = data;
                            cmd = "";
                            break;

                        case "-log":
                            Configuration.logFileName = data;
                            cmd = "";
                            break;
                        case "/log":
                            Configuration.logFileName = data;
                            cmd = "";
                            break;
                        case "-elog":
                            Configuration.errorFileName = data;
                            cmd = "";
                            break;
                        case "/elog":
                            Configuration.errorFileName = data;
                            cmd = "";
                            break;
                        case "-eftlog":
                            Configuration.errorFileTooLongFileName = data;
                            cmd = "";
                            break;
                        case "/eftlog":
                            Configuration.errorFileTooLongFileName = data;
                            cmd = "";
                            break;

                        case "-excfile":
                            Configuration.excludeFileName = data;
                            cmd = "";
                            break;
                        case "/excfile":
                            Configuration.excludeFileName = data;
                            cmd = "";
                            break;

                        case "-excfilefolders":
                            Configuration.excludeFolderFileName = data;
                            cmd = "";
                            break;
                        case "/excfilefolders":
                            Configuration.excludeFolderFileName = data;
                            cmd = "";
                            break;

                        case "-resume":
                            Configuration.startfrom = data;
                            Configuration.resumed = false;
                            cmd = "";
                            break;
                        case "/resume":
                            Configuration.startfrom = data;
                            Configuration.resumed = false;
                            cmd = "";
                            break;
                        case "-skipfoldercreate":
                            Configuration.skipfoldercreate = int.Parse(data);
                            cmd = "";
                            break;
                        case "/skipfoldercreate":
                            Configuration.skipfoldercreate = int.Parse(data);
                            cmd = "";
                            break;
                        case "-limitdepth":
                            Configuration.limitdepth = int.Parse(data);
                            cmd = "";
                            break;
                        case "/limitdepth":
                            Configuration.limitdepth = int.Parse(data);
                            cmd = "";
                            break;
                        case "-custom":
                            Configuration.customexe = data;
                            Configuration.custom = 1;
                            cmd = "";
                            break;
                        case "/custom":
                            Configuration.customexe = data;
                            Configuration.custom = 1;
                            cmd = "";
                            break;

                        case "-overwrite":
                            if (data.ToLower() == "y") { Configuration.overwrite = true; }
                            if (data.ToLower() == "yes") { Configuration.overwrite = true; }
                            if (data.ToLower() == "n") { Configuration.overwrite = false; }
                            if (data.ToLower() == "no") { Configuration.overwrite = false; }
                            cmd = "";
                            break;
                        case "/overwrite":
                            if (data.ToLower() == "y") { Configuration.overwrite = true; }
                            if (data.ToLower() == "yes") { Configuration.overwrite = true; }
                            if (data.ToLower() == "n") { Configuration.overwrite = false; }
                            if (data.ToLower() == "no") { Configuration.overwrite = false; }
                            cmd = "";
                            break;
                        case "-dt":
                            try
                            {
                                dx = data.Split(new string[] { "-" }, StringSplitOptions.None);
                                dt = new DateTime(int.Parse(dx[0]), int.Parse(dx[1]), int.Parse(dx[2]));
                                Configuration.dt = dt;
                            }
                            catch (Exception ex) { }
                            cmd = "";
                            break;
                        case "/dt":
                            try
                            {
                                dx = data.Split(new string[] { "-" }, StringSplitOptions.None);
                                dt = new DateTime(int.Parse(dx[0]), int.Parse(dx[1]), int.Parse(dx[2]));
                                Configuration.dt = dt;
                            }
                            catch (Exception ex) { }
                            cmd = "";
                            break;
                        case "-date":
                            try
                            {
                                dx = data.Split(new string[] { "-" }, StringSplitOptions.None);
                                dt = new DateTime(int.Parse(dx[0]), int.Parse(dx[1]), int.Parse(dx[2]));
                                Configuration.dt = dt;
                            }
                            catch (Exception ex) { }
                            cmd = "";
                            break;
                        case "/date":
                            try
                            {
                                dx = data.Split(new string[] { "-" }, StringSplitOptions.None);
                                dt = new DateTime(int.Parse(dx[0]), int.Parse(dx[1]), int.Parse(dx[3]));
                                Configuration.dt = dt;
                            }
                            catch (Exception ex) { }
                            cmd = "";
                            break;

                        case "/csvfile":
                            Configuration.csvfile = data;
                            cmd = "";
                            break;
                        case "-csvfile":
                            Configuration.csvfile= data;
                            cmd = "";
                            break;


                    }
                    cmd = "";


                }

            }

           


            if (Configuration.localSource.Length == 2)
            {
                if (Configuration.localSource.Substring(1) == ":")
                {
                    Configuration.localSource = Configuration.localSource + "\\";
                }
            }


            if (Configuration.excludeFileName.Length > 0)
            {
                checkgetExclusionFile();
            }

            if (Configuration.excludeFolderFileName.Length > 0)
            {
                checkgetExclusionFileFolders();
            }

            string[] urlstring = Configuration.o365SiteURL.Split(new string[] { "/" }, StringSplitOptions.None);
            int y = 0;
            string middle = "";
            if (urlstring.Length > 3)
            {
                for (y = 3; y < urlstring.Length; y++)
                {
                    Configuration.o365subsite = Configuration.o365subsite + middle + urlstring[y];
                    middle = "/";
                }
                middle = "";
                for (y = 0; y < 3; y++)
                {
                    Configuration.coreSiteURL = Configuration.coreSiteURL + middle + urlstring[y];
                    middle = "/";
                }

            }
            else
            {
                Configuration.coreSiteURL = Configuration.o365SiteURL;
            }



            Console.WriteLine("--------------------------");
            Console.WriteLine("Copying to sharepoint");
            Console.WriteLine("created by Camulos Consulting, Copywrite Camulos Consulting");
            Console.WriteLine("By using this software application you are agreeing to the License agreement.  License Agreement can be accessed here: " + licAgreement);
            Console.WriteLine("login credentials = user: {0}, password length: {1}", Configuration.o365UserName, Configuration.o365Password.Length);



            if (Configuration.sourcePSList.Length > 0)
            {
                if (Configuration.metadatatofoldermode)
               {
                   Console.WriteLine("Copying to site: {0}, List: {1}, initial Directory {2}", Configuration.o365SiteURL, Configuration.o365List, Configuration.initialdir);
                   Console.WriteLine("Using sharepoint source list ({0}) and converting METADATA to Folders ", Configuration.sourcePSList);
                   Console.WriteLine("Fields to be used ( {0} ) ", Configuration.metadatafoldersfieldlist);
                   if (Configuration.skiproot)
                       Console.WriteLine("Skipping Root Folder");
                   else
                       Console.WriteLine("Copying files into root folder if there is no meta data");
                   if (Configuration.sourceListGUID.Length > 0)
                       Console.WriteLine("Source List GUID {0}", Configuration.sourceListGUID);
               }
               else
               {


                   Console.WriteLine("Connecting to site: {0}", Configuration.o365SiteURL);
                    if (Configuration.o365subsite.Length > 0)
                    {
                        Console.WriteLine("Subsite: {0}", Configuration.o365subsite);
                    }
                    Console.WriteLine("Using sharepoint list ( {0} ) to define source ", Configuration.sourcePSList);

                    Console.WriteLine("Sharepoint list - Library Name sourced from field: {0} OR from -list flag if doesn't exist or empty ({1})", Configuration.sourceSPList_DocLibraryFieldName, Configuration.o365List);
                    Console.WriteLine("Sharepoint list - Target Subsite location sourced from field: {0}", Configuration.sourceSPList_SubSiteLocationFieldName);
                    Console.WriteLine("Sharepoint list - Local Source location sourced from field: {0}", Configuration.sourceSPList_SourceLocation);
                }

            }
            else if (Configuration.remoteSourceList.Length > 0)
            {
                Console.WriteLine("Copying from site: {0} Subsite: {1}, List: {2}, initial Directory {3}", Configuration.coreSiteURL,Configuration.remoteSourceSubSite, Configuration.remoteSourceList, Configuration.localSource);
                Console.WriteLine("Copying to site: {0}, List: {1}, initial Directory {2}", Configuration.o365SiteURL, Configuration.o365List, Configuration.initialdir);
                Console.WriteLine("From: {0}", Configuration.localSource);
            }else if (Configuration.custom > 0)
            {
                Console.WriteLine("Connecting to site: {0}", Configuration.o365SiteURL);
                if (Configuration.o365subsite.Length > 0)
                {
                    Console.WriteLine("Subsite: {0}", Configuration.o365subsite);
                }
                Console.WriteLine("Running Custom Script: {0} ", Configuration.customexe);
            }
           else
            {

                Console.WriteLine("Copying to site: {0}, List: {1}, initial Directory {2}", Configuration.o365SiteURL, Configuration.o365List, Configuration.initialdir);
                Console.WriteLine("From: {0}", Configuration.localSource);
            }
            Console.WriteLine("Overwrite existing files: {0}", Configuration.overwrite);
            Console.WriteLine("Keeping Source Create and Edit Dates: {0}", Configuration.setEditDate);
            Console.WriteLine("General Log File: {0}", Configuration.logFileName);
            Console.WriteLine("Error Log File: {0}", Configuration.errorFileName);
            Console.WriteLine("File To Long Error Log File: {0}", Configuration.errorFileTooLongFileName);
            if (Configuration.skipfoldercreate > 0) {
                Console.WriteLine("Skipping Folder Create {0}", Configuration.skipfoldercreate);
            }
            if (Configuration.limitdepth > 0)
            {
                Console.WriteLine("Limiting Depth {0}", Configuration.limitdepth);
            }

            if (Configuration.resumed == false)
           {
                Console.WriteLine("resuming from: {0}", Configuration.startfrom);
            }
            if (Configuration.timeoutValue != 0)
            {
                Console.WriteLine("upload timeout set to: {0}", Configuration.timeoutValue);
            }
            if (Configuration.excludeFileName.Length > 0)
            {
               Console.WriteLine("Using exclusions file: {0}", Configuration.excludeFileName);
            }

            if (Configuration.dt == new DateTime(1900, 1, 1))
            {
                Console.WriteLine("Not using dates");
            }
            else
            {
                Console.WriteLine("Only files newer than {0} will be copied across ", Configuration.dt);
            }
            Console.WriteLine("");

            Console.WriteLine("--------------------------");
            Console.WriteLine(" ");
            bool goodforgo = false;

            string s = "";
            if (Configuration.confirm)
            {
                Console.WriteLine("Please confirm Yes (y) or No (n) to continue");
                s = Console.ReadLine();
            }
            else
            {
                goodforgo = true;
            }

            if (s.ToLower() == "y")
            {
                goodforgo = true;
            }
            else if (s.ToLower() == "yes")
            {
                goodforgo = true;
            }

            if (Configuration.sourcePSList.Length == 0)
            {
                if (Configuration.remoteSourceList.Length == 0)
                {
                    if (Configuration.custom == 0)
                    {
                        if (!System.IO.Directory.Exists(Configuration.localSource))
                        {
                            goodforgo = false;
                            Console.WriteLine("ERROR 1002: The source folder does not exist: " + Configuration.localSource);
                        }
                    }
                }
            }
            if (Configuration.translateUsernameFile.Length > 0)
            {
                getAuthorTranslateFile();
            }


            if (goodforgo == true)
            {
                ClientContext o365context = Configuration.GetUserContext();
                if (Configuration.timeoutValue != 0)
                {

                    o365context.RequestTimeout = Configuration.timeoutValue;

                }

                string initdir = Configuration.initialdir;
                try
                {

                    if (initdir.Substring(0, 1) == "\\") { initdir = initdir.Substring(1); }
                    if (initdir.Substring(0, 1) == "/") { initdir = initdir.Substring(1); }
                }
                catch (Exception ex) { }
                Configuration.initialdir = initdir;

                if (Configuration.cleanLog)
                {
                    try
                    {
                        if (System.IO.File.Exists(Configuration.logFileName))
                        {
                            System.IO.File.Delete(Configuration.logFileName);
                        }
                    }
                    catch (Exception ex) { }
                }

                


                if (Configuration.sourcePSList.Length > 0)
                {
                    if (Configuration.metadatafoldersfieldlist.Length > 0)
                    {
                        String targ = Configuration.o365subsite;
                        if (targ.Length > 0 )
                        {
                            targ += "/";
                        }
                        targ += Configuration.o365List;
                        if (Configuration.initialdir.Length > 0)
                        {
                            targ += "/" + Configuration.initialdir;
                        }
                        useMetadataToFolderModeCordner(o365context, targ);
                    }
                    else
                    {
                        useSPList(o365context);
                    }
                }
                else if (Configuration.remoteSourceList.Length > 0)
                {


                    
                    string listd = "";
                    if (Configuration.o365subsite.Length > 0)
                    {
                        //// need to test this to see why it is set this way for subsites
                        listd = Configuration.o365subsite + "/" + Configuration.o365List;
                        ///temporarily overriding
                        //listd = Configuration.o365List;
                    }
                    else
                    {
                        listd = Configuration.o365List;
                    }

                    string xfolder = "";
                    string pfolder = "";
                    if (Configuration.initialdir.Length > 0)
                    {
                        if (Configuration.initialdir.Contains("/"))
                        {
                            string[] flds = Configuration.initialdir.Split('/');
                            string relpath = "";
                            for (int i = 0; i < flds.Length; i++)
                            {
                                string thisfolder = sanitiseFileName(flds[i]);
                                string curfolder = thisfolder;

                                if (!FolderExists(o365context, listd + relpath + "/" + thisfolder))
                                {
                                    curfolder = CreateFolder(o365context, Configuration.o365SiteURL, Configuration.o365List, relpath, thisfolder, Configuration.listGUID);
                                    o365context.ExecuteQuery();
                                }
                                relpath = relpath + "/" + thisfolder;
                                if (i == (flds.Length - 1))
                                {
                                    xfolder = flds[i];
                                }
                                else
                                {
                                    pfolder = pfolder + "/" + flds[i];
                                }
                                
                            }
                            //listd = listd + "/" + Configuration.initialdir;
                            

                        }
                        else
                        {
                            string thisfolder = sanitiseFileName(Configuration.initialdir);
                            xfolder = Configuration.initialdir;
                            pfolder = "";
                               
                            if (!FolderExists(o365context, listd  + "/" + thisfolder))
                            {
                                string curfolder = CreateFolder(o365context, Configuration.o365SiteURL, Configuration.o365List,"", thisfolder, Configuration.listGUID);
                                o365context.ExecuteQuery();
                            }
                            
                        }
                    }




                    string lists = Configuration.remoteSourceList;
                    if (Configuration.remoteSourceSubSite.Length > 0)
                    {
                        //// need to test this to see why it is set this way for subsites
                        lists = Configuration.remoteSourceSubSite + "/" + Configuration.remoteSourceList;

                    }
                    else
                    {
                        lists = Configuration.remoteSourceList;
                    }
                    if (Configuration.localSource.Length > 0)
                    {
                        /// this will be the start from folder.
                        lists = lists + "/" + Configuration.localSource;
                    }

                    if (Configuration.initialdir.Length > 0)
                    {
                        if (Configuration.initialdir.Contains("/"))
                        {

                        }
                        else
                        {

                          
                        }
                    }
                    goodforgo = true;
                    Microsoft.SharePoint.Client.Folder folder = null;
                    string sourcectxurl = Configuration.coreSiteURL;
                    if (Configuration.remoteSourceSubSite.Length > 0)
                    {
                        sourcectxurl = sourcectxurl + "/" + Configuration.remoteSourceSubSite;
                    }
                    Microsoft.SharePoint.Client.ClientContext sctx = Configuration.GetUserContext(sourcectxurl);
                    if (Configuration.timeoutValue != 0)
                    {

                        sctx.RequestTimeout = Configuration.timeoutValue;

                    }
                    try
                    {
                        
                        folder = sctx.Web.GetFolderByServerRelativeUrl("/" + lists);
                        
                        sctx.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        goodforgo = false;
                        Console.WriteLine(ex.Message);
                        logit("Error", "Error finding Source File:" + lists, "", ex.Message + "");
                        logError(lists, "", ex.Message + "");
                        Console.WriteLine(ex.Message + ":" + lists);
                    }

                    if (goodforgo)
                    {
                        

                        //doFolderWithSourceList(o365context, sctx, Configuration.localSource, Configuration.initialdir, "", listd, folder);
                        doFolderWithSourceList(o365context, sctx, Configuration.localSource, xfolder, pfolder, listd, folder);
                    }

                }

                else if (Configuration.custom == 1)
                {
                    if(Configuration.customexe == "cordnerdeleter")
                    {
                        string listd = "";
                        if (Configuration.o365subsite.Length > 0)
                        {
                            //// need to test this to see why it is set this way for subsites
                            listd = Configuration.o365subsite + "/" + Configuration.o365List;
                            ///temporarily overriding
                            //listd = Configuration.o365List;
                        }
                        else
                        {
                            listd = Configuration.o365List;
                        }
                        var folder = o365context.Web.GetFolderByServerRelativeUrl(Configuration.o365SiteURL + "/" + Configuration.o365List);
                        o365context.Load(folder.ListItemAllFields);
                        o365context.ExecuteQuery();
                        cordnergetFolderCustomRunOnce(o365context, listd, folder, 0, "","");

                    }else if (Configuration.customexe == "fromcsv")
                    {
                        string listd = "";
                        if (Configuration.o365subsite.Length > 0)
                        {
                            //// need to test this to see why it is set this way for subsites
                            listd = Configuration.o365subsite + "/" + Configuration.o365List;
                            ///temporarily overriding
                            //listd = Configuration.o365List;
                        }
                        else
                        {
                            listd = Configuration.o365List;
                        }
                        var folder = o365context.Web.GetFolderByServerRelativeUrl(Configuration.o365SiteURL + "/" + Configuration.o365List);
                        o365context.Load(folder.ListItemAllFields);
                        goodforgo = true;
                        try
                        {
                            o365context.ExecuteQuery();
                        }catch(Exception ex)
                        {
                            goodforgo = false;
                            logError("Connect to Site", "NA", "Something is wrong with your target or library, attempted to use getFolderByServerRelativeUrl(" + Configuration.o365SiteURL + "/" + Configuration.o365List + "). ERROR 101:" + ex.Message.ToString());
                            logit("Error","Connect to Site", Configuration.o365SiteURL + "/" + Configuration.o365List, "Something is wrong with your target or library, attempted to use getFolderByServerRelativeUrl(" + Configuration.o365SiteURL + "/" + Configuration.o365List + "). ERROR 101:" + ex.Message.ToString());
                            Console.WriteLine("Something is wrong with your target or library, attempted to use getFolderByServerRelativeUrl(" + Configuration.o365SiteURL + "/" + Configuration.o365List + "). ERROR 101:" + ex.Message.ToString());
                        }

                        if (goodforgo)
                        {
                            //string listd = "";
                            if (Configuration.o365subsite.Length > 0)
                            {
                                //// need to test this to see why it is set this way for subsites
                                listd = Configuration.o365subsite + "/" + Configuration.o365List;
                                ///temporarily overriding
                                //listd = Configuration.o365List;
                            }
                            else
                            {
                                listd = Configuration.o365List;
                            }

                            if (Configuration.initialdir.Length > 0)
                            {
                                if (Configuration.initialdir.Contains("/"))
                                {

                                }
                                else
                                {
                                    if (!FolderExists(o365context, Configuration.initialdir))
                                    {

                                        //curfolder = CreateFolder(o365Context, Configuration.o365SiteURL, o365DocumentList, o365ParentFolder, thisfolder, Configuration.listGUID);
                                    }
                                }
                            }
                            run365csv(o365context);
                        }

                    }
                }
                else
                {
                    string listd = "";
                    if (Configuration.o365subsite.Length > 0)
                    {
                        //// need to test this to see why it is set this way for subsites
                        listd = Configuration.o365subsite + "/" + Configuration.o365List;
                        ///temporarily overriding
                        //listd = Configuration.o365List;
                    }
                    else
                    {
                        listd = Configuration.o365List;
                    }

                    if (Configuration.initialdir.Length > 0)
                    {
                        if (Configuration.initialdir.Contains("/"))
                       {

                        }
                       else
                        {
                            if (!FolderExists(o365context, Configuration.initialdir))
                            {

                                //curfolder = CreateFolder(o365Context, Configuration.o365SiteURL, o365DocumentList, o365ParentFolder, thisfolder, Configuration.listGUID);
                            }
                        }
                    }

                    /*
                    if (Configuration.setAuthor == true)
                    {
                        UserCollection users = o365context.Web.SiteUsers;
                        o365context.Load(users);
                        // otherwise you can user collection and check if user exists of not.
                        
                        List userinfos = o365context.Web.SiteUserInfoList;
                        o365context.Load(userinfos);
                        try
                        {
                            o365context.ExecuteQuery();
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            Console.WriteLine("Unable to get site users");
                        }
                        
                    }*/
                    doFolder(o365context, Configuration.localSource, Configuration.initialdir, "", listd);
                }
            }
            //
            if (Configuration.confirm)
            {
                Console.WriteLine("complete - press any key to continue");
                Console.ReadLine();
            }
        }

        /*public static User getUser(string user)
        {
            User u = null;
            foreach(User x in Configuration.users)
            {
                
            }
            
        }*/

        public static void runfromconfigfile(string fle)
        {
            Configuration.migrationsdatafile = fle;
            Configuration.Migrationdb = Configuration.LoadMigrationDatabase(Configuration.migrationsdatafile);

            foreach (Migration m in Configuration.Migrationdb.Migrations)
            {
                Configuration.o365List = m.DocLibraryName;
                Configuration.o365Password = m.Password;
                Configuration.o365UserName = m.Username;
                Configuration.o365SiteURL = m.SharepointSite;
                Configuration.listGUID = m.DocLibraryGUiD;
                Configuration.localSource = m.DocSource;
                Configuration.startfrom = m.Resume;
                Configuration.excludeFileName = m.ExcludeFiles;
                Configuration.excludeFolderFileName = m.ExcludeFolders;
                //Configuration.timeoutValue = Int32.Parse(m.Timeout);
                Configuration.logFileName = m.LogFileName;
                Configuration.errorFileName = m.ErrorFileName;
                Program.StartMigrationFromForm();
            }
        }
        public static void StartMigrationFromForm()
        {
            if (Configuration.localSource.Length == 2)
            {
                if (Configuration.localSource.Substring(1) == ":")
                {
                    Configuration.localSource = Configuration.localSource + "\\";
                }
            }


            if (Configuration.excludeFileName.Length > 0)
            {
                checkgetExclusionFile();
            }

            if (Configuration.excludeFolderFileName.Length > 0)
            {
                checkgetExclusionFileFolders();
            }

            string[] urlstring = Configuration.o365SiteURL.Split(new string[] { "/" }, StringSplitOptions.None);
            int y = 0;
            string middle = "";
            if (urlstring.Length > 3)
            {
                for (y = 3; y < urlstring.Length; y++)
                {
                    Configuration.o365subsite = Configuration.o365subsite + middle + urlstring[y];
                    middle = "/";
                }
            }



            Console.WriteLine("--------------------------");
            Console.WriteLine("Copying to sharepoint");
            Console.WriteLine("created by Camulos Consulting, Copywrite Camulos Consulting");
            Console.WriteLine("By using this software application you are agreeing to the License agreement.  License Agreement can be accessed here: " + licAgreement);
            Console.WriteLine("login credentials = user: {0}, password Length: {1}", Configuration.o365UserName, Configuration.o365Password.Length);



            if (Configuration.sourcePSList.Length > 0)
            {
                if (Configuration.metadatatofoldermode)
                {
                    Console.WriteLine("Copying to site: {0}, List: {1}, initial Directory {2}", Configuration.o365SiteURL, Configuration.o365List, Configuration.initialdir);
                    Console.WriteLine("Using sharepoint source list ({0}) and converting METADATA to Folders ", Configuration.sourcePSList);
                    Console.WriteLine("Fields to be used ( {0} ) ", Configuration.metadatafoldersfieldlist);
                    if (Configuration.skiproot)
                        Console.WriteLine("Skipping Root Folder");
                    else
                        Console.WriteLine("Copying files into root folder if there is no meta data");
                    if (Configuration.sourceListGUID.Length > 0)
                        Console.WriteLine("Source List GUID {0}", Configuration.sourceListGUID);
                }
                else
                {


                    Console.WriteLine("Connecting to site: {0}", Configuration.o365SiteURL);
                    if (Configuration.o365subsite.Length > 0)
                    {
                        Console.WriteLine("Subsite: {0}", Configuration.o365subsite);
                    }
                    Console.WriteLine("Using sharepoint list ( {0} ) to define source ", Configuration.sourcePSList);

                    Console.WriteLine("Sharepoint list - Library Name sourced from field: {0} OR from -list flag if doesn't exist or empty ({1})", Configuration.sourceSPList_DocLibraryFieldName, Configuration.o365List);
                    Console.WriteLine("Sharepoint list - Target Subsite location sourced from field: {0}", Configuration.sourceSPList_SubSiteLocationFieldName);
                    Console.WriteLine("Sharepoint list - Local Source location sourced from field: {0}", Configuration.sourceSPList_SourceLocation);
                }

            }
            else if (Configuration.remoteSourceList.Length > 0)
            {
                Console.WriteLine("Copying from site: {0} Subsite{1}, List: {2}, initial Directory {3}", Configuration.o365SiteURL, Configuration.remoteSourceSubSite, Configuration.remoteSourceList, Configuration.localSource);
                Console.WriteLine("Copying to site: {0}, List: {1}, initial Directory {2}", Configuration.o365SiteURL, Configuration.o365List, Configuration.initialdir);
                Console.WriteLine("From: {0}", Configuration.localSource);
            }
            else
            {
                Console.WriteLine("Copying to site: {0}, List: {1}, initial Directory {2}", Configuration.o365SiteURL, Configuration.o365List, Configuration.initialdir);
                Console.WriteLine("From: {0}", Configuration.localSource);
            }
            Console.WriteLine("Overwrite existing files: {0}", Configuration.overwrite);
            Console.WriteLine("General Log File: {0}", Configuration.logFileName);
            Console.WriteLine("Error Log File: {0}", Configuration.errorFileName);
            Console.WriteLine("File To Long Error Log File: {0}", Configuration.errorFileTooLongFileName);
            if (Configuration.resumed == false)
            {
                Console.WriteLine("resuming from: {0}", Configuration.startfrom);
            }
            if (Configuration.timeoutValue != 0)
            {
                Console.WriteLine("upload timeout set to: {0}", Configuration.timeoutValue);
            }
            if (Configuration.excludeFileName.Length > 0)
            {
                Console.WriteLine("Using exclusions file: {0}", Configuration.excludeFileName);
            }

            if (Configuration.dt == new DateTime(1900, 1, 1))
            {
                Console.WriteLine("Not using dates");
            }
            else
            {
                Console.WriteLine("Only files newer than {0} will be copied across ", Configuration.dt);
            }
            Console.WriteLine("");

            Console.WriteLine("--------------------------");
            Console.WriteLine(" ");
            bool goodforgo = false;

            string s = "";
            if (Configuration.confirm)
            {
                Console.WriteLine("Please confirm Yes (y) or No (n) to continue");
                s = Console.ReadLine();
            }
            else
            {
                goodforgo = true;
            }

            if (s.ToLower() == "y")
            {
                goodforgo = true;
            }
            else if (s.ToLower() == "yes")
            {
                goodforgo = true;
            }

            if (Configuration.sourcePSList.Length == 0)
            {
                if (Configuration.remoteSourceList.Length == 0)
                {
                    if (!System.IO.Directory.Exists(Configuration.localSource))
                    {
                        goodforgo = false;
                        Console.WriteLine("ERROR 1002: The source folder does not exist: " + Configuration.localSource);
                    }
                }
            }


            if (goodforgo == true)
            {
                ClientContext o365context = Configuration.GetUserContext();
                if (Configuration.timeoutValue != 0)
                {

                    o365context.RequestTimeout = Configuration.timeoutValue;

                }

                string initdir = Configuration.initialdir;
                try
                {

                    if (initdir.Substring(0, 1) == "\\") { initdir = initdir.Substring(1); }
                    if (initdir.Substring(0, 1) == "/") { initdir = initdir.Substring(1); }
                }
                catch (Exception ex) { }
                Configuration.initialdir = initdir;

                if (Configuration.cleanLog)
                {
                    try
                    {
                        if (System.IO.File.Exists(Configuration.logFileName))
                        {
                            System.IO.File.Delete(Configuration.logFileName);
                        }
                    }
                    catch (Exception ex) { }
                }




                if (Configuration.sourcePSList.Length > 0)
                {
                    useSPList(o365context);
                }
                else if (Configuration.remoteSourceList.Length > 0)
                {
                    string listd = "";
                    if (Configuration.o365subsite.Length > 0)
                    {
                        //// need to test this to see why it is set this way for subsites
                        listd = Configuration.o365subsite + "/" + Configuration.o365List;
                        ///temporarily overriding
                        //listd = Configuration.o365List;
                    }
                    else
                    {
                        listd = Configuration.o365List;
                    }

                    if (Configuration.initialdir.Length > 0)
                    {
                        if (Configuration.initialdir.Contains("/"))
                        {

                        }
                        else
                        {
                            if (!FolderExists(o365context, Configuration.initialdir))
                            {

                                //curfolder = CreateFolder(o365Context, Configuration.o365SiteURL, o365DocumentList, o365ParentFolder, thisfolder, Configuration.listGUID);
                            }
                        }
                    }




                    string lists = Configuration.remoteSourceList;
                    if (Configuration.remoteSourceSubSite.Length > 0)
                    {
                        //// need to test this to see why it is set this way for subsites
                        lists = Configuration.remoteSourceSubSite + "/" + Configuration.remoteSourceList;

                    }
                    else
                    {
                        lists = Configuration.remoteSourceList;
                    }
                    if (Configuration.localSource.Length > 0)
                    {
                        /// this will be the start from folder.
                        lists = lists + "/" + Configuration.localSource;
                    }

                    if (Configuration.initialdir.Length > 0)
                    {
                        if (Configuration.initialdir.Contains("/"))
                        {

                        }
                        else
                        {


                        }
                    }
                    goodforgo = true;
                    Microsoft.SharePoint.Client.Folder folder = null;
                    string sourcectxurl = Configuration.coreSiteURL;
                    if (Configuration.remoteSourceSubSite.Length > 0)
                    {
                        sourcectxurl = sourcectxurl + "/" + Configuration.remoteSourceSubSite;
                    }
                    var sctx = Configuration.GetUserContext(sourcectxurl);
                    try
                    {

                        folder = sctx.Web.GetFolderByServerRelativeUrl("/" + lists);
                        sctx.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        goodforgo = false;
                        Console.WriteLine(ex.Message);
                        logit("Error", "Error finding Source File:" + lists, "", ex.Message + "");
                        logError(lists, "", ex.Message + "");
                        Console.WriteLine(ex.Message + ":" + lists);
                    }

                    if (goodforgo)
                    {
                        doFolderWithSourceList(o365context, sctx, Configuration.localSource, Configuration.initialdir, "", listd, folder);
                    }


                }
                else
                {
                    string listd = "";
                    if (Configuration.o365subsite.Length > 0)
                    {
                        //// need to test this to see why it is set this way for subsites
                        listd = Configuration.o365subsite + "/" + Configuration.o365List;
                        ///temporarily overriding
                        //listd = Configuration.o365List;
                    }
                    else
                    {
                        listd = Configuration.o365List;
                    }

                    if (Configuration.initialdir.Length > 0)
                    {
                        if (Configuration.initialdir.Contains("/"))
                        {

                        }
                        else
                        {
                            if (!FolderExists(o365context, Configuration.initialdir))
                            {

                                //curfolder = CreateFolder(o365Context, Configuration.o365SiteURL, o365DocumentList, o365ParentFolder, thisfolder, Configuration.listGUID);
                            }
                        }
                    }
                    doFolder(o365context, Configuration.localSource, Configuration.initialdir, "", listd);
                }
            }
        }


        private static int useMetadataToFolderModeCordner(ClientContext ctx, string targetParentFolder)
        {
            Console.WriteLine("Using Metadata Fields and Source List - checking for first item in list - Cordner version");
            int keepgoing = 1;
            string[] fdata = new string[100];
            string[] fdatadesc = new string[100];
            string str1 = "";
            List clientObject;
            ListItemCollection items;
            /// customisation for cordner //
            /// 
            List ClientGroupList = ctx.Web.Lists.GetByTitle("ClientGroupList");
            List ClientList = ctx.Web.Lists.GetByTitle("ClientList");
            List ClientListOld = ctx.Web.Lists.GetByTitle("Clients");

            var cgfields = ClientGroupList.Fields;
            var cfields = ClientList.Fields;
            var cfields2 = ClientListOld.Fields;

            ctx.Load(cgfields);
            ctx.Load(cfields);
            ctx.Load(cfields2);
            ctx.ExecuteQuery();

            /// end custom //


            int num2;
            try
            {
                clientObject = !(Configuration.sourceListGUID != "") ? ctx.Web.Lists.GetByTitle(Configuration.sourcePSList) : ctx.Web.Lists.GetById(new Guid(Configuration.sourceListGUID));
                ctx.Load(clientObject);
                items = clientObject.GetItems(new CamlQuery()
                {
                    ViewXml = "<View Scope = 'RecursiveAll'><RowLimit>1</RowLimit><Query><OrderBy><FieldRef Name='ID' Type='Number' Ascending='FALSE' /></OrderBy></Query></View>"
                });
                ctx.Load(items);
                Console.WriteLine("Getting first and last document numbers..");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: 3015: " + ex.Message);
                Console.WriteLine("Source List does not exist: " + Configuration.sourcePSList + ":" + Configuration.sourceListGUID);
                num2 = 0;
                return 0;
            }
            if (keepgoing == 1)
            {
                try
                {
                    ctx.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: 3015: " + ex.Message);
                    Console.WriteLine("Source List does not exist: " + Configuration.sourcePSList);
                    num2 = 0;
                    return 0;
                }
            }
            long firstid = 0;
            long incrementBy = 2000;
            long lastid = 0;
            if (keepgoing == 1)
            {
                foreach (Microsoft.SharePoint.Client.ListItem listItem in items)
                {
                    try
                    {
                        int result;
                        lastid = !int.TryParse(listItem["ID"].ToString(), out result) ? 0L : (long)result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: 3016: " + ex.Message);
                        Console.WriteLine("Source List does not have an ID field?: " + Configuration.sourcePSList);
                        keepgoing = 0;
                    }
                }
            }
            if (keepgoing == 1)
            {
                items = clientObject.GetItems(new CamlQuery()
                {
                    ViewXml = "<View Scope = 'RecursiveAll'><RowLimit>1</RowLimit><Query><OrderBy><FieldRef Name='ID' Type='Number'  /></OrderBy></Query></View>"
                });
                ctx.Load(items);
                try
                {
                    ctx.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: 3015: " + ex.Message);
                    Console.WriteLine("Source List does not exist: " + Configuration.sourcePSList);
                    keepgoing = 0;
                }
            }
            if (keepgoing == 1)
            {
                foreach (Microsoft.SharePoint.Client.ListItem listItem in items)
                {
                    try
                    {
                        int result;
                        firstid = !int.TryParse(listItem["ID"].ToString(), out result) ? 0L : (long)result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: 3016: " + ex.Message);
                        Console.WriteLine("Source List does not have an ID field?: " + Configuration.sourcePSList);
                        keepgoing = 0;
                    }
                }
            }
            int continuer = 1;
            long upto = firstid - 1L;
            Console.WriteLine("First DocID: " + upto + ", Last Doc ID:" + lastid);
            if (Configuration.startfrom.Length > 0)
            {
                if (Int32.Parse(Configuration.startfrom) > 0)
                {
                    upto = Int32.Parse(Configuration.startfrom);
                    Console.WriteLine("Starting from DocID: " + upto + ", Last Doc ID:" + lastid);
                }
            }
            if (Program.getMetadataFields(Configuration.metadatafoldersfieldlist))
            {
                fdata = new string[Configuration.metadatafolderfields.Length];
            }
            else
            {
                Console.WriteLine("ERROR 3019 No fields: " + Configuration.metadatafoldersfieldlist);
                continuer = 0;
                keepgoing = 0;
            }
            if (keepgoing == 1)
            {
                while (continuer == 1)
                {
                    Console.WriteLine("Next batch:{0}-{1}", upto, (upto + incrementBy));
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = "<View Scope = 'RecursiveAll'><Query><Where><And><Geq><FieldRef Name=\"ID\"/><Value Type=\"Integer\">" + upto + "</Value></Geq><Leq><FieldRef Name=\"ID\"/><Value Type=\"Integer\">" + (upto + incrementBy) + "</Value></Leq></And></Where>";
                    query.ViewXml += "<OrderBy>";
                    for (int x = 0; x < Configuration.metadatafolderfields.Length; ++x)
                    {
                        CamlQuery camlQuery = query;
                        camlQuery.ViewXml = camlQuery.ViewXml + "<FieldRef Name = '" + Configuration.metadatafolderfields[x] + "' />";
                    }
                    query.ViewXml += "</OrderBy>";
                    query.ViewXml += "</Query></View>";
                    ListItemCollection listItems = clientObject.GetItems(query);
                    ctx.Load(listItems);

                    upto = upto + incrementBy;

                    try
                    {
                        ctx.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: 3015: " + ex.Message);
                        Console.WriteLine("There are issues with your query columns - they might not exist: " + Configuration.metadatafoldersfieldlist);
                        return 0;
                    }
                    int num8 = 0;
                    String topfolderreplacement = "NoClientGroup";
                    string baseurl = Configuration.o365SiteURL + "/" + Configuration.o365List;
                    if (Configuration.initialdir.Length > 0)
                        baseurl = baseurl + "/" + Configuration.initialdir;
                    if (continuer == 1)
                    {
                        foreach (Microsoft.SharePoint.Client.ListItem listItem in (ClientObjectCollection<Microsoft.SharePoint.Client.ListItem>)listItems)
                        {
                            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.P)
                            {
                                Console.Write("pausing, press enter to continue");
                                Console.ReadLine();
                            }
                            num8 = 0;
                            int num9 = 1;
                            int result;

                            long curid = !int.TryParse(listItem["ID"].ToString(), out result) ? 0L : (long)result;


                            if (listItem.FileSystemObjectType == FileSystemObjectType.File)
                            {
                                /// added this to reset all values.
                                //fdata = new string[100];
                                /// end update
                                Microsoft.SharePoint.Client.File file = listItem.File;
                                ctx.Load(file);
                                ctx.ExecuteQuery();
                                int skipremainingfields = 0;
                                String ClientGroupName = "";
                                String ClientGroupID = "";
                                int cgroupid = 0;
                                string clientcode = "";
                                string clientgroupcode = "";


                                try
                                {
                                    string metadatastring = "";
                                    str1 = "";
                                    fdata = new string[Configuration.metadatafolderfields.Length];
                                    fdatadesc = new string[Configuration.metadatafolderfields.Length];
                                    Boolean skippingclient = false;
                                    for (int x = 0; x < Configuration.metadatafolderfields.Length; ++x)
                                    {
                                        /// step 1 - get the metadata
                                        string metadatafolderfield = Configuration.metadatafolderfields[x];
                                        metadatastring = "";
                                        if (listItem[metadatafolderfield] == null || listItem[metadatafolderfield].ToString() == "")
                                        {
                                            if (x == 0)
                                            {
                                                if (topfolderreplacement.Length > 0)
                                                {
                                                    fdata[x] = topfolderreplacement;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            fdata[x] = listItem[metadatafolderfield].ToString();

                                        }
                                        fdatadesc[x] = metadatafolderfield + ":" + fdata[x];
                                    }


                                    ////// step two rework the folders ///
                                    ///

                                    if (fdata[5] == "BAS")
                                    {
                                        // fdata[2] = year
                                        fdata[3] = "BAS IAS";
                                        fdata[4] = "";

                                    }
                                    else if (fdata[3] == "Advisory")
                                    {
                                        if (fdata[4] == "ASIC")
                                        {
                                            fdata[2] = "Register";
                                            fdata[3] = "ASIC Docs";
                                            fdata[4] = "";
                                        }
                                        else
                                        {
                                            fdata[2] = "Advisory";
                                            fdata[3] = "";
                                            fdata[4] = "";
                                        }
                                    }
                                    else if (fdata[3] == "Client Engagement")
                                    {
                                        if (fdata[4] == "ASIC")
                                        {
                                            fdata[2] = "Register";
                                            fdata[3] = "ASIC Docs";
                                            fdata[4] = "";
                                        }
                                        else
                                        {
                                            skippingclient = true;
                                            fdata[1] = "Planning";
                                            fdata[2] = "";
                                            fdata[3] = "";
                                            fdata[4] = "";
                                        }

                                    }
                                    else if (fdata[3] == "Company Secretarial")
                                    {
                                        if (fdata[4] == "Company")
                                        {
                                            fdata[2] = "Register";
                                            fdata[3] = "Constitution or Deed";
                                            fdata[4] = "";
                                        }
                                        else if (fdata[4] == "Trust")
                                        {
                                            fdata[2] = "Register";
                                            fdata[3] = "Constitution or Deed";
                                            fdata[4] = "";
                                        }
                                        else if (fdata[4] == "Workpapers")
                                        {
                                            // fdata[2] = year
                                            fdata[3] = "Workpapers";
                                            fdata[4] = "";
                                        }
                                        else if (fdata[4] == "Year End")
                                        {
                                            // fdata[2] = year
                                            fdata[3] = "Workpapers";
                                            fdata[4] = "";
                                        }
                                        else
                                        {

                                            fdata[2] = "Register";
                                            fdata[3] = "ASIC Docs";
                                            fdata[4] = "";
                                        }
                                    }
                                    else if (fdata[3] == "Compliance")
                                    {
                                        if (fdata[4] == "ASIC")
                                        {
                                            fdata[2] = "Register";
                                            fdata[3] = "ASIC Docs";
                                            fdata[4] = "";
                                        }
                                        else if (fdata[4] == "Client Data")
                                        {
                                            // fdata[2] = year
                                            fdata[3] = "Workpapers";
                                            fdata[4] = "";
                                        }
                                        else
                                        {
                                            // fdata[2] = year
                                            fdata[3] = "Workpapers";
                                            fdata[4] = "";
                                        }
                                    }
                                    else if (fdata[3] == "Correspondence")
                                    {
                                        if (fdata[4] == "ASIC")
                                        {
                                            fdata[2] = "Register";
                                            fdata[3] = "ASIC Docs";
                                            fdata[4] = "";
                                        }
                                        else
                                        {
                                            fdata[2] = "Corro - Entity";
                                            fdata[3] = "";
                                            fdata[4] = "";
                                        }
                                    }
                                
                                    else if (fdata[3] == "Permanent")
                                    {
                                        if (fdata[4] == "ASIC")
                                        {
                                            fdata[2] = "Register";
                                            fdata[3] = "ASIC Docs";
                                            fdata[4] = "";
                                        }
                                        else
                                        {
                                            fdata[2] = "Permanent";
                                            fdata[3] = "";
                                            fdata[4] = "";
                                        }

                                    }
                                    else if (fdata[3] == "SMSF")
                                    {

                                        //fdata[2] = year;
                                        fdata[3] = "Workpapers";
                                        fdata[4] = "";


                                    }
                                    else if (fdata==null || fdata[3] == "")
                                    {
                                        //fdata[2] = year;
                                        fdata[3] = "Workpapers";
                                        fdata[4] = "";

                                    }
                                    else
                                    {
                                       /// Leave as is and this should then show up in an odd place thus highlighting that someone has been missed.

                                    }

                                    /// end change folders


                                    for (int x = 0; x < fdata.Length; ++x)
                                    {
                                        if (skipremainingfields == 0)
                                        {


                                            string str3 = fdata[x];
                                            string rawstr3 = str3;
                                            if (str3 == null || str3 == "")
                                            {
                                                skipremainingfields = 1;
                                            }
                                            //else if (fdata[x] == str3)
                                            //{
                                            //    str1 = str1 + "/" + fdata[x];
                                            //}
                                            else
                                            {
                                                str3 = sanitiseFileName(str3);
                                                fdata[x] = str3;

                                                // cornder customisation

                                                if (x == 1 && skippingclient == false)
                                                {

                                                    CamlQuery squery = new CamlQuery();
                                                    squery.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='ClientName' /><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", rawstr3);
                                                    ListItemCollection clientOldListItems = ClientListOld.GetItems(squery);
                                                    ctx.Load(clientOldListItems);

                                                    try
                                                    {
                                                        ctx.ExecuteQuery();
                                                        if (clientOldListItems.Count > 0)
                                                        {
                                                            foreach (ListItem oli in clientOldListItems)
                                                            {
                                                                clientcode = oli["ClientID"].ToString();
                                                                clientgroupcode = clientcode.Substring(0, 5);
                                                                if (x == 0)
                                                                {
                                                                    fdata[x] = clientgroupcode + " - " + str3;
                                                                }
                                                                else if (x == 1)
                                                                {
                                                                    fdata[x] = clientcode + " - " + str3;

                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            clientcode = "";

                                                            //foldername = str3;
                                                        }

                                                        ctx.ExecuteQuery();
                                                    }
                                                    catch (Exception ex) { }
                                                }
                                                else if (x == 0)
                                                {
                                                    CamlQuery squery = new CamlQuery();
                                                    squery.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='ClientGroup' /><Value Type='Text'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>", rawstr3);
                                                    ListItemCollection clientOldListItems = ClientListOld.GetItems(squery);
                                                    ctx.Load(clientOldListItems);

                                                    clientgroupcode = "";

                                                    try
                                                    {
                                                        ctx.ExecuteQuery();
                                                        if (clientOldListItems.Count > 0)
                                                        {
                                                            foreach (ListItem oli in clientOldListItems)
                                                            {
                                                                clientcode = oli["ClientID"].ToString();
                                                                clientgroupcode = clientcode.Substring(0, 5);
                                                                ClientGroupName = oli["ClientGroup"].ToString();
                                                                if (x == 0)
                                                                {
                                                                    fdata[x] = clientgroupcode + " - " + str3;
                                                                }
                                                                else if (x == 1)
                                                                {
                                                                    fdata[x] = clientcode + " - " + str3;

                                                                }
                                                            }
                                                        }
                                                        else
                                                        {

                                                            clientgroupcode = "";
                                                            ClientGroupName = "";
                                                            //foldername = str3;
                                                        }

                                                        ctx.ExecuteQuery();
                                                    }
                                                    catch (Exception ex) { }

                                                }
                                                /// end cordner customisation



                                                String curfold = targetParentFolder + str1;
                                                if (!Program.FolderExists(ctx, targetParentFolder + str1 + "/" + sanitiseFileName(fdata[x])))
                                                {
                                                    string relativePath = str1.Length <= 0 ? str1 : (!(str1.Substring(1, 1) == "/") ? str1 : str1.Substring(2));
                                                    if (Program.CreateFolder(ctx, Configuration.o365SiteURL, Configuration.o365List, relativePath, fdata[x], Configuration.listGUID) == null)
                                                    {
                                                        num9 = 0;
                                                        Program.logError(file.ServerRelativeUrl, str1 + "/" + fdata[x], "ERROR: 3017: unable to create folder: " + str1 + "/" + fdata[x] + " -docid: " + curid + " subsite:" + Configuration.o365subsite + ", targetList:" + Configuration.o365List);
                                                        Console.WriteLine("ERROR - Unable to create folder:  {0}", (object)str1);
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Created Folder:  {0}", relativePath + "/" + fdata[x]);
                                                        // cordner customisation

                                                        if (x == 0)
                                                        {
                                                            //// cordner customisation
                                                            ClientGroupName = rawstr3;

                                                            CamlQuery squery2 = new CamlQuery();
                                                            squery2.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", rawstr3);
                                                            ListItemCollection clientgroupListItems = ClientGroupList.GetItems(squery2);
                                                            ctx.Load(clientgroupListItems);
                                                            try
                                                            {
                                                                ctx.ExecuteQuery();
                                                                if (clientgroupListItems.Count > 0)
                                                                {
                                                                    foreach (ListItem oli in clientgroupListItems)
                                                                    {

                                                                        oli["folderLocation"] = Configuration.o365List + relativePath + "/" + fdata[x];
                                                                        oli["ClientGroupNumber"] = clientgroupcode;
                                                                        oli.Update();
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var itemCreateInfo = new ListItemCreationInformation();
                                                                    var newItem = ClientGroupList.AddItem(itemCreateInfo);
                                                                    newItem["Title"] = rawstr3;
                                                                    newItem["ClientGroupNumber"] = clientgroupcode;
                                                                    newItem["folderLocation"] = Configuration.o365List + relativePath + "/" + fdata[x];
                                                                    newItem.Update();
                                                                }

                                                                ctx.ExecuteQuery();


                                                            }
                                                            catch (Exception ex) { }

                                                            squery2 = new CamlQuery();
                                                            squery2.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", rawstr3);
                                                            clientgroupListItems = ClientGroupList.GetItems(squery2);
                                                            ctx.Load(clientgroupListItems);
                                                            cgroupid = 0;
                                                            try
                                                            {
                                                                ctx.ExecuteQuery();

                                                                if (clientgroupListItems.Count > 0)
                                                                {
                                                                    foreach (ListItem oli in clientgroupListItems)
                                                                    {
                                                                        cgroupid = Int32.Parse(oli["ID"].ToString());
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    cgroupid = 0;
                                                                }

                                                                ctx.ExecuteQuery();


                                                            }
                                                            catch (Exception ex) { }

                                                            /// create the sub folders
                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "Corro - Group");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "Final Accounts");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "Permanent");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "Planning");

                                                        }
                                                        else if (x == 1)
                                                        {
                                                            CamlQuery squery3 = new CamlQuery();
                                                            squery3.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", rawstr3);
                                                            ListItemCollection clientListItems = ClientList.GetItems(squery3);
                                                            ctx.Load(clientListItems);
                                                            try
                                                            {
                                                                ctx.ExecuteQuery();
                                                                if (clientListItems.Count > 0)
                                                                {
                                                                    foreach (ListItem oli in clientListItems)
                                                                    {
                                                                        oli["ClientGroup"] = ClientGroupName;
                                                                        oli["folderLocation"] = Configuration.o365List + relativePath + "/" + fdata[x];
                                                                        oli["ClientNumber"] = clientcode;
                                                                        oli.Update();
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var itemCreateInfo = new ListItemCreationInformation();
                                                                    var newItem = ClientList.AddItem(itemCreateInfo);
                                                                    newItem["Title"] = rawstr3;
                                                                    newItem["ClientNumber"] = clientcode;
                                                                    newItem["ClientGroup"] = cgroupid;
                                                                    newItem["folderLocation"] = Configuration.o365List + relativePath + "/" + fdata[x]; ;
                                                                    newItem.Update();
                                                                }

                                                                ctx.ExecuteQuery();
                                                            }
                                                            catch (Exception ex) { }
                                                            /// make all the client folders
                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "Advisory");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "Corro - Entity");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "Permanent");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "Register");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/Register", "Constitution Deeds");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/Register", "Minutes");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/Register", "ASIC Docs");

                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "2019");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/2019", "Bookkeeping");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/2019", "BAS IAS");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/2019", "Client Data");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/2019", "Workpapers");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x], "2020");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/2020", "Bookkeeping");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/2020", "BAS IAS");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/2020", "Client Data");
                                                            makeafolder(ctx, relativePath + "/" + fdata[x] + "/2020", "Workpapers");

                                                        }
                                                    }
                                                }
                                                str1 = str1 + "/" + fdata[x];
                                            }
                                            
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("ERROR: 3016: " + ex.Message);
                                    Console.WriteLine("Something wrong with your field list: " + Configuration.metadatafoldersfieldlist);
                                    num2 = 0;
                                }
                                string name = file.Name;
                                string str4 = baseurl;
                                string str5 = str1.Length <= 0 ? str4 + "/" : str4 + str1 + "/";
                                if (Configuration.skiproot && str1.Length == 0)
                                {
                                    num9 = 0;
                                    Program.logit("Skip", file.ServerRelativeUrl, str5 + name, file.ServerRelativeUrl + " -> " + str5 + name);
                                    Console.WriteLine("Skipped:  {0} -> {1}", (object)file.ServerRelativeUrl, (object)(str5 + name));
                                }
                                if (num9 == 1)
                                {
                                    try
                                    {
                                        string str3 = string.Format("{0}/{1}", str1, sanitiseFileName(name));
                                        string serverRelativeUrl = string.Format("/{0}{1}", Configuration.o365List, str3);
                                        if (Configuration.o365subsite.Length > 0)
                                        {
                                            serverRelativeUrl = "/" + Configuration.o365subsite + serverRelativeUrl;
                                        }
                                        if (!Configuration.overwrite)
                                        {
                                            if (!Program.TryGetFileByServerRelativeUrl(ctx.Web, serverRelativeUrl))
                                            {
                                                file.CopyTo(str5 + name, Configuration.overwrite);
                                                ctx.ExecuteQuery();
                                                string d = "metadata{ " + string.Join("; ", fdatadesc) + ")";
                                                Program.logit("Normal", file.ServerRelativeUrl, str5 + name, "(docid:" + curid + ") " + d + file.ServerRelativeUrl + " -> " + str5 + name);
                                                Console.WriteLine("(docid: " + curid + ")Copied:  {0} -> {1}", (object)file.ServerRelativeUrl, (object)(str5 + name));
                                                setCreateData(ctx, str5 + name, file.TimeCreated, file.Author, file.TimeLastModified, file.ModifiedBy);
                                            }
                                            else
                                            {
                                                Program.logit("Skip", file.ServerRelativeUrl, str5 + name, "destination file already exists");
                                                Console.WriteLine("Skipped:  {0} -> {1}", (object)file.ServerRelativeUrl, (object)(str5 + name));
                                            }
                                        }
                                        else
                                        {

                                            file.CopyTo(str5 + name, Configuration.overwrite);
                                            ctx.ExecuteQuery();
                                            string d = "metadata{ " + string.Join("; ", fdatadesc) + ")";
                                            Program.logit("Normal", file.ServerRelativeUrl, str5 + name, "(docid: " + curid + ") " + d + file.ServerRelativeUrl + "-> " + str5 + name);
                                            Console.WriteLine("(docid: " + curid + ") Copied: {0} -> {1}", (object)file.ServerRelativeUrl, (object)(str5 + name));
                                            Console.WriteLine(file.ServerRelativeUrl);
                                            setCreateData(ctx, str5 + name, file.TimeCreated, file.Author, file.TimeLastModified, file.ModifiedBy);

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("ERROR: 3021: " + ex.Message);
                                        Console.WriteLine("unable to copy to: " + baseurl + "/" + str1 + "/" + name);
                                        string d = "metadata{ " + string.Join("; ", fdatadesc) + ")";
                                        Program.logError(file.ServerRelativeUrl, str5 + name, "ERROR: 3021: unable to cpoy file: " + str5 + name + " -docid: " + curid + " - " + d + " subsite:" + Configuration.o365subsite + ", targetList:" + Configuration.o365List);
                                        num2 = 0;
                                    }
                                }
                            }
                            if (curid == lastid)
                            {
                                continuer = 0;
                                upto = curid;
                            }
                            else if (curid > lastid)
                            {
                                continuer = 0;
                                upto = curid;
                            }
                        }
                    }
                }
            }
            return 1;
        }

        public static void cordnergetFolderCustomRunOnce(ClientContext ctx, string folderURL, Folder folder, int level, string copytofolder, string parentcode)
        {

            if (Configuration.resumed == false)
            {
                if (Configuration.startfrom == folderURL)
                {
                    Configuration.resumed = true;
                }
            }
            ctx.Load(folder);
            ctx.Load(folder.Folders);
            if (Configuration.resumed)
            {
                ctx.Load(folder.Files);
                ctx.Load(folder.Files);
            }
            else
            {

            }
            Boolean foundit = true;
            try
            {
                ctx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                foundit = false;
            }


            if (foundit)
            {
                level = level + 1;
                Console.WriteLine(folder.ServerRelativeUrl);

                if (Configuration.resumed)
                {

                    if (level > 2)
                    {

                        foreach (Microsoft.SharePoint.Client.File fl in folder.Files)
                        {
                            ListItem item = fl.ListItemAllFields;
                            ctx.Load(item);
                            try
                            {
                                ctx.ExecuteQuery();
                                Console.WriteLine("MOVE FILE {0} to {1}", fl.ServerRelativeUrl, copytofolder + "/" + fl.Name);
                                logit("MOVE FILE", fl.ServerRelativeUrl, copytofolder + "/" + fl.Name, "Move File");
                                string tmpFileName = copytofolder + "/" + fl.Name;
                                //tmpFileName = tmpFileName string.Format("/{0}{1}", o365DocumentList, tmpFileName);
                                string flename = fl.Name;
                                if (TryGetFileByServerRelativeUrl(ctx.Web, tmpFileName))
                                {
                                    Console.WriteLine("check file:" + tmpFileName);
                                    flename = DateTime.Now.ToString("yyyyMMddHHmmssfff") + flename;
                                    logit("PROBLEM", fl.ServerRelativeUrl, copytofolder + "/" + fl.Name, "File already exists: will call file similar to:" + flename);
                                }

                                /// UNCOMMENT FOR THE FULL RUN
                                fl.MoveTo(copytofolder + "/" + flename, MoveOperations.Overwrite);
                                //fl.CopyTo(copytofolder + "/" + flename,true);
                                ctx.ExecuteQuery();
                                    
                                /// END UNCOMMENT
                            }
                            catch (Exception ex)
                            {
                                Program.logError(folder.ServerRelativeUrl, fl.ServerRelativeUrl, "ERROR 1001: Unable to get file metadata in folder:" + ex.Message);
                                logit("Error", folder.ServerRelativeUrl, fl.ServerRelativeUrl, "ERROR 1001: Unable to get file metadata in folder:" + ex.Message);
                            }
                            

                        }
                    }
                }

                Regex r = new Regex(@"^[0-9A-Za-z][0-9A-Za-z][0-9A-Za-z][0-9A-Za-z][0-9A-Za-z] -");
                //Regex r = new Regex(@"^[0-9][0-9][0-9][0-9][0-9] -");
                //[0-9A-Za-z]
                string[] excludes = { "Corro" };
                foreach (Folder f in folder.Folders)
                {
                    ctx.Load(f.ListItemAllFields);
                    ctx.ExecuteQuery();
                    Console.WriteLine("folder: {0}", f.ServerRelativeUrl);
                    if (level == 2)
                    {

                        if (r.IsMatch(f.Name))
                        {
                            string clientcode = f.Name.Substring(0, 5);
                            
                                if (excludes.Contains(clientcode))
                                {
                                    logit("SKIP FOLDER", f.ServerRelativeUrl, f.Name, "TO Remain as in exlusion list");
                                }
                                else
                                {
                                    if (clientcode == parentcode)
                                    {
                                        Console.WriteLine("{0} {1}", "FOUND DELETABLE", f.ServerRelativeUrl);
                                        logit("DELETABLE FOLDER", f.ServerRelativeUrl, f.Name, "To be deleted");
                                        /// first go find all the sub folders and then delete this one.
                                        cordnergetFolderCustomRunOnce(ctx, Configuration.o365SiteURL + f.ServerRelativeUrl, f, level, copytofolder, parentcode);

                                        try
                                    {
                                        Console.WriteLine("{0} {1}", "DELETE", f.ServerRelativeUrl);
                                        logit("Delete", f.ServerRelativeUrl, f.Name, "DELETE FOLDER");
                                        /// UNCOMMENT FOR THE FULL RUN
                                        f.MoveTo("/documentcenter/Client Documents/000ForDelete/" + f.Name);
                                        //f.Recycle(); 
                                         ctx.ExecuteQuery();
                                        // end UNCOMMENT

                                    }
                                    catch (Exception ex)
                                    {
                                        logError(f.ServerRelativeUrl, f.Name, "Unable to DELETE FOLDER -> " + ex.Message);
                                        logit("Error", f.ServerRelativeUrl, f.Name, "Unable to DELETE FOLDER -> " + ex.Message);
                                    }
                                    }
                                    else
                                    {
                                            logit("SKIP FOLDER", f.ServerRelativeUrl, f.Name, "NOT the Same code as parent");
                                    }
                            }
                            


                        }
                    }
                    else if (level > 2)
                    {
                        cordnergetFolderCustomRunOnce(ctx, Configuration.o365SiteURL + f.ServerRelativeUrl,  f, level, copytofolder,parentcode);

                    }
                    else if (level == 1)
                    {
                        string clientcode = f.Name.Substring(0, 5);
                        if (Configuration.resumed == false)
                        {
                            if (Configuration.startfrom == clientcode)
                            {
                                Console.WriteLine("RESUME");
                                Configuration.resumed = true;
                            }
                        }
                        if (Configuration.resumed)
                        {
                            cordnergetFolderCustomRunOnce(ctx, Configuration.o365SiteURL + f.ServerRelativeUrl, f, level, f.ServerRelativeUrl, clientcode);
                        }
                    }


                    if (Console.KeyAvailable)
                    {
                        if (Console.ReadKey(true).Key == ConsoleKey.P)
                        {
                            Console.Write("pausing, press enter to continue");
                            Console.ReadLine();
                        }
                    }

                }

            }
            else
            {
                Console.WriteLine("Something is wrong with the list you indicated. You may have to revert to using GUID");
            }



        }

        private static int setCreateData2(ClientContext ctx, String flelocation, System.DateTime createdate, User createby, System.DateTime editdate, User editby)
        {
           
                //Microsoft.SharePoint.Client.File file = ctx.Web.GetFileByServerRelativeUrl(flelocation);
            String fle = flelocation;//.Remove(0,1);
            try
            {
                //if (Configuration.o365subsite.Length > 0)
                //{
                //    fle = "/" + Configuration.o365subsite + "/" + fle;
                //}
                
                
                //fle = "/" + Configuration.o365subsite + "/" + fle;
                //String fle = flelocation; //flelocation.Remove(0, Configuration.o365SiteURL.Length + 1);
                Microsoft.SharePoint.Client.File file = ctx.Web.GetFileByServerRelativeUrl(fle);
                ctx.Load(file, f => f.ListItemAllFields);
                ctx.ExecuteQuery();
                ListItem item = file.ListItemAllFields;

                item["Created"] = createdate;
                item["Modified"] = editdate;
                item["Author"] = createby;
                item["Editor"] = editby;

                item.Update();
                file.Update();
                ctx.ExecuteQuery();
            }catch(Exception ex)
            {
                logError("", flelocation, "ERROR: 1099: unable to set metadata" + flelocation + ex.Message);
                logit("Error", "", "", "ERROR: 1099: unable to set metadata" + flelocation + ex.Message);
                Console.WriteLine("ERROR: 1099: unable to set metadata" + flelocation + ex.Message);
            }

            return 0;

        }
        private static int setCreateData(ClientContext ctx, String flelocation, System.DateTime createdate, User createby, System.DateTime editdate, User editby)
        {
            //Microsoft.SharePoint.Client.File file = ctx.Web.GetFileByServerRelativeUrl(flelocation);
            String fle = flelocation.Remove(0,Configuration.o365SiteURL.Length + 1);
            fle = "/" + Configuration.o365subsite + "/"  + fle;
            Microsoft.SharePoint.Client.File file = ctx.Web.GetFileByServerRelativeUrl(fle);
            ctx.Load(file, f => f.ListItemAllFields);
            ctx.ExecuteQuery();
            ListItem item = file.ListItemAllFields;
            item["Author"] = createby;
            item["Editor"] = editby;
            item["Created"] = createdate;
            item["Modified"] = editdate;
            item.Update();
            file.Update();
            ctx.ExecuteQuery();
            

            /*
            //throw new Exception("You need to change the Destination Library Name in the rename thing");

            //String fle = flelocation.Remove(0,Configuration.o365SiteURL.Length + 1);
            //fle = "/" + Configuration.o365subsite + "/"  + fle;

            String targetLibrary = Configuration.o365List;

            //String fle = flelocation.Remove(0, Configuration.o365SiteURL.Length + 1);
            //fle = fle.Remove(0, targetLibrary.Length + 1);

            List NewDocList = ctx.Web.Lists.GetByTitle(targetLibrary);
            CamlQuery squery = new CamlQuery();
            squery.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='URL' /><Value Type='URL'>{0}</Value></Eq></Where></Query> <ViewFields><FieldRef Name='Author' /><FieldRef Name='Created' /><FieldRef Name='Title' /></ViewFields> </View>", fle);
            ListItemCollection Items = NewDocList.GetItems(squery);
            ctx.Load(NewDocList);
            ctx.Load(Items);

            
            try
            {
                ctx.ExecuteQuery();
                if (Items.Count > 0)
                {
                    foreach (ListItem oli in Items)
                    {
                        oli["Author"] = createby;
                        oli["TimeCreated"] = createdate;
                        oli.Update();
                    }
                }
                ctx.ExecuteQuery();
            }catch(Exception ex)
            {
                Console.WriteLine("ERROR: couldnt set date or author");
            
            */
            
            return 0;

        }
        private static int setEditDate(ClientContext ctx, String flelocation, System.DateTime createdate, System.DateTime editdate)
        {
            String fle = flelocation;// flelocation.Remove(0, Configuration.o365SiteURL.Length + 1);
            try
            {
                //Microsoft.SharePoint.Client.File file = ctx.Web.GetFileByServerRelativeUrl(flelocation);

                                         //fle = "/" + Configuration.o365subsite  + fle;
                Microsoft.SharePoint.Client.File file = ctx.Web.GetFileByServerRelativeUrl(fle);
                ctx.Load(file, f => f.ListItemAllFields);
                //ctx.Load(file.ListItemAllFields);
            
                //ctx.ExecuteQuery();
                ListItem item = file.ListItemAllFields;
                //ctx.Load(item);
                String log1 = String.Format("Set Edit Date create:{0} edit:{1} -> {2}", createdate, editdate, fle);
                doExecute(ctx, log1, "", fle, fle);
                //ctx.ExecuteQuery();

                item["Created"] = createdate;
                item["Modified"] = editdate;
                item.Update();
                file.Update();
                //ctx.ExecuteQuery();
                doExecute(ctx, log1, "", fle, fle);
                Console.WriteLine("Set Edit Date create:{0} edit:{1} -> {2}", createdate, editdate, fle);

            } catch(Exception ex){
                Console.WriteLine("Error Setting Dates:{0} edit:{1} -> {2}", createdate, editdate, fle);
                logit("ErrorMeta", flelocation, "", ex.Message + ":" + flelocation);
                logError(flelocation, "error setting edit and create date", ex.Message + ":" + flelocation);
            }


            return 0;

        }

        private static void doExecute(ClientContext ctx, String log, String type, String sourcefilename, String targetfilename)
        {
            try
            {
                ctx.ExecuteQuery();
            }
            catch (System.Net.WebException wex)
            {
                var response = wex.Response as System.Net.HttpWebResponse;
                if (response != null && (response.StatusCode == (System.Net.HttpStatusCode)429 || response.StatusCode == (System.Net.HttpStatusCode)503))
                {
                    logit("Notice:", "", "", "Throttling - Notice 1005");
                    Console.WriteLine("Throttling - Notice 1005");

                    var retryAfter = Int32.Parse(response.Headers["Retry-After"].ToString());
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(retryAfter));
                    try
                    {
                        ctx.ExecuteQuery();
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine("Error:{0} {1} {2}", type, log + ex2.Message, sourcefilename);
                        logit("Error", sourcefilename, "", ex2.Message + ":" + targetfilename);
                        logError(sourcefilename, log, ex2.Message + ":" + targetfilename);
                    }
                }
                else
                {
                    Console.WriteLine("Error:{0} {1} {2}", type, log, sourcefilename);
                    logit("Error", sourcefilename, "", wex.Message + ":" + targetfilename);
                    logError(sourcefilename, log, wex.Message + ":" + targetfilename);
                }
            }
        }

        private static int useMetadataToFolderMode(ClientContext ctx, string targetParentFolder)
        {
            Console.WriteLine("Using Metadata Fields and Source List - checking for first item in list");
            int keepgoing = 1;
            string[] fdata = new string[100];
            string str1 = "";
            List clientObject;
            ListItemCollection items;
            /// customisation for cordner //
            /// 
            List ClientGroupList = ctx.Web.Lists.GetByTitle("ClientGroupList");
            List ClientList = ctx.Web.Lists.GetByTitle("ClientList");
            List ClientListOld = ctx.Web.Lists.GetByTitle("Clients");

            var cgfields = ClientGroupList.Fields;
            var cfields = ClientList.Fields;
            var cfields2 = ClientListOld.Fields;

            ctx.Load(cgfields);
            ctx.Load(cfields);
            ctx.Load(cfields2);
            ctx.ExecuteQuery();

            /// end custom //


            int num2;
            try
            {
                clientObject = !(Configuration.sourceListGUID != "") ? ctx.Web.Lists.GetByTitle(Configuration.sourcePSList) : ctx.Web.Lists.GetById(new Guid(Configuration.sourceListGUID));
                ctx.Load(clientObject);
                items = clientObject.GetItems(new CamlQuery()
                {
                    ViewXml = "<View Scope = 'RecursiveAll'><RowLimit>1</RowLimit><Query><OrderBy><FieldRef Name='ID' Type='Number' Ascending='FALSE' /></OrderBy></Query></View>"
                });
                ctx.Load(items);
                Console.WriteLine("Getting first and last document numbers..");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: 3015: " + ex.Message);
                Console.WriteLine("Source List does not exist: " + Configuration.sourcePSList + ":" + Configuration.sourceListGUID);
                num2 = 0;
                return 0;
            }
            if (keepgoing == 1)
            {
                try
                {
                    ctx.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: 3015: " + ex.Message);
                    Console.WriteLine("Source List does not exist: " + Configuration.sourcePSList);
                    num2 = 0;
                    return 0;
                }
            }
            long firstid = 0;
            long incrementBy = 50;
            long lastid = 0;
            if (keepgoing == 1)
            {
                foreach (Microsoft.SharePoint.Client.ListItem listItem in items)
                {
                    try
                    {
                        int result;
                        lastid = !int.TryParse(listItem["ID"].ToString(), out result) ? 0L : (long)result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: 3016: " + ex.Message);
                        Console.WriteLine("Source List does not have an ID field?: " + Configuration.sourcePSList);
                        keepgoing = 0;
                    }
                }
            }
            if (keepgoing == 1)
            {
                items = clientObject.GetItems(new CamlQuery()
                {
                    ViewXml = "<View Scope = 'RecursiveAll'><RowLimit>1</RowLimit><Query><OrderBy><FieldRef Name='ID' Type='Number'  /></OrderBy></Query></View>"
                });
                ctx.Load(items);
                try
                {
                    ctx.ExecuteQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: 3015: " + ex.Message);
                    Console.WriteLine("Source List does not exist: " + Configuration.sourcePSList);
                    keepgoing = 0;
                }
            }
            if (keepgoing == 1)
            {
                foreach (Microsoft.SharePoint.Client.ListItem listItem in items)
                {
                    try
                    {
                        int result;
                        firstid = !int.TryParse(listItem["ID"].ToString(), out result) ? 0L : (long)result;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: 3016: " + ex.Message);
                        Console.WriteLine("Source List does not have an ID field?: " + Configuration.sourcePSList);
                        keepgoing = 0;
                    }
                }
            }
            int continuer = 1;
            long upto = firstid - 1L;
            Console.WriteLine("First DocID: " + upto + ", Last Doc ID:" + lastid);
            if (Configuration.startfrom.Length > 0) { 
                if (Int32.Parse(Configuration.startfrom) > 0)
                {
                    upto = Int32.Parse(Configuration.startfrom);
                    Console.WriteLine("Starting from DocID: " + upto + ", Last Doc ID:" + lastid);
                }
            }
            if (Program.getMetadataFields(Configuration.metadatafoldersfieldlist))
            {
                fdata = new string[Configuration.metadatafolderfields.Length];
            }
            else
            {
                Console.WriteLine("ERROR 3019 No fields: " + Configuration.metadatafoldersfieldlist);
                continuer = 0;
                keepgoing = 0;
            }
            if (keepgoing == 1)
            {
                while (continuer == 1)
                {
                    CamlQuery query = new CamlQuery();
                    query.ViewXml = "<View Scope = 'RecursiveAll'><Query><Where><And><Geq><FieldRef Name=\"ID\"/><Value Type=\"Integer\">" + upto + "</Value></Geq><Leq><FieldRef Name=\"ID\"/><Value Type=\"Integer\">" + (upto + incrementBy) + "</Value></Leq></And></Where>";
                    query.ViewXml += "<OrderBy>";
                    for (int x = 0; x < Configuration.metadatafolderfields.Length; ++x)
                    {
                        CamlQuery camlQuery = query;
                        camlQuery.ViewXml = camlQuery.ViewXml + "<FieldRef Name = '" + Configuration.metadatafolderfields[x] + "' />";
                    }
                    query.ViewXml += "</OrderBy>";
                    query.ViewXml += "</Query></View>";
                    ListItemCollection listItems = clientObject.GetItems(query);
                    ctx.Load(listItems);

                    upto = upto + incrementBy;

                    try
                    {
                        ctx.ExecuteQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: 3015: " + ex.Message);
                        Console.WriteLine("There are issues with your query columns - they might not exist: " + Configuration.metadatafoldersfieldlist);
                        return 0;
                    }
                    int num8 = 0;
                    String topfolderreplacement = "NoClientGroup";
                    string baseurl = Configuration.o365SiteURL + "/" + Configuration.o365List;
                    if (Configuration.initialdir.Length > 0)
                        baseurl = baseurl + "/" + Configuration.initialdir;
                    if (continuer == 1)
                    {
                        foreach (Microsoft.SharePoint.Client.ListItem listItem in (ClientObjectCollection<Microsoft.SharePoint.Client.ListItem>)listItems)
                        {
                            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.P)
                            {
                                Console.Write("pausing, press enter to continue");
                                Console.ReadLine();
                            }
                            num8 = 0;
                            int num9 = 1;
                            int result;

                            long curid = !int.TryParse(listItem["ID"].ToString(), out result) ? 0L : (long)result;
                            

                            if (listItem.FileSystemObjectType == FileSystemObjectType.File)
                            {
                                /// added this to reset all values.
                                //fdata = new string[100];
                                /// end update
                                Microsoft.SharePoint.Client.File file = listItem.File;
                                ctx.Load(file);
                                ctx.ExecuteQuery();
                                int skipremainingfields = 0;
                                String ClientGroupName = "";
                                String ClientGroupID = "";
                                int cgroupid = 0;
                                string clientcode = "";
                                string clientgroupcode = "";
                              

                                try
                                {
                                    str1 = "";
                                    fdata = new string[Configuration.metadatafolderfields.Length];
                                    for (int x = 0; x < Configuration.metadatafolderfields.Length; ++x)
                                    {
                                        if (skipremainingfields == 0)
                                        {
                                            string metadatafolderfield = Configuration.metadatafolderfields[x];
                                            if (listItem[metadatafolderfield] == null || listItem[metadatafolderfield].ToString()=="")
                                            {
                                               
                                                skipremainingfields = 1;
                                                if (x == 0)
                                                {
                                                    if (topfolderreplacement.Length > 0)
                                                    {
                                                        skipremainingfields = 0;
                                                        //str1 = str1 + "/" + topfolderreplacement;
                                                        //targetParentFolder = Configuration.o365subsite;

                                                        if (!Program.FolderExists(ctx, targetParentFolder + "/" + topfolderreplacement))
                                                        {
                                                        string relativePath = str1.Length <= 0 ? str1 : (!(str1.Substring(1, 1) == "/") ? str1 : str1.Substring(2));
                                                        if (Program.CreateFolder(ctx, Configuration.o365SiteURL, Configuration.o365List, relativePath, topfolderreplacement, Configuration.listGUID) == null)
                                                        {
                                                            num9 = 0;
                                                            Program.logError(file.ServerRelativeUrl, str1 + "/" + topfolderreplacement, "ERROR: 3017: unable to create folder: " + str1 + "/" + topfolderreplacement + " - subsite:" + Configuration.o365subsite + ", targetList:" + Configuration.o365List);
                                                            Console.WriteLine("ERROR - Unable to create folder:  {0}", (object)str1);
                                                        }
                                                        else
                                                            Console.WriteLine("Created Folder:  {0}", (object)str1);
                                                        }
                                                        str1 = str1 + "/" + topfolderreplacement;
                                                    }


                                                }
                                                else
                                                {
                                                   

                                                }
                                            }
                                            else
                                            {

                                                string str3 = !(listItem[metadatafolderfield].GetType().Name == "FieldLookupValue") ? listItem[metadatafolderfield].ToString() : (listItem[metadatafolderfield] as FieldLookupValue).LookupValue.ToString();
                                                string rawstr3 = str3;
                                                str3 = sanitiseFileName(str3);
                                                if (str3 == null)
                                                    skipremainingfields = 1;
                                                else if (fdata[x] == str3)
                                                {
                                                    str1 = str1 + "/" + fdata[x];
                                                }
                                                else
                                                {
                                                    fdata[x] = str3;

                                                    // cornder customisation

                                                    if (x == 1)
                                                    {

                                                        CamlQuery squery = new CamlQuery();
                                                        squery.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='ClientName' /><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", rawstr3);
                                                        ListItemCollection clientOldListItems = ClientListOld.GetItems(squery);
                                                        ctx.Load(clientOldListItems);
                                                       
                                                        try
                                                        {
                                                            ctx.ExecuteQuery();
                                                            if (clientOldListItems.Count > 0)
                                                            {
                                                                foreach (ListItem oli in clientOldListItems)
                                                                {
                                                                    clientcode = oli["ClientID"].ToString();
                                                                    clientgroupcode = clientcode.Substring(0, 5);
                                                                    if (x == 0)
                                                                    {
                                                                        fdata[x] = clientgroupcode + " - " + str3;
                                                                    }
                                                                    else if (x == 1)
                                                                    {
                                                                        fdata[x] = clientcode + " - " + str3;

                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                clientcode = "";
                                                                
                                                                //foldername = str3;
                                                            }

                                                            ctx.ExecuteQuery();
                                                        }
                                                        catch (Exception ex) { }
                                                    }else if (x == 0)
                                                    {
                                                        CamlQuery squery = new CamlQuery();
                                                        squery.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='ClientGroup' /><Value Type='Text'>{0}</Value></Eq></Where></Query><RowLimit>1</RowLimit></View>", rawstr3);
                                                        ListItemCollection clientOldListItems = ClientListOld.GetItems(squery);
                                                        ctx.Load(clientOldListItems);
                                                       
                                                        clientgroupcode = "";
                                                        
                                                        try
                                                        {
                                                            ctx.ExecuteQuery();
                                                            if (clientOldListItems.Count > 0)
                                                            {
                                                                foreach (ListItem oli in clientOldListItems)
                                                                {
                                                                    clientcode = oli["ClientID"].ToString();
                                                                    clientgroupcode = clientcode.Substring(0, 5);
                                                                    ClientGroupName = oli["ClientGroup"].ToString();
                                                                    if (x == 0)
                                                                    {
                                                                        fdata[x] = clientgroupcode + " - " + str3;
                                                                    }
                                                                    else if (x == 1)
                                                                    {
                                                                        fdata[x] = clientcode + " - " + str3;

                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                
                                                                clientgroupcode = "";
                                                                ClientGroupName = "";
                                                                //foldername = str3;
                                                            }

                                                            ctx.ExecuteQuery();
                                                        }
                                                        catch (Exception ex) { }

                                                    }
                                                    /// end cordner customisation



                                                    String curfold = targetParentFolder + str1;
                                                    if (!Program.FolderExists(ctx, targetParentFolder +  str1 + "/" + sanitiseFileName(fdata[x])))
                                                    {
                                                        string relativePath = str1.Length <= 0 ? str1 : (!(str1.Substring(1, 1) == "/") ? str1 : str1.Substring(2));
                                                        if (Program.CreateFolder(ctx, Configuration.o365SiteURL, Configuration.o365List, relativePath, fdata[x], Configuration.listGUID) == null)
                                                        {
                                                            num9 = 0;
                                                            Program.logError(file.ServerRelativeUrl, str1 + "/" + fdata[x], "ERROR: 3017: unable to create folder: " + str1 + "/" + fdata[x] + " -docid: " + curid + " subsite:" + Configuration.o365subsite + ", targetList:" + Configuration.o365List);
                                                            Console.WriteLine("ERROR - Unable to create folder:  {0}", (object)str1);
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Created Folder:  {0}", relativePath + "/" + fdata[x]);
                                                            // cordner customisation
                                                            
                                                            if (x == 0)
                                                            {
                                                                //// cordner customisation
                                                                ClientGroupName = rawstr3;

                                                                CamlQuery squery2 = new CamlQuery();
                                                                squery2.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", rawstr3);
                                                                ListItemCollection clientgroupListItems = ClientGroupList.GetItems(squery2);
                                                                ctx.Load(clientgroupListItems);
                                                                try
                                                                {
                                                                    ctx.ExecuteQuery();
                                                                    if (clientgroupListItems.Count > 0)
                                                                    {
                                                                        foreach (ListItem oli in clientgroupListItems)
                                                                        {
                                                                            
                                                                            oli["folderLocation"] =  Configuration.o365List + relativePath + "/" + fdata[x];
                                                                            oli["ClientGroupNumber"] = clientgroupcode;
                                                                            oli.Update();
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var itemCreateInfo = new ListItemCreationInformation();
                                                                        var newItem = ClientGroupList.AddItem(itemCreateInfo);
                                                                        newItem["Title"] = rawstr3;
                                                                        newItem["ClientGroupNumber"] = clientgroupcode;
                                                                        newItem["folderLocation"] =  Configuration.o365List + relativePath + "/" + fdata[x];
                                                                        newItem.Update();
                                                                    }

                                                                    ctx.ExecuteQuery();


                                                                }
                                                                catch (Exception ex) { }

                                                                squery2 = new CamlQuery();
                                                                squery2.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", rawstr3);
                                                                clientgroupListItems = ClientGroupList.GetItems(squery2);
                                                                ctx.Load(clientgroupListItems);
                                                                cgroupid = 0;
                                                                try
                                                                {
                                                                    ctx.ExecuteQuery();
                                                                    
                                                                    if (clientgroupListItems.Count > 0)
                                                                    {
                                                                        foreach (ListItem oli in clientgroupListItems)
                                                                        {
                                                                            cgroupid = Int32.Parse(oli["ID"].ToString());
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        cgroupid = 0;
                                                                    }

                                                                    ctx.ExecuteQuery();


                                                                }
                                                                catch (Exception ex) { }

                                                                /// create the sub folders
                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "Corro - Group");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "Final Accounts");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "Permanent");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "Planning");

                                                            }
                                                            else if (x == 1)
                                                            {
                                                                CamlQuery squery3 = new CamlQuery();
                                                                squery3.ViewXml = String.Format("<View><Query><Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where></Query></View>", rawstr3);
                                                                ListItemCollection clientListItems = ClientList.GetItems(squery3);
                                                                ctx.Load(clientListItems);
                                                                try
                                                                {
                                                                    ctx.ExecuteQuery();
                                                                    if (clientListItems.Count > 0)
                                                                    {
                                                                        foreach (ListItem oli in clientListItems)
                                                                        {
                                                                            oli["ClientGroup"] = ClientGroupName;
                                                                            oli["folderLocation"] =  Configuration.o365List + relativePath + "/" + fdata[x];
                                                                            oli["ClientNumber"] = clientcode;
                                                                            oli.Update();
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        var itemCreateInfo = new ListItemCreationInformation();
                                                                        var newItem = ClientList.AddItem(itemCreateInfo);
                                                                        newItem["Title"] = rawstr3;
                                                                        newItem["ClientNumber"] = clientcode;
                                                                        newItem["ClientGroup"] = cgroupid;
                                                                        newItem["folderLocation"] =  Configuration.o365List + relativePath + "/" + fdata[x]; ;
                                                                        newItem.Update();
                                                                    }

                                                                    ctx.ExecuteQuery();
                                                                }
                                                                catch (Exception ex) { }
                                                                /// make all the client folders
                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "Advisory");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "Corro - Entity");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "Permanent");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "Register");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/Register","Constitution Deeds");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/Register", "Minutes");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/Register", "ASIC Docs");

                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "2019");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/2019", "Bookkeeping");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/2019", "BAS IAS");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/2019", "Client Data");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/2019", "Workpapers");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x], "2020");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/2020", "Bookkeeping");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/2020", "BAS IAS");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/2020", "Client Data");
                                                                makeafolder(ctx, relativePath + "/" + fdata[x] + "/2020", "Workpapers");

                                                            }
                                                        }
                                                    }
                                                    str1 = str1 + "/" + fdata[x];
                                                }
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("ERROR: 3016: " + ex.Message);
                                    Console.WriteLine("Something wrong with your field list: " + Configuration.metadatafoldersfieldlist);
                                    num2 = 0;
                                }
                                string name = file.Name;
                                string str4 = baseurl;
                                string str5 = str1.Length <= 0 ? str4 + "/" : str4 + str1 + "/";
                                if (Configuration.skiproot && str1.Length == 0)
                                {
                                    num9 = 0;
                                    Program.logit("Skip", file.ServerRelativeUrl, str5 + name, file.ServerRelativeUrl + " -> " + str5 + name);
                                    Console.WriteLine("Skipped:  {0} -> {1}", (object)file.ServerRelativeUrl, (object)(str5 + name));
                                }
                                if (num9 == 1)
                                {
                                    try
                                    {
                                        string str3 = string.Format("{0}/{1}", str1, sanitiseFileName(name));
                                        string serverRelativeUrl = string.Format("/{0}{1}", Configuration.o365List, str3);
                                        if (Configuration.o365subsite.Length > 0)
                                        {
                                            serverRelativeUrl = "/" + Configuration.o365subsite + serverRelativeUrl;
                                        }
                                        if (!Configuration.overwrite)
                                        {
                                            if (!Program.TryGetFileByServerRelativeUrl(ctx.Web, serverRelativeUrl))
                                            {
                                                file.CopyTo(str5 + name, Configuration.overwrite);
                                                ctx.ExecuteQuery();
                                                Program.logit("Normal", file.ServerRelativeUrl, str5 + name, "(docid:" + curid + ")" + file.ServerRelativeUrl + " -> " + str5 + name);
                                                Console.WriteLine("(docid: " + curid + ")Copied:  {0} -> {1}", (object)file.ServerRelativeUrl, (object)(str5 + name));
                                            }
                                            else
                                            {
                                                Program.logit("Skip", file.ServerRelativeUrl, str5 + name, "destination file already exists");
                                                Console.WriteLine("Skipped:  {0} -> {1}", (object)file.ServerRelativeUrl, (object)(str5 + name));
                                            }
                                        }
                                        else
                                        {
                                            
                                            file.CopyTo(str5 + name, Configuration.overwrite);
                                            ctx.ExecuteQuery();
                                            Program.logit("Normal", file.ServerRelativeUrl, str5 + name, file.ServerRelativeUrl + " -> " + str5 + name);
                                            Console.WriteLine("(docid: " + curid + ")Copied:  {0} -> {1}", (object)file.ServerRelativeUrl, (object)(str5 + name));

                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("ERROR: 3021: " + ex.Message);
                                        Console.WriteLine("unable to copy to: " + baseurl + "/" + str1 + "/" + name);
                                        Program.logError(file.ServerRelativeUrl, str5 + name, "ERROR: 3021: unable to cpoy file: " + str5 + name + " -docid: " + curid + "  subsite:" + Configuration.o365subsite + ", targetList:" + Configuration.o365List);
                                        num2 = 0;
                                    }
                                }
                            }
                            if (curid == lastid)
                            {
                                continuer = 0;
                                upto = curid;
                            }
                            else if (curid > lastid)
                            {
                                continuer = 0;
                                upto = curid;
                            }
                        }
                    }
                }
            }
            return 1;
        }
        private static bool getMetadataFields(string fldlist)
        {
            bool flag = true;
            if (fldlist.Length > 0)
            {
                if (fldlist.Contains(","))
                {
                    try
                    {
                        Configuration.metadatafolderfields = fldlist.Split(',');
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: 3018: " + ex.Message);
                        Console.WriteLine("there was an issue with splitting of field names");
                        flag = false;
                    }
                }
                else
                    Configuration.metadatafolderfields = new List<string>()
          {
            fldlist
          }.ToArray();
            }
            else
                flag = false;
            return flag;
        }
        private static void useSPList(ClientContext ctx)
        {
            List list;
            if (Configuration.listGUID != "")
            {
                list = ctx.Web.Lists.GetById(new Guid(Configuration.listGUID));
            }
            else
            {
                list = ctx.Web.Lists.GetByTitle(Configuration.sourcePSList);
            }
            ctx.Load(list);
            CamlQuery query = new CamlQuery();
            query.ViewXml = "<View><RowLimit>4999</RowLimit></View>";
            ListItemCollection listItems = list.GetItems(query);
            ctx.Load(listItems);
            try
            {
                ctx.ExecuteQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: 1015: " + ex.Message);
                Console.WriteLine("Please ensure that your SharePoint list contains these fields: targetDocLibrary, targetsiteurl, localSourceLocation");
            }

            string splist = "";
            string localsource = "";
            string spsite = "";
            string subsite = "";
            string defaultList = Configuration.o365List;

            foreach (ListItem oListItem in listItems)
            {
                // Console.WriteLine("ID: {0} \nTitle: {1} ", oListItem.Id, oListItem["Title"]);
                try
                {
                    splist = oListItem[Configuration.sourceSPList_DocLibraryFieldName].ToString();
                }
                catch (Exception e)
                {
                    splist = defaultList;
                }
                try
                {
                    spsite = oListItem[Configuration.sourceSPList_SubSiteLocationFieldName].ToString();
                }
                catch (Exception e){
                    spsite = "";
                }
                try
                {
                    


                    if (spsite.Length == 0)
                    {
                        if (Configuration.createsiteifnotexists == true)
                        {
                            string sitename = oListItem["Title"].ToString();

                            WebCreationInformation nsite = new WebCreationInformation();
                            nsite.Url = sitename;
                            nsite.WebTemplate = "{0AD4E635-2100-44B0-92CA-D3ED976FB1A8}#projectTemplate";
                            nsite.Url = sitename.Replace(" ","");
                            nsite.Description = sitename;
                            nsite.Title = sitename;
                            nsite.UseSamePermissionsAsParentSite = true;
                            if (!checkSiteExists(ctx, Configuration.coreSiteURL, nsite.Url)) ;
                            {
                                ctx.Site.RootWeb.Webs.Add(nsite);
                                ctx.ExecuteQuery();
                            }
                            oListItem[Configuration.sourceSPList_SubSiteLocationFieldName] = Configuration.coreSiteURL + "/" + nsite.Url;
                            oListItem.Update();
                            ctx.Load(oListItem);
                            ctx.ExecuteQuery();
                            spsite = oListItem[Configuration.sourceSPList_SubSiteLocationFieldName].ToString();

                        }
                    }

                    localsource = oListItem[Configuration.sourceSPList_SourceLocation].ToString();
                    string[] urlstring = spsite.Split(new string[] { "/" }, StringSplitOptions.None);
                    int x = 0;
                    string middle = "";
                    subsite = "";
                    if (urlstring.Length > 3)
                    {
                        for (x = 3; x < urlstring.Length; x++)
                        {
                            subsite = subsite + middle + urlstring[x];
                            middle = "/";
                        }
                    }
                    if (splist.Length > 0 && spsite.Length > 0 && localsource.Length > 0)
                    {
                        Console.WriteLine("about to start site localsource: " + localsource + ", subsite:" + spsite + ", targetList:" + splist);
                        logit("Normal Subsite", spsite + "/" + splist, localsource, "about to start site localsource: " + localsource + ", subsite:" + spsite + ", targetList:" + splist);
                        Console.WriteLine("connecting to site: " + spsite);
                        //Configuration.o365SiteURL = spsite;
                        Configuration.localSource = localsource;
                        Configuration.o365List = splist;
                        Configuration.o365subsite = subsite;
                        String tmpurl = Configuration.o365SiteURL;
                        Configuration.o365SiteURL = Configuration.o365SiteURL + "/" + subsite;

                        /// create the site if it doesn't exist.




                        //ClientContext o365context = Configuration.GetUserContext(spsite);
                        if (Directory.Exists(localsource))
                        {
                            ClientContext ctx2 = Configuration.GetUserContext();
                            doFolder(ctx2, localsource, "", "", subsite + "/" + splist);
                        }
                        else
                        {
                            logError("", "", "ERROR: 1091: localsource does not exist: " + localsource + ", subsite:" + spsite + ", targetList:" + splist);
                            logit("Error", "", "", "ERROR: 1091: localsource does not exist: " + localsource + ", subsite:" + spsite + ", targetList:" + splist);
                            Console.WriteLine("ERROR: 1091: localsource does not exist: " + localsource + ", subsite:" + spsite + ", targetList:" + splist);
                        }
                        Configuration.o365SiteURL = tmpurl;
                    }
                    else
                    {
                        logError("", "", "ERROR: 1029: list does not contain the correct data- localsource: " + localsource + ", subsite:" + spsite + ", targetList:" + splist);
                        logit("Error", "", "", "ERROR: 1029: list does not contain the correct data- localsource: " + localsource + ", subsite:" + spsite + ", targetList:" + splist);
                        Console.WriteLine("ERROR: 1029: list does not contain the correct data- localsource: " + localsource + ", subsite:" + spsite + ", targetList:" + splist);
                    }
                    //targetsiteurl, targetDocLibrary and localSourceLocation
                }
                catch (Exception e)
                {
                    logError("", "", "ERROR: 1021: there is an issue with the list configuration.please ensure the feilds are called: targetsiteurl, targetDocLibrary and localSourceLocation to use this option - error" + e.Message);
                    logit("Error", "", "", "ERROR: 1021: there is an issue with the list configuration.please ensure the feilds are called: targetsiteurl, targetDocLibrary and localSourceLocation to use this option - error" + e.Message);
                    Console.WriteLine("ERROR: 1021: there is an issue with the list configuration.  please ensure the feilds are called: targetsiteurl, targetDocLibrary and localSourceLocation to use this option - error: " + e.Message);
                }
            }
        }
        private static void doFolder(ClientContext o365Context, string d, string thisfolder, string o365ParentFolder, string o365DocumentList)
        {
            string tmpfldforerror = "";
            thisfolder = sanitiseFileName(thisfolder);
            string curfolder = thisfolder;
            try
            {
                tmpfldforerror = d;
                if (!FolderExists(o365Context, o365DocumentList + o365ParentFolder + "/" + thisfolder))
                {
                    curfolder = CreateFolder(o365Context, Configuration.o365SiteURL, o365DocumentList, o365ParentFolder, thisfolder, Configuration.listGUID);
                }
                if (curfolder != null)
                {
                    thisfolder = curfolder;
                    string dirPath = d;
                    List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
                    string childfld = "";
                    foreach (var dir in dirs)
                    {
                        if (Console.KeyAvailable)
                        {
                            if (Console.ReadKey(true).Key == ConsoleKey.P)
                            {
                               Console.Write("pausing, press enter to continue");
                                Console.ReadLine();
                            }
                        }
                        if (Configuration.resumed == false)
                        {
                            if (Configuration.startfrom == dir)
                            {
                                Configuration.resumed = true;
                            }
                        }
                        try
                        {
                            tmpfldforerror = dir;
                            childfld = sanitiseFileName(dir.Substring(dir.LastIndexOf("\\") + 1));
                            string tmpParFolder = "";
                            if (thisfolder != "")
                            {
                                tmpParFolder = o365ParentFolder + "/" + thisfolder;
                            }
                            else
                            {
                                tmpParFolder = o365ParentFolder;
                            }
                            logit("Normal", dir, tmpParFolder + "/" + childfld, "folder: " + dir + " -> " + tmpParFolder + "/" + childfld);
                            Console.WriteLine("copying {0} => {1}", dir, tmpParFolder + "/" + childfld);
                            if (foldernameMatch(dir.ToString()))
                            {
                                logit("Normal", dir.ToString(), "", " skipping folder -> excluded");
                                Console.WriteLine("Skipping folder {0} ::  matched in exclude list", dir.ToString());
                            }
                            else
                            {
                                doFolder(o365Context, dir.ToString(), childfld, tmpParFolder, o365DocumentList);
                            }
                        }
                        catch (UnauthorizedAccessException UAEx)
                        {
                            logit("Error", tmpfldforerror, "", UAEx.Message + ":" + tmpfldforerror);
                            //correct one
                            logError(tmpfldforerror, "", UAEx.Message + ":" + tmpfldforerror);
                            logPermissionError(tmpfldforerror);
                            Console.WriteLine(UAEx.Message + ":" + tmpfldforerror);
                            tmpfldforerror = "";
                        }
                        catch (PathTooLongException PathEx)
                        {
                            logError(tmpfldforerror, "", PathEx.Message + ":" + tmpfldforerror);
                            logit("Error", tmpfldforerror, "", PathEx.Message + ":" + tmpfldforerror);
                            Console.WriteLine(PathEx.Message + ":" + tmpfldforerror);
                            tmpfldforerror = "";
                        }
                    }
                }
                else
                {
                    logit("Error", tmpfldforerror, o365DocumentList + "/" + o365ParentFolder + "/" + thisfolder, "aborting as unable to create folder: " + o365DocumentList + "/" + o365ParentFolder + "/" + thisfolder);
                    Console.WriteLine("aborting as unable to create folder: " + o365DocumentList + "/" + o365ParentFolder + "/" + thisfolder);
                }
            }
            catch (UnauthorizedAccessException UAEx)
            {
                logit("Error", tmpfldforerror, "", UAEx.Message + ":" + tmpfldforerror);
                logError(tmpfldforerror, "", UAEx.Message + ":" + tmpfldforerror);
                logPermissionError(tmpfldforerror);
                Console.WriteLine(UAEx.Message + ":" + tmpfldforerror);
                tmpfldforerror = "";
            }
            catch (PathTooLongException PathEx)
            {
                logError(tmpfldforerror, "", PathEx.Message + ":" + tmpfldforerror);
                logFileTooLongError(tmpfldforerror, "", tmpfldforerror);
                logit("Error", tmpfldforerror, "", PathEx.Message + ":" + tmpfldforerror);
                Console.WriteLine(PathEx.Message + ":" + tmpfldforerror);
                tmpfldforerror = "";
            }
            if (curfolder != null)
            {
                string tmpfilenameforerror = "";
                try
                {
                    string tmpParFolder = "";
                    if (thisfolder != "")
                    {
                        tmpParFolder = o365ParentFolder + "/" + thisfolder;
                    }
                    else
                    {
                        tmpParFolder = o365ParentFolder;
                    }
                    string dirPath = d;
                    List<string> files = new List<string>(Directory.EnumerateFiles(dirPath));
                    string flename = "";
                    long lnth = 0;
                    Boolean ro = false;
                    foreach (var fle in files)
                    {
                        if (Console.KeyAvailable)
                        {
                            if (Console.ReadKey(true).Key == ConsoleKey.P)
                            {
                                Console.Write("pausing, press enter to continue");
                                Console.ReadLine();
                            }
                        }
                        tmpfilenameforerror = fle;
                        lnth = 0;
                        FileInfo f;
                        //FileInfo f = new FileInfo(fle);
                        f = new FileInfo(fle);
                        try
                        {
                            
                            lnth = ((f.Length / 1024));
                        }
                        catch (Exception ex)
                        {
                        }
                        
                        

                        if (f.LastWriteTime > Configuration.dt)
                        {
                            flename = sanitiseFileName(fle.Substring(fle.LastIndexOf("\\") + 1));
                            if (fle.Length < 255)
                            {
                                if (!filenameMatch(f.Name.ToLower(),f.FullName.ToLower()))
                                {
                                    if (f.IsReadOnly)
                                    {
                                        ro = true;
                                        f.IsReadOnly = false;
                                    }
                                    else
                                    {
                                        ro = false;
                                    }

                                    if (Configuration.resumed)
                                    {
                                        if (Configuration.overwrite == false)
                                        {
                                            //var list = o365Context.Web.Lists.GetByTitle(o365DocumentList);
                                            //var list = web.Lists.GetByTitle(listTitle);
                                            //o365Context.Load(list, l => l.ParentWeb.ServerRelativeUrl);
                                            //o365Context.ExecuteQuery();
                                            string tmpFileName = string.Format("{0}/{1}", tmpParFolder, sanitiseFileName(flename));
                                            //string tmpFileName = sanitiseFileName(flename);
                                           //tmpFileName =  string.Format("{0}/{1}{2}", Configuration.o365SiteURL, o365DocumentList, tmpFileName);
                                            tmpFileName = string.Format("/{0}{1}", o365DocumentList, tmpFileName);
                                            //tmpFileName = string.Format("{0}", tmpFileName);
                                            //tmpFileName = string.Format("/{0}{1}", tmpParFolder, tmpFileName);
                                            if (!TryGetFileByServerRelativeUrl(o365Context.Web, tmpFileName))
                                            //if (!FileExists(list, tmpFileName))
                                            {
                                                Console.WriteLine("file {0} kb | {1} -> {2}", lnth, fle, tmpParFolder + "/" + flename);
                                                logit("Normal", fle, tmpParFolder + "/" + flename, lnth + " " + fle + " -> " + tmpParFolder + "/" + flename);
                                                saveFileto365(o365Context, o365DocumentList, fle, tmpParFolder + "/", flename);
                                                if (Configuration.setEditDate == true && Configuration.setAuthor==false)
                                                {
                                                    setEditDate(o365Context, "/" + o365DocumentList + tmpParFolder + "/" + flename, f.CreationTime, f.LastWriteTime);
                                                }else if (Configuration.setAuthor == true)
                                                {
                                                    string modby = "";
                                                    try
                                                    {
                                                        modby = getAuthorName(f.FullName);
                                                        
                                                        if (modby == "0")
                                                        {
                                                            logitUnknownAuthor("None found on file", f.FullName);
                                                            modby = "";
                                                        }
                                                    }catch(Exception ex)
                                                    {
                                                        modby = "";
                                                    }
                                                    if (Configuration.translateUsernameFile.Length > 0)
                                                    {
                                                        modby = tranlateUser(modby);
                                                    }


                                                    if (modby == "" && Configuration.translateUsernameFile.Length > 0)
                                                    {
                                                        {
                                                            try
                                                            {
                                                                FileSecurity fs = f.GetAccessControl();
                                                                modby = fs.GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
                                                                modby = tranlateUser(modby);
                                                            }
                                                            catch (Exception ex)
                                                            {

                                                            }
                                                        }
                                                    }
                                                    if (modby.Length > 0)
                                                    {

                                                        User spuser = o365Context.Web.EnsureUser(modby); // This will give exception if user does not exist.
                                                        try
                                                        {
                                                            o365Context.Load(spuser);
                                                            o365Context.ExecuteQuery();
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Console.WriteLine("Unable to set Author:" + modby);
                                                            logitUnknownAuthor(modby, f.FullName);
                                                            modby = "0";
                                                        }
                                                        if (modby != "0")
                                                        {
                                                            try
                                                            {
                                                                setCreateData2(o365Context, "/" + o365DocumentList + tmpParFolder + "/" + flename, f.CreationTime, spuser, f.LastWriteTime, spuser);
                                                            }
                                                            catch (Exception e)
                                                            {
                                                                Console.WriteLine("Unable to set Author:" + modby);
                                                                logit("Error", f.FullName, "/" + o365DocumentList + tmpParFolder + "/" + flename, "Unable to set Author and dates: " + e.Message.ToString());
                                                                logError(f.FullName, "/" + o365DocumentList + tmpParFolder + "/" + flename, "Unable to set Author and dates: " + e.Message.ToString());
                                                            }
                                                        }

                                                    }
                                                    
                                                }

                                            }
                                            else
                                            {
                                                Console.WriteLine("Skip file already exists {0} kb | {1} -> {2}", lnth, fle, tmpParFolder + "/" + flename);
                                                logit("Skip", fle, tmpParFolder + "/" + flename, "File already exists: " + lnth + " " + fle + " -> " + tmpParFolder + "/" + flename);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("file {0} kb | {1} -> {2}", lnth, fle, tmpParFolder + "/" + flename);
                                            logit("Normal", fle, tmpParFolder + "/" + flename, lnth + " " + fle + " -> " + tmpParFolder + "/" + flename);
                                            saveFileto365(o365Context, o365DocumentList, fle, tmpParFolder + "/", flename);
                                            if (Configuration.setEditDate == true)
                                            {
                                                setEditDate(o365Context, "/" + o365DocumentList + tmpParFolder + "/" + flename, f.CreationTime, f.LastWriteTime);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("SKIP file {0} kb | {1} ", lnth, fle);
                                    }
                                    if (ro)
                                    {
                                        f.IsReadOnly = true;
                                        ro = false;
                                    }
                                }
                                else
                                {
                                    logit("Exclude", fle, "", lnth + " skipping file" + fle + " -> excluded");
                                    Console.WriteLine("Skipping file {0} exclude list: {1} ", fle, f.Name);
                                }
                            }
                            else
                            {
                                /// filename is too long so log it as a filename error
                                logit("Error", fle, "", "filename too long: " + fle.Length + ":" + fle);
                                logFileTooLongError(fle, fle, fle.Length + ":" + fle);
                                Console.WriteLine("Skipping file {0} to long: {1} chars", fle, fle.Length);
                            }
                        }
                        else
                        {
                            logit("Skip", fle, "", lnth + " skipping file" + fle + " -> date is older");
                            Console.WriteLine("Skipping file: {0} - date is older", fle);
                        }
                    }
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    logit("Error", tmpfilenameforerror, "", UAEx.Message + ":" + tmpfilenameforerror);
                    logError(tmpfilenameforerror, "", UAEx.Message + ":" + tmpfilenameforerror);
                    logPermissionError(tmpfilenameforerror);
                    Console.WriteLine(UAEx.Message + ":" + tmpfilenameforerror);
                    tmpfilenameforerror = "";
                }
                catch (PathTooLongException PathEx)
                {
                    logError(tmpfilenameforerror, "", PathEx.Message + ":" + tmpfilenameforerror);
                    logit("Error", tmpfilenameforerror, "", PathEx.Message + ":" + tmpfilenameforerror);
                    Console.WriteLine(PathEx.Message + ":" + tmpfilenameforerror);
                }
                catch (Exception ex)
                {
                    logError(tmpfilenameforerror, "", ex.Message + ":" + tmpfilenameforerror);
                    logit("Error", tmpfilenameforerror, "", ex.Message + ":" + tmpfilenameforerror);
                    Console.WriteLine(ex.Message + ":" + tmpfilenameforerror);
                }
            }
        }
        private static void doFolderWithSourceList(ClientContext desctx, ClientContext sourcectx, string d, string thisfolder, string o365ParentFolder, string o365DocumentList, Folder folder, int depth=0)
        {


            sourcectx.Load(folder);
            sourcectx.Load(folder.Folders);
            sourcectx.Load(folder.Files);
            sourcectx.Load(folder.Files);
            Boolean foundit = true;
            try
            {
                sourcectx.ExecuteQuery();
            }
            catch (System.Net.WebException wex)
            {
                var response = wex.Response as System.Net.HttpWebResponse;
                if (response != null && (response.StatusCode == (System.Net.HttpStatusCode)429 || response.StatusCode == (System.Net.HttpStatusCode)503))
                {
                    logit("Notice:", d,"","Throttling - Notice 1001");
                    Console.WriteLine("Throttling - Notice 1001");

                    var retryAfter = Int32.Parse(response.Headers["Retry-After"].ToString());
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(retryAfter));
                    try
                    {
                        sourcectx.ExecuteQuery();
                    }
                    catch (Exception ex2)
                    {
                        foundit = false;
                        logit("Error", "Error finding Source File:" + d, "", ex2.Message + "");
                        logError(d, "", ex2.Message + "");
                        Console.WriteLine(ex2.Message + ":" + d);
                    }
                }
                else
                {
                    foundit = false;
                    logit("Error", "Error finding Source File:" + d, "", wex.Message + "");
                    logError(d, "", wex.Message + "");
                    Console.WriteLine(wex.Message + ":" + d);
                }
            }
            catch (Exception ex)
            {
                foundit = false;
                logit("Error", "Error finding Source File:" + d, "", ex.Message + "");
                logError(d, "", ex.Message + "");
                Console.WriteLine(ex.Message + ":" + d);
            }

            // todo add exlude folder here.

            if (foundit)
            {

                
                string tmpfldforerror = "";
                thisfolder = sanitiseFileName(thisfolder);
                string curfolder = thisfolder;
                tmpfldforerror = d;
                if (Configuration.skipfoldercreate == 0)
                {
                    if (!FolderExists(desctx, o365DocumentList + o365ParentFolder + "/" + thisfolder))
                    {
                        if (Configuration.skipfoldercreate == 0)
                        {
                            curfolder = CreateFolder(desctx, Configuration.o365SiteURL, o365DocumentList, o365ParentFolder, thisfolder, Configuration.listGUID);
                        }
                        else
                        {
                            logit("Notice", thisfolder, o365DocumentList + o365ParentFolder + "/" + thisfolder, "folder doesn't exist: " + o365DocumentList + o365ParentFolder + "/" + thisfolder);
                            Console.WriteLine(thisfolder, "folder doesn't exist: " + o365DocumentList + o365ParentFolder + "/" + thisfolder);
                        }

                    }
                }
                

                try
                {

                    if (curfolder != null)
                    {
                        thisfolder = curfolder;
                        string dirPath = d;
                        //List<string> dirs = new List<string>(Directory.EnumerateDirectories(dirPath));
                        string childfld = "";
                        //foreach (var dir in dirs)

                        foreach (Folder f in folder.Folders)
                        {
                            if (Configuration.resumed == false)
                            {
                                if (Configuration.startfrom == f.ServerRelativeUrl)
                                {
                                    Console.WriteLine("RESUME");
                                    Configuration.resumed = true;
                                }
                            }
                            if (Configuration.resumed == false)
                            {
                                Console.WriteLine("Skipping Folder:" + f.ServerRelativeUrl);
                            }

                            if (Configuration.resumed == true)
                            {
                                string dir = f.Name;
                                Boolean skip = false;
                                try
                                {
                                    sourcectx.Load(f.ListItemAllFields);
                                    sourcectx.ExecuteQuery();
                                }
                                catch (System.Net.WebException wex)
                                {
                                    var response = wex.Response as System.Net.HttpWebResponse;
                                    if (response != null && (response.StatusCode == (System.Net.HttpStatusCode)429 || response.StatusCode == (System.Net.HttpStatusCode)503))
                                    {
                                        logit("Notice:", d, "", "Throttling - Notice 1002");
                                        Console.WriteLine("Throttling - Notice 1002");

                                        var retryAfter = Int32.Parse(response.Headers["Retry-After"].ToString());
                                        System.Threading.Thread.Sleep(TimeSpan.FromSeconds(retryAfter));
                                        try
                                        {
                                            sourcectx.ExecuteQuery();
                                        }
                                        catch (Exception ex2)
                                        {
                                            foundit = false;
                                            logit("Error", "error getting source folder line 3343:" + f.Name, "", ex2.Message + "");
                                            logError(d, "", ex2.Message + "");
                                            Console.WriteLine(ex2.Message + ":" + d);
                                        }
                                    }
                                    else
                                    {
                                        logit("Error", "error getting source folder line 3343:" + f.Name, "", wex.Message + ": f.Name");
                                        logError(f.Name, "", wex.Message + ":" + f.Name);
                                        Console.WriteLine(wex.Message + ":" + f.Name);
                                        tmpfldforerror = "";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    logit("Error", "error getting source folder line 3343:" + f.Name, "", ex.Message + ": f.Name");
                                    logError(f.Name, "", ex.Message + ":" + f.Name);
                                    Console.WriteLine(ex.Message + ":" + f.Name);
                                    tmpfldforerror = "";
                                }

                                if (f.Name == "Forms")
                                {
                                    Console.WriteLine("Skipping Forms Folder");
                                    skip = true;
                                }

                                if (Console.KeyAvailable)
                                {
                                    if (Console.ReadKey(true).Key == ConsoleKey.P)
                                    {
                                        Console.Write("pausing, press enter to continue");
                                        Console.ReadLine();
                                    }
                                }
                                if (Configuration.resumed == false)
                                {
                                    if (Configuration.startfrom == f.ServerRelativeUrl)
                                    {
                                        Configuration.resumed = true;
                                    }
                                }

                                if (skip == false)
                                {
                                    try
                                    {
                                        tmpfldforerror = dir;
                                        childfld = sanitiseFileName(dir.Substring(dir.LastIndexOf("\\") + 1));
                                        string tmpParFolder = "";
                                        if (thisfolder != "")
                                        {
                                            tmpParFolder = o365ParentFolder + "/" + thisfolder;
                                        }
                                        else
                                        {
                                            tmpParFolder = o365ParentFolder;
                                        }
                                        logit("Normal", dir, tmpParFolder + "/" + childfld, "folder: " + dir + " -> " + tmpParFolder + "/" + childfld);
                                        Console.WriteLine("copying {0} => {1}", dir, tmpParFolder + "/" + childfld);
                                        if (foldernameMatch(f.ServerRelativeUrl))
                                        {
                                            logit("Exclude", dir.ToString(), "", " skipping folder -> excluded");
                                            Console.WriteLine("Skipping folder {0} ::  matched in exclude list", dir.ToString());
                                        }
                                        else
                                        {
                                            if (Configuration.limitdepth > 0)
                                            {
                                                if (depth < Configuration.limitdepth)
                                                {
                                                    doFolderWithSourceList(desctx, sourcectx, dir.ToString(), childfld, tmpParFolder, o365DocumentList, f, depth+1);
                                                }
                                            }
                                            else
                                            {
                                                doFolderWithSourceList(desctx, sourcectx, dir.ToString(), childfld, tmpParFolder, o365DocumentList, f, depth+1);
                                            }
                                        }
                                    }
                                    catch (UnauthorizedAccessException UAEx)
                                    {
                                        logit("Error", tmpfldforerror, "", UAEx.Message + ":" + tmpfldforerror);
                                        //correct one
                                        logError(tmpfldforerror, "", UAEx.Message + ":" + tmpfldforerror);
                                        logPermissionError(tmpfldforerror);
                                        Console.WriteLine(UAEx.Message + ":" + tmpfldforerror);
                                        tmpfldforerror = "";
                                    }
                                    catch (PathTooLongException PathEx)
                                    {
                                        logError(tmpfldforerror, "", PathEx.Message + ":" + tmpfldforerror);
                                        logit("Error", tmpfldforerror, "", PathEx.Message + ":" + tmpfldforerror);
                                        Console.WriteLine(PathEx.Message + ":" + tmpfldforerror);
                                        tmpfldforerror = "";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        logit("Error", tmpfldforerror, o365DocumentList + "/" + o365ParentFolder + "/" + thisfolder, "aborting as unable to create folder: " + o365DocumentList + "/" + o365ParentFolder + "/" + thisfolder);
                        Console.WriteLine("aborting as unable to create folder: " + o365DocumentList + "/" + o365ParentFolder + "/" + thisfolder);
                    }
                }
                catch (UnauthorizedAccessException UAEx)
                {
                    logit("Error", tmpfldforerror, "", UAEx.Message + ":" + tmpfldforerror);
                    logError(tmpfldforerror, "", UAEx.Message + ":" + tmpfldforerror);
                    logPermissionError(tmpfldforerror);
                    Console.WriteLine(UAEx.Message + ":" + tmpfldforerror);
                    tmpfldforerror = "";
                }
                catch (PathTooLongException PathEx)
                {
                    logError(tmpfldforerror, "", PathEx.Message + ":" + tmpfldforerror);
                    logFileTooLongError(tmpfldforerror, "", tmpfldforerror);
                    logit("Error", tmpfldforerror, "", PathEx.Message + ":" + tmpfldforerror);
                    Console.WriteLine(PathEx.Message + ":" + tmpfldforerror);
                    tmpfldforerror = "";
                }

                if (curfolder != null)
                {
                    string tmpfilenameforerror = "";
                    try
                    {
                        string tmpParFolder = "";
                        if (thisfolder != "")
                        {
                            tmpParFolder = o365ParentFolder + "/" + thisfolder;
                        }
                        else
                        {
                            tmpParFolder = o365ParentFolder;
                        }
                        string dirPath = d;
                       
                        string flename = "";
                        long lnth = 0;
                        Boolean ro = false;
                        //foreach (var fle in files)
                        foreach(var fle in folder.Files)
                        {
                            
                            if (Console.KeyAvailable)
                            {
                                if (Console.ReadKey(true).Key == ConsoleKey.P)
                                {
                                    Console.Write("pausing, press enter to continue");
                                    Console.ReadLine();
                                }
                            }
                            tmpfilenameforerror = fle.Name;
                            lnth = 0;
                            
                            try
                            {
                                lnth = ((fle.Length / 1024));
                            }
                            catch (Exception ex)
                            {
                            }

                            if (fle.TimeLastModified > Configuration.dt)
                            {
                                if (flename.Contains("#"))
                                {
                                    Console.WriteLine("Has a hash");
                                }
                                flename = sanitiseFileName(fle.Name.Substring(fle.Name.LastIndexOf("\\") + 1));
                                //// NOTE TO SELF. should we add this back in?
                                //if (fle.Length < 10000)
                                //{
                                    if (!filenameMatch(fle.Name.ToLower()))
                                    {
                                        ro = false;

                                        if (Configuration.resumed)
                                        {
                                            if (Configuration.overwrite == false)
                                            {
                                                //var list = o365Context.Web.Lists.GetByTitle(o365DocumentList);
                                                //var list = web.Lists.GetByTitle(listTitle);
                                                //o365Context.Load(list, l => l.ParentWeb.ServerRelativeUrl);
                                                //o365Context.ExecuteQuery();
                                                string tmpFileName = string.Format("{0}/{1}", tmpParFolder, sanitiseFileName(flename));
                                                //string tmpFileName = sanitiseFileName(flename);
                                                //tmpFileName =  string.Format("{0}/{1}{2}", Configuration.o365SiteURL, o365DocumentList, tmpFileName);
                                                tmpFileName = string.Format("/{0}{1}", o365DocumentList, tmpFileName);
                                                //tmpFileName = string.Format("{0}", tmpFileName);
                                                //tmpFileName = string.Format("/{0}{1}", tmpParFolder, tmpFileName);
                                                if (!TryGetFileByServerRelativeUrl(desctx.Web, tmpFileName))
                                                //if (!FileExists(list, tmpFileName))
                                                {
                                                    Console.WriteLine("file {0} kb | {1} -> {2}", lnth, fle, tmpParFolder + "/" + flename);
                                                    logit("Normal", fle.ServerRelativeUrl, tmpParFolder + "/" + flename, lnth + " " + fle + " -> " + tmpParFolder + "/" + flename);
                                                    //fle.CopyTo(tmpParFolder + "/" + flename, Configuration.overwrite);
                                                    copyFile(desctx,sourcectx, o365DocumentList, fle.Name, tmpParFolder + "/", flename,fle);
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Skip file already exists {0} kb | {1} -> {2}", lnth, fle, tmpParFolder + "/" + flename);
                                                    logit("Skip", fle.ServerRelativeUrl, tmpParFolder + "/" + flename, "File already exists: " + lnth + " " + fle + " -> " + tmpParFolder + "/" + flename);
                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine("file {0} kb | {1} -> {2}", lnth, fle, tmpParFolder + "/" + flename);
                                                logit("Normal", fle.ServerRelativeUrl, tmpParFolder + "/" + flename, lnth + " " + fle + " -> " + tmpParFolder + "/" + flename);
                                                copyFile(desctx, sourcectx, o365DocumentList, fle.Name, tmpParFolder + "/", flename,fle);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("SKIP file {0} kb | {1} ", lnth, fle);
                                        }
                                     }
                                    else
                                    {
                                        logit("Exclude", fle.ServerRelativeUrl, "", lnth + " skipping file" + fle + " -> excluded");
                                        Console.WriteLine("Skipping file {0} exclude list: {1} ", fle.ServerRelativeUrl, folder.Name);
                                    }
                                //}
                                //else
                                //{
                                //    /// filename is too long so log it as a filename error
                                //    logit("Error", fle.ServerRelativeUrl, "", "filename too long: " + fle.Length + ":" + fle);
                                //    logFileTooLongError(fle.ServerRelativeUrl, fle.Name, fle.Length + ":" + fle);
                                //    Console.WriteLine("Skipping file {0} to long: {1} chars", fle, fle.Length);
                                //}
                            }
                            else
                            {
                                logit("Skip", fle.ServerRelativeUrl, "", lnth + " skipping file" + fle.Name + " -> date is older");
                                Console.WriteLine("Skipping file: {0} - date is older", fle.ServerRelativeUrl);
                            }
                        }
                    }
                    catch (UnauthorizedAccessException UAEx)
                    {
                        logit("Error", tmpfilenameforerror, "", UAEx.Message + ":" + tmpfilenameforerror);
                        logError(tmpfilenameforerror, "", UAEx.Message + ":" + tmpfilenameforerror);
                        logPermissionError(tmpfilenameforerror);
                        Console.WriteLine(UAEx.Message + ":" + tmpfilenameforerror);
                        tmpfilenameforerror = "";
                    }
                    catch (PathTooLongException PathEx)
                    {
                        logError(tmpfilenameforerror, "", PathEx.Message + ":" + tmpfilenameforerror);
                        logit("Error", tmpfilenameforerror, "", PathEx.Message + ":" + tmpfilenameforerror);
                        Console.WriteLine(PathEx.Message + ":" + tmpfilenameforerror);
                    }
                }
            }
        }
        private static void GetSiteLists()
        {
            ClientContext context = Configuration.GetUserContext();
            Web oWebsite = context.Web;
            ListCollection col1List = oWebsite.Lists;
            context.Load(col1List);
            context.ExecuteQuery();
            foreach (Microsoft.SharePoint.Client.List oList in col1List)
            {
                Console.WriteLine("Title: {0} Created: {1}", oList.Title, oList.Created.ToString());
            }
        }
        private static bool LibraryExists(ClientContext o365Context, Web o365Web, string o365LibraryName)
        {
            ListCollection o365Lists = o365Web.Lists;
            IEnumerable<List> o365Results = o365Context.LoadQuery<List>(o365Lists.Where(List => List.Title == o365LibraryName));
            o365Context.ExecuteQuery();
            List o365ExistingList = o365Results.FirstOrDefault();
            if (o365ExistingList != null)
            {
                return true;
            }
            return false;
        }
        private static void CreateLibrary(ClientContext o365Context, Web o365Web, string o365LibraryName)
        {
            ListCreationInformation o365CreateInfo = new ListCreationInformation();
            o365CreateInfo.Title = o365LibraryName;
            o365CreateInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
            List o365List = o365Web.Lists.Add(o365CreateInfo);
            o365Context.ExecuteQuery();
        }
        private static bool FileExists(List list, string fileUrl)
        {
            try
            {
                var ctx = list.Context;
                var qry = new CamlQuery();
                qry.ViewXml = string.Format("<View Scope=\"RecursiveAll\"><Query><Where><Eq><FieldRef Name=\"FileRef\"/><Value Type=\"Url\">{0}</Value></Eq></Where></Query></View>", fileUrl);
                var items = list.GetItems(qry);
                ctx.Load(items);
                ctx.ExecuteQuery();
                return items.Count > 0;
            }
           catch (Exception e)
            {
                return false;
            }
        }
        private static bool TryGetFileByServerRelativeUrl(Web web, string serverRelativeUrl)//, out Microsoft.SharePoint.Client.File file)
        {
            var ctx = web.Context;
            Microsoft.SharePoint.Client.File file;
            try
            {


                file = web.GetFileByServerRelativeUrl(serverRelativeUrl);
                ctx.Load(file);
                ctx.ExecuteQuery();
                return true;
            }
            catch (System.Net.WebException wex)
            {
                var response = wex.Response as System.Net.HttpWebResponse;
                if (response != null && (response.StatusCode == (System.Net.HttpStatusCode)429 || response.StatusCode == (System.Net.HttpStatusCode)503))
                {
                    logit("Notice:", "TryGet FileByServerRelativeURL", "", "Throttling - Notice 1003");
                    Console.WriteLine("Throttling - Notice 1003");

                    var retryAfter = Int32.Parse(response.Headers["Retry-After"].ToString());
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(retryAfter));
                    try
                    {
                        file = web.GetFileByServerRelativeUrl(serverRelativeUrl);
                        ctx.Load(file);
                        ctx.ExecuteQuery();
                        return true;

                    }
                    catch (Exception ex2)
                    {
                        logError(serverRelativeUrl, serverRelativeUrl, "Error checking for file (TryGetFileByServerRelativeUrl) returning False, will attempt to upload with overwrite flag set as " + Configuration.overwrite + " -> " + ex2.Message);
                        logit("Error", serverRelativeUrl, serverRelativeUrl, "Error checking for file (TryGetFileByServerRelativeUrl) returning False, will attempt to upload with overwrite flag set as " + Configuration.overwrite + " -> " + ex2.Message);
                        return false;
                    }
                }
                else
                {
                    logError(serverRelativeUrl, serverRelativeUrl, "Error checking for file (TryGetFileByServerRelativeUrl) returning False, will attempt to upload with overwrite flag set as " + Configuration.overwrite + " -> " + wex.Message);
                    logit("Error", serverRelativeUrl, serverRelativeUrl, "Error checking for file (TryGetFileByServerRelativeUrl) returning False, will attempt to upload with overwrite flag set as " + Configuration.overwrite + " -> " + wex.Message);
                    return false;
                }
            }
            catch (Microsoft.SharePoint.Client.ServerException ex)
            {
                if (ex.ServerErrorTypeName == "System.IO.FileNotFoundException")
                {
                    file = null;
                    return false;
                }
                else
                    //Console.WriteLine("Error checking for file (TryGetFileByServerRelativeUrl) returning False, will attempt to upload with overwrite flag set as " + Configuration.overwrite + " -> " + ex.Message)
                    logError(serverRelativeUrl, serverRelativeUrl, "Error checking for file (TryGetFileByServerRelativeUrl) returning False, will attempt to upload with overwrite flag set as " + Configuration.overwrite + " -> " + ex.Message);
                logit("Error", serverRelativeUrl, serverRelativeUrl, "Error checking for file (TryGetFileByServerRelativeUrl) returning False, will attempt to upload with overwrite flag set as " + Configuration.overwrite + " -> " + ex.Message);
                return false;
            }
        }
        private static void o365SaveBinaryDirect(ClientContext o365Context, string o365LibraryName, string o365FilePath, string o365FileName)
        {
            Web o365Web = o365Context.Web;
            //if (!LibraryExists(o365Context, o365Web, o365LibraryName))
            //{
            //    CreateLibrary(o365Context, o365Web, o365LibraryName);
            //}
            using (FileStream o365FileStream = new FileStream(o365FilePath, FileMode.Open))
            {
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(o365Context, string.Format("/{0}/{1}", o365LibraryName, o365FileName), o365FileStream, true);
            }
        }
        private static void copyFile(ClientContext desctx, ClientContext sourcectx, string o365LibraryName, string srcFilePath, string o365FilePath, string o365FileName,Microsoft.SharePoint.Client.File fle)
        {

            //Web o365Web = o365Context.Web;

            /*if (!LibraryExists(o365Context, o365Web, o365LibraryName))
            {
                CreateLibrary(o365Context, o365Web, o365LibraryName);
            }*/

            o365FileName = sanitiseFileName(o365FileName);
            string tmpFileName = o365FilePath + o365FileName;
            string copytofile = string.Format("/{0}{1}", o365LibraryName, tmpFileName);

            try
            {
               

                var tm = fle.TimeCreated;
                //Microsoft.SharePoint.Client.User auth = fle.Author;
                //Microsoft.SharePoint.Client.User modb = fle.ModifiedBy;
                Microsoft.SharePoint.Client.User auth = null;
                Microsoft.SharePoint.Client.User modb = null;
                //Microsoft.SharePoint.Client.User auths = fle.Author;
                //Microsoft.SharePoint.Client.User modbs = fle.ModifiedBy;

                var modf = fle.TimeLastModified;
                Boolean candometa = true;
                try
                {
                    sourcectx.ExecuteQuery();
                    //sourcectx.Load(auth);


                    ListItem item = fle.ListItemAllFields;
                    sourcectx.Load(fle);
                    sourcectx.Load(item);
                    sourcectx.ExecuteQuery();



                    FieldUserValue oAuth = item["Author"] as FieldUserValue;
                    FieldUserValue oMod = item["Editor"] as FieldUserValue;
                    auth = sourcectx.Web.GetUserById(oAuth.LookupId);
                    modb = sourcectx.Web.GetUserById(oMod.LookupId);
                    //String strAuthorName = oValueAuth.LookupValue;

                    sourcectx.Load(modb);
                    sourcectx.Load(auth);
                    sourcectx.ExecuteQuery();
                    
                    modb = desctx.Web.EnsureUser(modb.LoginName);
                    auth = desctx.Web.EnsureUser(auth.LoginName);
                    
                    desctx.Load(modb);
                    desctx.Load(auth);
                    desctx.ExecuteQuery();
                }catch(Exception ex)
                {
                    candometa = false;
                    logError(srcFilePath, copytofile, "Unable to set metadata - location 1: " + srcFilePath + " -> " + string.Format("/{0}{1}{2}", o365LibraryName, tmpFileName, ex.Message.ToString()));
                    logit("Error", srcFilePath, copytofile, "Unable to set meta data - location 1: " + srcFilePath + " -> " + string.Format("/{0}{1}{2}", o365LibraryName, tmpFileName, ex.Message.ToString()));
                    Console.WriteLine("Unable to set meta data - location 1 " + srcFilePath + " -> " + string.Format("/{0}{1}{2}", o365LibraryName, tmpFileName, ex.Message.ToString()));
                }




                if (Configuration.o365subsite == Configuration.remoteSourceSubSite)
                {
                    fle.CopyTo(copytofile, Configuration.overwrite);

                    sourcectx.ExecuteQuery();
                    if (candometa)
                    {
                        setCreateData2(desctx, copytofile, fle.TimeCreated, fle.Author, fle.TimeLastModified, fle.ModifiedBy);
                    }

                    
                }
                else
                {
                    try
                    {
                        fle.CopyTo(copytofile, Configuration.overwrite);
                        sourcectx.ExecuteQuery();
                        

                    }
                    catch(TimeoutException ex)
                    {
                        logError(srcFilePath, copytofile, "Unable to save file - too large - timeout: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                        logit("Error", srcFilePath, copytofile, "Unable to save file - too large - timeout: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                        Console.WriteLine("Unable to save file - too large - timeout: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));

                    }
                    catch(Exception ex)
                    {
                        String sourcefile = fle.ServerRelativeUrl;
                        
                        if (sourcefile.ToString().Contains("#"))
                        {
                            sourcefile = sourcefile.ToString().Replace("#", Uri.HexEscape('#'));
                        }

                        
                        //FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(sourcectx, fle.ServerRelativeUrl);
                        FileInformation fileInfo = Microsoft.SharePoint.Client.File.OpenBinaryDirect(sourcectx, sourcefile);
                        Microsoft.SharePoint.Client.File.SaveBinaryDirect(desctx, copytofile, fileInfo.Stream, true);
                        desctx.ExecuteQuery();
                        
                    }
                    if (candometa)
                    {
                        setCreateData2(desctx, copytofile, tm, auth, modf, modb);
                        desctx.ExecuteQuery();
                    }


                }
            }
            catch (TimeoutException ex)
            {
                logError(srcFilePath, copytofile, "Unable to save file - too large - timeout: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                logit("Error", srcFilePath, copytofile, "Unable to save file - too large - timeout: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                Console.WriteLine("Unable to save file - too large - timeout: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
            }
            catch (UnauthorizedAccessException UAEx)
            {
                logError(srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), "Error: " + UAEx.Message + " - Unable to save file: |" + srcFilePath + "| -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                logit("Error", srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), UAEx.Message + " - Unable to save file:" + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                logPermissionError(srcFilePath);
                Console.WriteLine("Error| " + UAEx.Message + " - Unable to save file: |" + srcFilePath + "| -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
            }
            catch (Exception ex)
            {
                logError(srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), "Error: " + ex.Message + " - Unable to save file: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                logit("Error", srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), ex.Message + " - Unable to save file: |" + srcFilePath + "| -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                Console.WriteLine("Error| " + ex.Message + " - Unable to save file: |" + srcFilePath + "| -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
            }

        }

        private static void saveFileto365(ClientContext o365Context, string o365LibraryName, string srcFilePath, string o365FilePath, string o365FileName)
        {

            Web o365Web = o365Context.Web;

            /*if (!LibraryExists(o365Context, o365Web, o365LibraryName))
            {
                CreateLibrary(o365Context, o365Web, o365LibraryName);
            }*/

            o365FileName = sanitiseFileName(o365FileName);
            string tmpFileName = o365FilePath + o365FileName;
            Console.WriteLine(tmpFileName);

            try
            {
                using (FileStream o365FileStream = new FileStream(srcFilePath, FileMode.Open))
                {
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(o365Context, string.Format("/{0}{1}", o365LibraryName, tmpFileName), o365FileStream, Configuration.overwrite);
                }
            }
            catch (System.Net.WebException wex)
            {
                var response = wex.Response as System.Net.HttpWebResponse;
                if (response != null && (response.StatusCode == (System.Net.HttpStatusCode)429 || response.StatusCode == (System.Net.HttpStatusCode)503))
                {
                    logit("Notice:", o365FileName, "", "Throttling - Notice 1001");
                    Console.WriteLine("Throttling - Notice 1001");
                }
            }
            catch (TimeoutException ex)
            {
                logError(srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), "Unable to save file - too large - timeout: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                logit("Error", srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), "Unable to save file - too large - timeout: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                Console.WriteLine("Unable to save file - too large - timeout: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
            }
            catch (UnauthorizedAccessException UAEx)
            {
                logError(srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), "Error: " + UAEx.Message + " - Unable to save file: |" + srcFilePath + "| -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                logit("Error", srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), UAEx.Message + " - Unable to save file:" + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                logPermissionError(srcFilePath);
                Console.WriteLine("Error| " + UAEx.Message + " - Unable to save file: |" + srcFilePath + "| -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
            }
            catch (Exception ex)
            {
                logError(srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), "Error: " + ex.Message + " - Unable to save file: " + srcFilePath + " -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                logit("Error", srcFilePath, string.Format("/{0}{1}", o365LibraryName, tmpFileName), ex.Message + " - Unable to save file: |" + srcFilePath + "| -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
                Console.WriteLine("Error| " + ex.Message + " - Unable to save file: |" + srcFilePath + "| -> " + string.Format("/{0}{1}", o365LibraryName, tmpFileName));
            }




        }
        private static string CreateFolder(ClientContext clientContext, string siteUrl, string listName, string relativePath, string folderName, string listID)

        {
            string tmpfld = "";
            ClientContext ctx = clientContext;
            Web web = ctx.Web;
            List list;
            try
            {
               
                if (Configuration.o365subsite.Length > 0)
                {
                    //ctx = Configuration.GetUserContext(siteUrl + "/" + Configuration.o365subsite);
                }
                else
                {

                }
                

                if (Configuration.listGUID.Length > 0)
                {
                    Guid g = new Guid(listID);
                    list = web.Lists.GetById(g); ;
                }
                else
                {
                    //string[] urlstring = spsite.Split(new string[] { "/" }, StringSplitOptions.None);
                    string lst = "";
                    if (Configuration.o365subsite.Length > 0)
                    {
                        //lst = "/" + Configuration.o365subsite + "/" + Configuration.o365List;
                        lst = Configuration.o365List;
                    }
                    else
                    {
                        lst = Configuration.o365List;
                    }
                    list = web.Lists.GetByTitle(lst);
                }



                ListItemCreationInformation newItem = new ListItemCreationInformation();
                newItem.UnderlyingObjectType = FileSystemObjectType.Folder;
                newItem.FolderUrl = siteUrl + "/" + listName;
                newItem.FolderUrl = siteUrl + "/" + Configuration.o365List;
                if (!relativePath.Equals(string.Empty))
                {
                    newItem.FolderUrl += relativePath;
                }
                folderName = sanitiseFileName(folderName);
                newItem.LeafName = folderName;
                tmpfld = newItem.FolderUrl + "/" + folderName;
                ListItem item = list.AddItem(newItem);
                item.Update();
                logit("Success", newItem.FolderUrl + "/" + newItem.LeafName, newItem.FolderUrl + "/" + newItem.LeafName, "Creating new directory:" + newItem.FolderUrl + "/" + newItem.LeafName);
                Console.WriteLine("Creating new directory: {0}", newItem.FolderUrl + "/" + newItem.LeafName);
                ctx.ExecuteQuery();
                return folderName;
            }

            catch (System.Net.WebException wex)
            {
                var response = wex.Response as System.Net.HttpWebResponse;
                if (response != null && (response.StatusCode == (System.Net.HttpStatusCode)429 || response.StatusCode == (System.Net.HttpStatusCode)503))
                {
                    logit("Notice:", relativePath + "/" + folderName, "", "Throttling - Notice 1007");
                    Console.WriteLine("Throttling - Notice 1007");

                    var retryAfter = Int32.Parse(response.Headers["Retry-After"].ToString());
                    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(retryAfter));
                    try
                    {
                        if (Configuration.listGUID.Length > 0)
                        {
                            Guid g = new Guid(listID);
                            list = web.Lists.GetById(g); ;
                        }
                        else
                        {
                            //string[] urlstring = spsite.Split(new string[] { "/" }, StringSplitOptions.None);
                            string lst = "";
                            if (Configuration.o365subsite.Length > 0)
                            {
                                //lst = "/" + Configuration.o365subsite + "/" + Configuration.o365List;
                                lst = Configuration.o365List;
                            }
                            else
                            {
                                lst = Configuration.o365List;
                            }
                            list = web.Lists.GetByTitle(lst);
                        }



                        ListItemCreationInformation newItem = new ListItemCreationInformation();
                        newItem.UnderlyingObjectType = FileSystemObjectType.Folder;
                        newItem.FolderUrl = siteUrl + "/" + listName;
                        newItem.FolderUrl = siteUrl + "/" + Configuration.o365List;
                        if (!relativePath.Equals(string.Empty))
                        {
                            newItem.FolderUrl += relativePath;
                        }
                        folderName = sanitiseFileName(folderName);
                        newItem.LeafName = folderName;
                        tmpfld = newItem.FolderUrl + "/" + folderName;
                        ListItem item = list.AddItem(newItem);
                        item.Update();
                        logit("Success", newItem.FolderUrl + "/" + newItem.LeafName, newItem.FolderUrl + "/" + newItem.LeafName, "Creating new directory:" + newItem.FolderUrl + "/" + newItem.LeafName);
                        Console.WriteLine("Creating new directory: {0}", newItem.FolderUrl + "/" + newItem.LeafName);
                        ctx.ExecuteQuery();
                        return folderName;
                    }
                    catch (Exception ex2)
                    {

                        logit("Error", "Web Exception:" + relativePath + "/" + folderName, "", ex2.Message + "");
                        logError(relativePath + "/" + folderName, "", ex2.Message + "");
                        Console.WriteLine(ex2.Message + ":" + relativePath + "/" + folderName);
                        return null;
                    }
                }
                else
                {

                    logit("Error", "Web Exception:" + relativePath + "/" + folderName, "", wex.Message + "");
                    logError("Web Exception:" + relativePath + "/" + folderName, "", wex.Message + "");
                    Console.WriteLine(wex.Message + ":" + "Web Exception:" + relativePath + "/" + folderName);
                    return null;
                }
            }


            catch (Exception ex)
            {
                logError(tmpfld, tmpfld, ex.Message + " - cannot create folder: " + tmpfld);
                logit("Error", tmpfld, tmpfld, ex.Message + " - cannot create folder: " + tmpfld);
                Console.WriteLine(ex.Message + " - cannot create folder: " + tmpfld);
                return null;
            }
        }
        private static string sanitiseFileName(string fname)
        {
            //string pattern = " *[\\~#%&*{}/:<>?|\"]+ *";

            string pattern = "&";
            string replacement = "and";

            Regex regEx = new Regex(pattern);
            fname = regEx.Replace(fname, replacement);
            fname = fname.Trim();


            pattern = "N/A";
            replacement = "NA";

            regEx = new Regex(pattern);
            fname = regEx.Replace(fname, replacement);
            fname = fname.Trim();

            pattern = ",";
            replacement = ".";

            regEx = new Regex(pattern);
            fname = regEx.Replace(fname, replacement);
            fname = fname.Trim();

            pattern = " *[\\~#%&*{}:<>?|\",/]+ *";
            replacement = " ";

            regEx = new Regex(pattern);
            string sanitized = regEx.Replace(fname, replacement);
            sanitized = sanitized.Trim();
            return sanitized;
        }
        private static bool FolderExists(ClientContext o365Context, string folderURL)
        {
            Web o365Web = o365Context.Web;
            //var exists = o365Web.GetFolderByServerRelativeUrl(Configuration.o365SiteURL + folderURL).Exists;
            var folder = o365Web.GetFolderByServerRelativeUrl("/" + folderURL);
            bool exists = false;


            try
            {
                o365Context.Load(folder);
                o365Context.ExecuteQuery();
                exists = folder.Exists;
                exists = true;
            }
            catch (Exception ex)
            { }

            if (!exists)
            {
                return false;
            }
            return true;
        }
        private static void logit(string status, string flename, string spFilename, string log)
        {

            using (StreamWriter w = System.IO.File.AppendText(Configuration.logFileName))
            {
                w.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}", Configuration.batchid, DateTime.Now.ToLongTimeString(), status, flename, spFilename, log);
            }
        }

        private static void logitUnknownAuthor(string authorname, string filename)
        {
            using (StreamWriter w = System.IO.File.AppendText(Configuration.errorUnknownAuthor))
            {
                w.WriteLine("{0}||{1}||{2}", Configuration.batchid, authorname,filename);
            }
        }
        private static void logError(string flename, string spFilename, string log)
        {

            using (StreamWriter w = System.IO.File.AppendText(Configuration.errorFileName))
            {
                //w.WriteLine("{0}: {1}", DateTime.Now.ToLongTimeString(), log);
                w.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}", Configuration.batchid, DateTime.Now.ToLongTimeString(), "Error", flename, spFilename, log);
            }
        }
        private static void logFileTooLongError(string flename, string spFilename, string log)
        {

            using (StreamWriter w = System.IO.File.AppendText(Configuration.errorFileTooLongFileName))
            {
                // w.WriteLine("{1}", DateTime.Now.ToLongTimeString(), log);
                w.WriteLine("{0}||{1}||{2}||{3}||{4}||{5}", Configuration.batchid, DateTime.Now.ToLongTimeString(), "Error", flename, spFilename, log);
            }
        }
        private static void logPermissionError(string log)
        {

            using (StreamWriter w = System.IO.File.AppendText("FilePermissionErrors.txt"))
            {
                w.WriteLine("{1}", DateTime.Now.ToLongTimeString(), log);
            }
        }
        private static void checkgetExclusionFile()
        {
            int counter = 0;
            string line;
            List<string> list = new List<string>();
            Configuration.badfileRegex = new List<String>();

            for (counter = 0; counter < Configuration.badfiles.Length; counter++)
            {
                list.Add(Configuration.badfiles[counter]);
            }

            if (Configuration.excludeFileName.Length > 0)
            {
                if (System.IO.File.Exists(Configuration.excludeFileName))
               {
                    System.IO.StreamReader file = new System.IO.StreamReader(Configuration.excludeFileName);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.IndexOf("[re]") > -1)
                        {
                            Configuration.badfileRegex.Add(line.Substring(line.IndexOf("[re]") + 4));
                        }
                       else
                       {
                            list.Add(line.ToString());
                        }

                    }
                    Configuration.badfiles = list.ToArray();
                }
            }
        }
        private static Boolean filenameMatch(string fname, string fullpath = "")
        {

            if (Configuration.badfiles.Contains(fname))
            {
                return true;
            }else if (Configuration.badfiles.Contains(fullpath)){
                return true;
            }
           else
            {
                if (Configuration.badfileRegex != null)
                {
                    foreach (string pattern in Configuration.badfileRegex)
                    {
                        try
                        {
                            Regex reg = new Regex(pattern);
                            if (reg.IsMatch(fname))
                            {
                                return true;
                            }
                            if (reg.IsMatch(fullpath))
                            {
                                return true;
                            }
                        }
                        catch (Exception ex) { }
                    }
                }


            }
            return false;
        }
        private static void checkgetExclusionFileFolders()
        {
            int counter = 0;
            string line;
            List<string> list = new List<string>();
            Configuration.badfolderRegex = new List<String>();

            if (Configuration.excludeFolderFileName.Length > 0)
            {
                if (System.IO.File.Exists(Configuration.excludeFolderFileName))
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(Configuration.excludeFolderFileName);
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.IndexOf("[re]") > -1)
                        {
                            Configuration.badfolderRegex.Add(line.Substring(line.IndexOf("[re]") + 4));
                        }
                        else
                        {
                            list.Add(line.ToString());
                        }

                    }
                    Configuration.badfolders = list.ToArray();
                }
            }
        }

        private static void getAuthorTranslateFile()
        {
            int counter = 0;
            string line;
            List<string> list = new List<string>();
            List<string> listt = new List<string>();
            //Configuration.tusernames = new List<String>();
            //Configuration.tusernamesreplace = new List<String>();

            if (Configuration.translateUsernameFile.Length > 0)
            {
                if (System.IO.File.Exists(Configuration.translateUsernameFile))
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(Configuration.translateUsernameFile);
                    while ((line = file.ReadLine()) != null)
                    {
                        string l = line.ToString();
                        if (l.Contains('|'))
                        {
                            string[] x = l.Split('|');
                            list.Add(x[0].Trim());
                            listt.Add(x[1].Trim());
                        }                       
                    }
                    Configuration.tusernames = list.ToArray();
                    Configuration.tusernamesreplace = listt.ToArray();
                }
            }
        }

        private static string tranlateUser(string uname)
        {
            string ret = uname;
            for(int x = 0; x < Configuration.tusernames.Length; x++)
            {
                if (Configuration.tusernames[x] == uname)
                {
                    return Configuration.tusernamesreplace[x];
                }
            }
            return ret;
        }
        private static Boolean foldernameMatch(string fname)
        {

            if (Configuration.badfolders.Contains(fname))
            {
                return true;
            }
            else
            {
                if (Configuration.badfolderRegex != null)
                {
                    foreach (string pattern in Configuration.badfolderRegex)
                    {
                        try
                        {
                            Regex reg = new Regex(pattern);
                            if (reg.IsMatch(fname))
                            {
                                return true;
                            }
                        }
                        catch (Exception ex) { }
                    }
                }


            }
            return false;
        }

        private static Boolean makeafolder(ClientContext ctx, String ParentFolderPath, String fld)
        {
            if (!Program.FolderExists(ctx, fld))
            {
                if (Program.CreateFolder(ctx, Configuration.o365SiteURL, Configuration.o365List, ParentFolderPath, fld, Configuration.listGUID) == null)
                {
                    
                    Program.logError(ParentFolderPath, ParentFolderPath + "/" + fld, "ERROR: 3099: unable to create folder: " + ParentFolderPath + "/" + fld + " - subsite:" + Configuration.o365subsite + ", targetList:" + Configuration.o365List);
                    Console.WriteLine("ERROR - Unable to create folder:  {0}", fld);
                    return false;
                }
                else
                {
                    Console.WriteLine("Created Folder:  {0}", fld);
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        public static void run365csv(ClientContext ctx)
        {
            if (Configuration.csvfile.Length > 0)
            {
                if (!System.IO.File.Exists(Configuration.csvfile))
                {

                    logError("Connect to CSV", "NA", "Something is wrong with your csv.  The CSV file does not exist - filename: " + Configuration.csvfile + "). ERROR 102: File Not Found");
                    logit("Error", "Connect to CSV", Configuration.csvfile, "Something is wrong with your csv.  The CSV file does not exist - filename: " + Configuration.csvfile + "). ERROR 102: File Not Found");
                    
                    Console.WriteLine("The CSV file does not exist - filename: " + Configuration.csvfile + "\nPress any key to continue");
                    Console.ReadLine();
                    //printHelp();


                }
                else
                {
                    // good to run the function
                    Console.WriteLine("Opening CSV: " + Configuration.csvfile + "");
                    Boolean goodforgo = true;
                    DataTable tbl = new DataTable();
                    try
                    {
                        tbl = getCSVoledata();
                    }catch(Exception ex)
                    {
                        goodforgo = false;
                        logError("Connect to CSV", "NA", "Something is wrong with your csv.  Cannot Open the CSV file - filename: " + Configuration.csvfile + "). ERROR 102: " + ex.Message.ToString());
                        logit("Error", "Connect to CSV", Configuration.csvfile, "Something is wrong with your csv.  Cannot Open the CSV file - filename: " + Configuration.csvfile + "). ERROR 102: " + ex.Message.ToString());
                        Console.WriteLine("Something is wrong with your csv.  Cannot Open the CSV file - filename: " + Configuration.csvfile + "). ERROR 102: " + ex.Message.ToString() + "\nPress any key to continue");
                        Console.ReadLine();
                    }
                    if (goodforgo == true)
                    {
                        long lnnum = 1;
                        foreach (DataRow rw in tbl.Rows)
                        {

                            if (Console.KeyAvailable)
                            {
                                if (Console.ReadKey(true).Key == ConsoleKey.P)
                                {
                                    Console.Write("pausing, press enter to continue");
                                    Console.ReadLine();
                                }
                            }
                            

                            Console.WriteLine("Line: " + lnnum + " - Source:" + rw[0] + "  Destination:" + rw[1]);
                            String fromfile = rw[0].ToString();
                            String toFilePath = rw[1].ToString();
                            String toFile = rw[2].ToString();
                            long lnth = 0;

                            string listd = "";
                            if (Configuration.o365subsite.Length > 0)
                            {
                                //// need to test this to see why it is set this way for subsites
                                listd = Configuration.o365subsite + "/" + Configuration.o365List;
                                ///temporarily overriding
                                //listd = Configuration.o365List;
                            }
                            else
                            {
                                listd = Configuration.o365List;
                            }



                            if (fromfile.Length > 0)
                            {


                                if (Configuration.resumed == false)
                                {
                                    if (Configuration.startfrom == fromfile)
                                    {
                                        Configuration.resumed = true;
                                    }
                                }
                                if (Configuration.resumed == true)
                                {
                                    if (!System.IO.File.Exists(fromfile))
                                    {
                                        logError("File Not Found", fromfile, "Source File Does not exist - filename: " + fromfile + "). ERROR 103: ");
                                        logit("Error", "File Not Found", fromfile, "Source File Does not exist - filename: " + fromfile + "). ERROR 103: ");
                                        Console.WriteLine(fromfile, "Source File Does not exist - filename: " + fromfile + "). ERROR 103: ");
                                    }
                                    else
                                    {
                                        
                                        lnth = 0;
                                        FileInfo f;

                                        f = new FileInfo(fromfile);
                                        try
                                        {

                                            lnth = ((f.Length / 1024));
                                        }
                                        catch (Exception ex)
                                        {
                                        }


                                        // double check that the destination file name has the correct extension.
                                        String sfext = System.IO.Path.GetExtension(fromfile);
                                        //sfext = "." + sfext;
                                        String dfext = "";
                                        if (toFile.Length > sfext.Length)
                                        {
                                            dfext = toFile.Substring(toFile.Length - (sfext.Length));
                                        }
                                        else
                                        {

                                        }
                                        if (sfext != dfext)
                                        {
                                            toFile = toFile + sfext;
                                        }
                                        //string o365LibraryName, string srcFilePath, string o365FilePath, string o365FileName

                                        

                                        if (f.LastWriteTime > Configuration.dt)
                                        {

                                            if (Configuration.overwrite == false)
                                            {
                                                String oldfilepath = listd + "/" + toFilePath;
                                                toFilePath = toFilePath + "/";
                                                string tmpFileName = string.Format("{0}{1}", toFilePath, toFile);
                                                tmpFileName = string.Format("/{0}/{1}", listd, tmpFileName);
                                                if (!TryGetFileByServerRelativeUrl(ctx.Web, tmpFileName))
                                                //if (!FileExists(list, tmpFileName))
                                                {
                                                    if (FolderExists(ctx, oldfilepath)){
                                                        
                                                        try
                                                        {
                                                            saveFileto365(ctx, listd + "/", fromfile, toFilePath, toFile);
                                                            Console.WriteLine("file {0} kb | {1} -> {2}", lnth, fromfile, toFilePath + "/" + toFile);
                                                            logit("Normal", toFile, toFilePath + "/" + toFile, lnth + " " + fromfile + " -> " + toFilePath + "/" + toFile);
                                                        }
                                                        catch (System.Net.WebException wex)
                                                        {
                                                            var response = wex.Response as System.Net.HttpWebResponse;
                                                            if (response != null && (response.StatusCode == (System.Net.HttpStatusCode)429 || response.StatusCode == (System.Net.HttpStatusCode)503))
                                                            {
                                                                logit("Notice:", toFilePath + "/" + toFile, "", "Throttling - Notice 1001");
                                                                Console.WriteLine("Throttling - Notice 1001");

                                                                var retryAfter = Int32.Parse(response.Headers["Retry-After"].ToString());
                                                                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(retryAfter));
                                                                try
                                                                {
                                                                    saveFileto365(ctx, listd + "/", fromfile, toFilePath, toFile);
                                                                }
                                                                catch (Exception ex2)
                                                                {
                                                                    
                                                                    logit("Error", "Web Exception:" + toFilePath + toFile, "", ex2.Message + "");
                                                                    logError(toFilePath + toFile, "", ex2.Message + "");
                                                                    Console.WriteLine(ex2.Message + ":" + toFilePath + toFile);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                
                                                                logit("Error", "Web Exception:" + toFilePath + toFile, "", wex.Message + "");
                                                                logError("Web Exception:" + toFilePath + toFile, "", wex.Message + "");
                                                                Console.WriteLine(wex.Message + ":" + "Web Exception:" + toFilePath + toFile);
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            
                                                            logit("Error", "Web Exception:" + toFilePath + toFile, "", ex.Message + "");
                                                            logError(toFilePath + toFile, "", ex.Message + "");
                                                            Console.WriteLine(ex.Message + ":" + "" + toFilePath + toFile);
                                                        }


                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("ERROR: Folder Does not exist:" +  toFilePath);
                                                        logError(toFile, toFilePath + "/" + toFile, "Folder Doesn't exist: " + toFilePath);
                                                        logit("Error", toFile, toFilePath + "/" + toFile, "Folder Doesn't exist: " + toFilePath );
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Skip file already exists {0} kb | {1} -> {2}", lnth, fromfile, toFilePath + "/" + toFile);
                                                    logit("Skip", fromfile, toFilePath + "/" + toFile, "File already exists: " + lnth + " " + fromfile + " -> " + toFilePath + "/" + toFile);
                                                }
                                            }
                                            else
                                            {

                                                String oldfilepath = listd + "/" + toFilePath;
                                                toFilePath = toFilePath + "/";
                                                string tmpFileName = string.Format("{0}{1}", toFilePath, toFile);
                                                tmpFileName = string.Format("/{0}/{1}", listd, tmpFileName);
                                                if (FolderExists(ctx, oldfilepath))
                                                {


                                                    try
                                                    {
                                                        saveFileto365(ctx, listd + "/", fromfile, toFilePath, toFile);
                                                        logit("Normal", toFile, toFilePath + "/" + toFile, lnth + " " + fromfile + " -> " + toFilePath + "/" + toFile);
                                                    }
                                                    catch (System.Net.WebException wex)
                                                    {
                                                        var response = wex.Response as System.Net.HttpWebResponse;
                                                        if (response != null && (response.StatusCode == (System.Net.HttpStatusCode)429 || response.StatusCode == (System.Net.HttpStatusCode)503))
                                                        {
                                                            logit("Notice:", toFilePath + "/" + toFile, "", "Throttling - Notice 1001");
                                                            Console.WriteLine("Throttling - Notice 1001");

                                                            var retryAfter = Int32.Parse(response.Headers["Retry-After"].ToString());
                                                            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(retryAfter));
                                                            try
                                                            {
                                                                saveFileto365(ctx, listd + "/", fromfile, toFilePath, toFile);
                                                            }
                                                            catch (Exception ex2)
                                                            {

                                                                logit("Error", "Web Exception:" + toFilePath + toFile, "", ex2.Message + "");
                                                                logError(toFilePath + toFile, "", ex2.Message + "");
                                                                Console.WriteLine(ex2.Message + ":" + toFilePath + toFile);
                                                            }
                                                        }
                                                        else
                                                        {

                                                            logit("Error", "Web Exception:" + toFilePath + toFile, "", wex.Message + "");
                                                            logError("Web Exception:" + toFilePath + toFile, "", wex.Message + "");
                                                            Console.WriteLine(wex.Message + ":" + "Web Exception:" + toFilePath + toFile);
                                                        }
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                        logit("Error", "Web Exception:" + toFilePath + toFile, "", ex.Message + "");
                                                        logError(toFilePath + toFile, "", ex.Message + "");
                                                        Console.WriteLine(ex.Message + ":" + "" + toFilePath + toFile);
                                                    }


                                                }
                                                else
                                                {
                                                    Console.WriteLine("ERROR: Folder Does not exist:" + toFilePath);
                                                    logError(toFile, toFilePath + "/" + toFile, "Folder Doesn't exist: " + toFilePath);
                                                    logit("Error", toFile, toFilePath + "/" + toFile, "Folder Doesn't exist: " + toFilePath);
                                                
                                                }
                                            }
                                            ctx.ExecuteQuery();
                                        }

                                    }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Empty Source file name");
                                }

                            

                        }
                    }
                }

            }
            else
            {
                Console.WriteLine("Please specify a csv file using -csvfile flag");
            }
        }
        public static System.Data.DataTable getCSVoledata()
        {
            //string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Configuration.CSVoutputcsvname + ";Extended Properties=\"Text;HDR=Yes;FORMAT=Delimited\"";

            String pathname = System.IO.Path.GetDirectoryName(Configuration.csvfile);
            String filename = System.IO.Path.GetFileName(Configuration.csvfile);

            string connString = "Provider = Microsoft.Jet.OLEDB.4.0;Data Source=" + pathname + ";Extended Properties=\"Text;HDR=Yes;FORMAT=Delimited\"";
            DataTable results = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                OleDbCommand cmd = new OleDbCommand("Select * from " + filename, conn);
                conn.Open();
                //results.Load(cmd.ExecuteReader);
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                adapter.Fill(results);
            }
            return results;
        }

        public static string getAuthorName(string filepath)
        {
            string ret = "0";
            string errmessage = "";

            try
            {
                using (var document = WordprocessingDocument.Open(filepath, false))
                {
                    return document.PackageProperties.Creator;
                }
            }catch(Exception ex)
            {
                ret = "0";
                //Console.WriteLine("Unable to get Author");
            }
            try
            {
                using (var document = SpreadsheetDocument.Open(filepath, false))
                {
                    return document.PackageProperties.Creator;
                }
            }
            catch (Exception ex)
            {
                ret = "0";
            }
            try
            {
                using (var document = DocumentFormat.OpenXml.Packaging.PresentationDocument.Open(filepath, false))
                {
                    return document.PackageProperties.Creator;
                }
            }
            catch (Exception ex)
            {
                ret = "0";
            }
            
            return ret;
        }
        /*
        // For Word documents
        public string WordDocuments(string pathToFile)
        {
            try
            {
                if (this.wordObject == null)
                {
                    this.wordObject = new Microsoft.Office.Interop.Word.Application();
                }
                
                object file = pathToFile; //this is the path
                object nullobject = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Word.Document docs = this.wordObject.Documents.Open(file, nullobject, nullobject, nullobject,
                nullobject, nullobject, nullobject, nullobject, nullobject, nullobject, nullobject, nullobject, nullobject, nullobject, nullobject, nullobject);
                //Get Author Name
                object wordProperties = docs.BuiltInDocumentProperties;
                Type typeDocBuiltInProps = wordProperties.GetType();
                Object Authorprop = typeDocBuiltInProps.InvokeMember("Item", BindingFlags.Default | BindingFlags.GetProperty, null, wordProperties, new object[] { "Author" });
                Type typeAuthorprop = Authorprop.GetType();
                string strAuthor = typeAuthorprop.InvokeMember("Value", BindingFlags.Default | BindingFlags.GetProperty, null, Authorprop, new object[] { }).ToString();
                //Console.WriteLine(strAuthor);
                docs.Close(Microsoft.Office.Interop.Word.WdSaveOptions.wdDoNotSaveChanges, nullobject, nullobject);
                return strAuthor;
            }
            catch (Exception j)
            {
                Console.WriteLine(j.Message);
                return "";
            }
        }

        //For Excel spreadsheets
        public string ExcelDocuments(string pathToFile)
        {
            try
            {
                if (this.excelObject == null)
                {
                    this.excelObject = new Microsoft.Office.Interop.Excel.Application();
        
                }
                
                string file = pathToFile; //this is the path
                object nullobject = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.Excel.Workbook sheets = excelObject.Workbooks.Open(file, nullobject, nullobject, nullobject,
                nullobject, nullobject, nullobject, nullobject, nullobject, nullobject, nullobject, nullobject,
                nullobject, nullobject, nullobject);
                //Get Author Name
                object excelProperties = sheets.BuiltinDocumentProperties;
                Type typeDocBuiltInProps = excelProperties.GetType();
                Object Authorprop = typeDocBuiltInProps.InvokeMember("Item", BindingFlags.Default | BindingFlags.GetProperty, null, excelProperties, new object[] { "Author" });
                Type typeAuthorprop = Authorprop.GetType();
                string strAuthor = typeAuthorprop.InvokeMember("Value", BindingFlags.Default | BindingFlags.GetProperty, null, Authorprop, new object[] { }).ToString();
                //Console.WriteLine(strAuthor);
                sheets.Close(XlSaveAction.xlDoNotSaveChanges, nullobject, nullobject);
                return strAuthor;
            }
            catch (Exception j)
            {
                Console.WriteLine(j.Message);
                return “”;
            }
        }

        //For powerpoint presentations
        public string PresentationDocuments(string pathToFile)
        {
            try
            {
                if (this.pptObject == null)
                {
                    this.pptObject = new Microsoft.Office.Interop.PowerPoint.Application();
                }
    }
                
                string file = pathToFile; //this is the path
                object nullobject = System.Reflection.Missing.Value;
                Microsoft.Office.Interop.PowerPoint.Presentation sheets = pptObject.Presentations.Open(file, MsoTriState.msoFalse, MsoTriState.msoFalse, MsoTriState.msoFalse);
                //Get Author Name
                object pptProperties = sheets.BuiltInDocumentProperties;
                Type typeDocBuiltInProps = pptProperties.GetType();
                Object Authorprop = typeDocBuiltInProps.InvokeMember(“Item”, BindingFlags.Default | BindingFlags.GetProperty, null, pptProperties, new object[] { “Author” });
                Type typeAuthorprop = Authorprop.GetType();
                string strAuthor = typeAuthorprop.InvokeMember(“Value”, BindingFlags.Default | BindingFlags.GetProperty, null, Authorprop, new object[] { }).ToString();
                //Console.WriteLine(strAuthor);
                sheets.Close();
                return strAuthor;
            }
            catch (Exception j)
            {
                Console.WriteLine(j.Message);
                return “”;
            }
        }
        */


        public static Boolean checkSiteExists(ClientContext ctx, string baseurl, string subsite)
        {
            Boolean ret;
            ret = false;
            var subWebs = ctx.Web.Webs;

            ctx.Load(subWebs);
            ctx.ExecuteQuery();

            foreach (var subweb in subWebs)
            {
                Console.WriteLine(subweb.Url);
                if (subweb.Url == baseurl + "/" + subsite)
                {

                    ret = true;
                }
            }
            
            return ret;
        }

        
    }

   

}


/*
 * Example 429 error handling
 * 
 * for (var i = 0; i < 1000; i++)

                {

                    var lib = ctx.Web.Lists.GetByTitle(libraryName);

                    var items = lib.GetItems(new CamlQuery());

                    ctx.Load(items);

                    try

                    {

                        ctx.ExecuteQuery();

                    }

                    catch(System.Net.WebException wex)

                    {

                        var response = wex.Response as System.Net.HttpWebResponse;

 

                        if (response != null && (response.StatusCode == (System.Net.HttpStatusCode)429 || response.StatusCode == (System.Net.HttpStatusCode)503))

                        {

                            var retryAfter = Int32.Parse(response.Headers["Retry-After"].ToString());

                            Thread.Sleep(TimeSpan.FromSeconds(retryAfter));

                            ctx.ExecuteQuery();

                            try

                            {

                                var x = items.Count; // throws CollectionNotInitializedException

                            }

                            catch(Exception ex2)

                            {

 

                            }

                        }

                    }

                    Console.WriteLine($"{i}:{items.Count}");

                }
 */
