~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~
   FiToS - Migrating files to Sharepoint
~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~*~

Software by Camulos Consulting
Copywrite Camulos Consulting.
Please note the License Agreement is at the bottom of this file.

How to use FiToS.
------------------------

FiToS is a console application best run from a batch file.  There are several other components which
can be used in conjunction with FiToS but each one has its own special requirements.

Create a batch file (eg migration.bat) and open within a text editor.
You will need to know the location of the software (ie where did you save it after download, where 
did you then unzip it to).

An example batch file will have the following content:

CamulosSharePointUpload.exe /source "C:\ImportantDocs\Client Documents" /user yourusername@youremail.com 
/password yourpassword /site https://yoursite.sharepoint.com /list "Shared Documents" 
-initialdir "" -listguid {averylongnumber-see help below on how to get this} -confirm "yes" -overwrite "no"

Here are the options available to you for cli and single run mode:

-user:       Required. Office 365/sharepoint Username for the sharepoint site.
-password:   Required. Office 365/sharepoint Password for the sharepoint site.
-site:       Required. site url eg https://yoursite.sharepoint.com
-list:       Required. Name of the target List in sharepoint - should be the root of the folder structure - eg Shared Documents, using
					list name sometimes doesn't work if using older Shared Document or Documents libraries, in this case using the -listguid option.
-source:     Required. location of documents on your local computer/network to be copied - ie c:\\documentsforupload

-listguid:   Optional. GUID of list
-initialdir: Optional. Initial directory ie listname / initial directory - this option enables you to target folders within a list
-confirm:    Optional. default is yes. yes/y or no/n - yes means confirms start and finish - user intervention required
-overwrite:  Optional. default is no. yes/y or no/n - yes mean you want to overwrite files already on the site, no means 
				that you do not want to overwrite any files
-dt:         Optional. format is 0000-00-00 this option is used for incremental uploads - if you specific the date, 
				it will ignore any files older and not upload them
-log:        Optional. specify the name of the log file.  eg logfile1.txt.  If left blank it will create a file in the same directory 
				as exe with the dateTime and Log.txt eg 201807011011Log.txt
-elog:       Optional. specify the name of the Errors log file.  eg Errorsfile1.txt.  If left blank it will create a file in the 
				same directory as exe with the dateTime and Error.txt eg 201807011011Error.txt
-eftlog:     Optional. specify the name of the File to long errors log file.  eg ErrorsFileNameTooLong.txt.  If left blank it will 
				create a file in the same directory as exe with the dateTime and ErrorsFileNameTooLong.txt eg 201807011011ErrorsFileNameTooLong.txt
-excfile:    Optional. specify the name of the File containing names of files to exclude.  eg exclusions.txt.  If left blank it 
				will exclude desktop.ini and thumbs.db only.  Contents of the file must be a new filename on each line, should not include 
				path only name of file.  You add filenames or valid regex if using regex, put a [re] in front of the entry 
				eg [re]\.(txt|doc|pdf) - this would exclude all files with extensions txt, doc or pdf
				if you are listing files you can use either the filename eg test.txt to exclude all text.txt
				OR you can specify the full source path eg c:\temp\text.txt
-excfilefolders: Optional. Specify the name of a file containing names of folders to exclude.  eg folderexclusions.txt.  if left blank it will not exclude any folders. 
				file should contain a folder on each new line and should be the full path.
				You can also add valid regex if using regex, put a [re] in front of the entry 
				eg [re]\(01|02|03) - this would exclude all folders with extensions 01, 02 or 03 in the folder name
				alternatively just specify the full folder path from the source eg c:\temp\subfolder1
-help:		 Displays this information
-resume		 Optional. specify a folder path to resume from.  eg -rusume "c:\documentsforUpload\folder220"
-timeout:    Optional. specify the timeout value in milliseconds for http post request to sharepoint - note a value of -1 means unlimited.  
-cleanlog:    Optional. default is no. yes/y or no/n - yes means start with clean log file by deleting previous content.  Log.txt only and does not affect the errors log or the filetolong log
-seteditdate:    Optional. default is no. yes/y or no/n - if set to yes will attempt to set the file create and edit dates - note this has overhead and will slow things down
-setauthor:    Optional. default is no. yes/y or no/n - if set to yes will attempt to set the file author - note this has overhead and will slow things down.  it will need to look for the author in the 365 system as well as update the file afterwards.
				You do not need to seteditdate if you are setting setauthor as setauthor does both actions.
-translateunamefile	Optional. if you are using -setauthor you can provide a translation file for usernames.  Author will first attempt to get
								the MS Office Author, if that comes back blank it will attempt to get the file Owner from windows.
								the format for this file is (each entry to one line please remove whitespace at start and end of names)
									fromusername|tousername
									each entry should be separated by a | character
								eg  fred.bloggs|fred bloggs
								eg  DOMAIN\fblogs|fred bloggs
								the o365 user is normally their full name.


/// alternative sources - SharePoint List - Same Tenancy
-sourcedoclist:       Optional. Name of a source document List in sharepoint - should be the root of the folder structure - eg Shared Documents
-source				  If using sourcedoclist - this flag is optional and specifies the root directory to use ie if you want to start within a folder in the source doc list.
-sourcesubsite		Optional. if the site/subsite is different to that of the destination, set this here.


using the sharepoint list as the source of "copy from" and "copy to" - these flags need to be contemplated. (NOTE this is used as a database table not the source of the documents.)
-sourcesplist:						Optional. specify a SharePoint list to use for targetsiteurl, localSourceLocation.  these must be the names of the fields.  the targetsiteurl must use the same authentication as provided to get to this list.
-splistdoclibraryfieldname:			Optional. default value is "documentLibrary". if this field does not exist or has not data in it, the -list flag above is used. this is used in conjunction with sourcesplist and defines the name of the field being used for the document library name on the target subsite.
-splistsubsitelocationfieldname:	Optional. default value is "subsiteLocation". used in conjunction with sourcesplist and defines the location of the target subsite.  Data in this field must be full site location eg https://mycompany.sharepoint.com/subsite.
-splistlocalsourcelocationfieldname: Optional. default value is "localsourceLocation". used in conjunction with sourcesplist and defines the location of the target subsite.  Data in this field must be full site location eg https://mycompany.sharepoint.com/subsite.



convert from metadata to folders mode can be enabled by using
-metadatafoldersfieldlist:				Optional. an array of field names from the source list.  Folders will be created reflecting these fields and these folders will be used instead of any existing folders eg  "Client,Project,Department".  If there is no data in the particular set of fields the document gets skipped.  Note use the title of the field eg Client Name and not the Field Name
-skiproot:                 Optional. Default is Yes,  yes/y or no/n,  IF used with metadatafoldersfieldlist, stop files being copied to the root folder.

Options available if using other modes.
-mode		Optional. Default 0 - runs in normal console mode using the above flag types.

-mode "1"	this will run using a configuration file. eg -mode "1" -configfile "c:\example\exampleconfig.spm"
-mode "configfile"	this is the same as running -mode "1" eg -mode "configfile" -configfile "c:\example\exampleconfig.spm"
-mode "config"	this is the same as running -mode "1" eg -mode "config" -configfile "c:\example\exampleconfig.spm"

The Windows Forms editors and log analyser have been removed. Modes 2/edit/editor
and 3/meta/metadata, and the -edit/-editor switches, are no longer available.
Existing configuration files can still be supplied with mode 1/config/configfile.
Use -help (or no arguments) to display terminal usage.

.NET 10 / Linux migration status
-------------------------------
The project targets net10.0 and contains no Windows Forms screens or resources.
This is the terminal-only preparation step; Linux upload support is not complete.
Remaining work includes modern SharePoint CSOM authentication and file transfer
APIs, replacing OLE DB CSV reading, and adapting Windows file ownership and paths.
Once these are complete, the intended Linux command is:
  dotnet CamulosSharePointUpload.dll -help



		



------------------ Usage ---------------

During execution of the application you can press the letter p (once only) and this will pause the process until you press enter.

An important flags -overwrite.  if you specify yes then all documents copied up with overwrite if they already exist on the site,
	this is important when you are doing incremental copies or a second copy and don't not want to overwrite the documents on the site..
	If you were doing incremental and the source was still the main document store (ie users are not yet migrated to sharepoint), you 
	would set this flag to yes to overwrite with any changes from the source.

List GUIDs
You do not have to specify a GUID.  This is an optional component, sometimes there is an issue using the name only
a GUID bypasses this problem.  Start by not listing a GUID and if you get errors, obtain the guid and try that way.
You can obtain a list GUID by using SharePoint Designer.  Examples where challenges arise are if the library was renamed.  The
original names is Documents and you renamed it to Team Documents.  GUID will be required in this instance.


********************************************************************************
*                          Example configurations                              *
********************************************************************************


-- Standard copying from local source folder to sharepoint library --
---------------------------------------------------------------------

-site "https://yoursite.sharepoint.com" -list "Shared Documents" -initialdir "Fred" -user "yourusername@yourdomain.com.au" -password "yourpassword"

This will copy the documents to "https://yoursite.sharepoint.com/Shared Documents/Fred"

-site - this is the site you will be copying files to
-list - this is the list you will be copying files to


-site "https://yoursite.sharepoint.com" -list "Shared Documents" -initialdir "Fred/documents/important" -user "yourusername@yourdomain.com.au" -password "yourpassword"
This will copy the documents to "https://yoursite.sharepoint.com/Shared Documents/Fred/documents/important"

Example subsite to subsite:
-site "https://yoursite.sharepoint.com/TeamSite/targetsubsite" -list "Team Library" -user "yourusername@yourdomain.com.au" -password "yourpassword" -sourcesubsite "TeamSite/sourcesite" -sourcedoclist "Client Documents" -source "Gnarly Surf Boards" -overwrite "yes" -confirm "yes"  -timeout -1

NOTE: -
-sourcesubsite "TeamSite/sourcesite"  this is the source sub site it will. leave blank if it is the root of the site.
-sourcedoclist "Client Documents" this is the source list.
-source "Gnarly Surf Boards" here we are specifying a particular folder to start from.  Leave blank if you are taking the whole document library.

~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
~              plist configuration                 ~
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

plist configuration is chosen to do a specific task.  This will use a list as a source of information to then copy documents to the site.
As an example, we could have a list of projects.  Each project might be stored on a different source location.

example sourceplist configuration
-site "https://yoursite.sharepoint.com" -list "Shared Documents" -initialdir "Fred/documents/important"
This will copy the documents to "https://yoursite.sharepoint.com/Shared Documents/Fred/documents/important"

Slighty different when using sharepoint list as source option
-site "https://yoursite.sharepoint.com" -list "Shared Documents" -user "yourusername@yourdomain.com.au" -password "yourpassword" -sourcesplist "examplesourcelist" -source "c:\localdocumentsourcefolder"
-site - this is the site you will be authenticating to and the location of the source list.  The authentication must also be valid for the subsites listed in your source list
-list - this is the name of the list you will be copying files to in the Sub Site. not that list you are using as the source file.
-sourcesplist - this the name of the list you will be using to define subsites and local location source.
-source - this is the local document source
within the sharepoint list you must have three columns, these must be called targetsiteurl, localSourceLocation.  
	* targetsiteurl = the full address of the subsite
	* localSourceLocation = the local location of the files to be copied up to the site.
	* Note the document library that will be used on the sub site is defined in the -list flag from above.


~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
~       convert from metadata only to folders      ~
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

These options will allow you to take an existing document library and use its metadata as folders in a new document library.
as an example, the source document list may have columns from Client and Project names.  We would specify that we want the values for these to be used for folders instead of metadata.
to make this version work you will need to add one key peice of information.  That is the -metadatafoldersfieldlist flag.  These must be valid field names on the source list.

example configuration is
-site "https://yoursite.sharepoint.com" -list "New Client Documents" -initialdir "" -sourcesplist "Client Documents" -metadatafoldersfieldlist "Client Name,Matter,DocumentType" -user "yourusername@yourdomain.com.au" -password "yourpassword"
note you can use -sourcelistguid flag if your sourcesplist's name is different to the folder.

----------------------------------
------- License Agreement --------
----------------------------------

End-User License Agreement ("Agreement")
Last updated: 30/6/2017
Version 1.0
Please read this End-User License Agreement ("Agreement") carefully before running this software ("Application").
By running or using this Application, you are agreeing to be bound by the terms and conditions of this Agreement.
If you do not agree to the terms of this Agreement, do not run the Application.

License
Camulos Consulting grants you a revocable, non-exclusive, non-transferable, limited license to download, install and use the Application solely for your purposes strictly in accordance with the terms of this Agreement.

Restrictions
You agree not to, and you will not permit others to:
license, sell, rent, lease, assign, distribute, transmit, host, outsource, disclose or otherwise commercially exploit the Application or make the Application available to any third party.


Modifications to Application
Camulos Consulting reserves the right to modify, suspend or discontinue, temporarily or permanently, the Application or any service to which it connects, with or without notice and without liability to you.

Disclaimer or Warranties
You acknowledge and agree that the Application is provided on an "As Is" basis and "As Available" basis, and that your use of or access thereby is at your sole risk and descretion. Camulos Consulting accepts no liability for any damages caused by either the proper use or the improper use of the Application.  Camulos Consulting accepts no liability for any damage, data loss perceived or otherwise caused by the Application or the use of the Application.

No assignment of Intellectual Property Rights
Nothing in this Agreement shall operate or assign or transfer any Intellectual Property Rights from the Supplier to the Customer or from the Customer to the Supplier.

Term and Termination
Term and Termination
This Agreement shall remain in effect until terminated by you or Camulos Consulting Pty Ltd. 
Camulos Consulting Pty Ltd may, in its sole discretion, at any time and for any or no reason, suspend or terminate this Agreement with or without prior notice.
This Agreement will terminate immediately, without prior notice from Camulos Consulting Pty Ltd, in the event that you fail to comply with any provision of this Agreement. You may also terminate this Agreement by deleting the Application and all copies thereof from your computer.
Upon termination of this Agreement, you shall cease all use of the Application and delete all copies of the Application from your computer.

Severability
If any provision of this Agreement is held to be unenforceable or invalid, such provision will be changed and interpreted to accomplish the objectives of such provision to the greatest extent possible under applicable law and the remaining provisions will continue in full force and effect.

Amendments to this Agreement
Camulos Consulting Pty Ltd reserves the right, at its sole discretion, to modify or replace this Agreement at any time. If a revision is material we will not be obligated to provide an notice prior to any new terms taking effect. What constitutes a material change will be determined at our sole discretion.

Contact Information
If you have any questions about this Agreement, please contact us.

