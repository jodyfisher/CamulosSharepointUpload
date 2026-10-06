using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Collections.Specialized;

/// <summary>
/// licensing version 1.0
/// date of last edit 29/6/2017
/// by Jody
/// </summary>
// don't forget to #using camulosTools
namespace camulosTools
{
    class camulosLicensing
    {
        public DateTime licenseExpiry;
        
        private const string camulosendpoint = "https://apps.camulos.com.au/licensing/getLicense/1";
        /// <summary>
        /// password version 1
        /// </summary>
        private const string password = "slkj9872lkdoinblksijr997y63jklslihsdf99873rlkjsdgjoisdg7763535272ijs.kdflsjdf";
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteurl">sharepoint site resource eg https://camulosconsulting.sharepoint.com</param>
        /// <returns>boolean - true if license is current, false if license has expired or doesn't exist, data is populated on the object for more information</returns>
        public license checkLicense(string siteurl, int appid)
        {
            license lc = new license();
            lc.LicensedEntity = siteurl;

            
            camulosencryption c = new camulosencryption();
            
            string encrypteddata = c.encrypt(siteurl + "::" + appid.ToString(), password);
           // Console.WriteLine(encrypteddata);
           // Console.WriteLine(c.decrypt(encrypteddata, password));

            WebClient wb = new WebClient();

            var data = new  NameValueCollection();
            data["d"] = encrypteddata;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var response = wb.UploadValues(camulosendpoint, "POST", data);

            string retdata = Encoding.UTF8.GetString(response);
            //Console.WriteLine(retdata);
            
            char delim = '-';
            

            retdata = c.decryptFromInt(retdata, password, delim);
            //Console.WriteLine(retdata);
            string[] returned = retdata.Split((":").ToCharArray());
            if (returned[0] == "true")
            {
                lc.currentLicense = true;
            }else
            {
                lc.currentLicense = false;
            }

            DateTime dt;
            try
            {
                if (returned[1] != "NA")
                {
                    lc.licenseExpiry = Convert.ToDateTime(returned[1]);
                    lc.licenseType = 0;
                }else
                {
                    if (lc.currentLicense)
                    {
                        lc.licenseType = 1;
                    }else
                    {
                        lc.licenseType = 0;
                    }
                }
            }catch(Exception ex) { }


            try
            {
                /// server license version
                lc.serverversion = returned[2];
                
            }
            catch (Exception ex) { }
            try
            {
                /// message from server
                lc.message = returned[3];

            }
            catch (Exception ex) { }

            return lc;
        }

       
        public class license
        {
            public Boolean currentLicense = false;
            public DateTime licenseExpiry;
            public string LicensedEntity = "";
            public string serverversion = "";
            public string message = "";
            public int licenseType = 0; /// 0=expires, 1=unlimited
            
        }
       
    }
}
