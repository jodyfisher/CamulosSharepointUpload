using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// crypto version 1.0
/// date of last edit 29/6/2017
/// by Jody
/// </summary>
// don't forget to #using camulosTools

namespace camulosTools
{
    class camulosencryption
    {
        public string encrypt(string data, string password)
        {
            string retString = "";
            char[] dta = data.ToCharArray();
            char[] pwd = password.ToCharArray();
            int i = 0;
            int ip = 0;
            int totp = pwd.Length;
            int thenum = 0;
            
            for (i = 0;i< dta.Length; i++){
                thenum = dta[i] + pwd[ip];
                if (thenum > 126)
                {
                    dta[i] = (char)(thenum - 126);
                }else
                {
                    dta[i] = (char)thenum;
                }
                
                if(ip == totp)
                {
                    ip = 0;
                }else
                {
                    ip += 1;
                }
            }

            retString = new string(dta);
            return retString;

        }

        public string decrypt(string data, string password)
        {

            string retString = "";
            char[] dta =  data.ToCharArray();
            char[] pwd = password.ToCharArray();
            int i = 0;
            int ip = 0;
            int totp = pwd.Length;
            int thenum = 0;

            for (i = 0; i < dta.Length; i++)
            {
                thenum = dta[i] - pwd[ip];
                if (thenum < 0)
                {
                    dta[i] = (char)(thenum + 126);
                }
                else
                {
                    dta[i] = (char)thenum;
                }

                if (ip == totp)
                {
                    ip = 0;
                }
                else
                {
                    ip += 1;
                }
            }


            retString = new string(dta);
            return retString;

        }


        public string decryptFromInt(string data, string password, char delimeter)
        {

            string retString = "";
            string[] dta1 = data.Split(delimeter);


            char[] dta = new char[dta1.Length];
            char[] pwd = password.ToCharArray();
            int i = 0;
            int ip = 0;
            int totp = pwd.Length;
            int thenum = 0;

            for (i = 0; i < dta1.Length; i++)
            {
                thenum = Convert.ToInt32(dta1[i]) - pwd[ip];
                if (thenum < 0)
                {
                    dta[i] = (char)(thenum + 126);
                }
                else
                {
                    dta[i] = (char)thenum;
                }

                if (ip == totp)
                {
                    ip = 0;
                }
                else
                {
                    ip += 1;
                }
            }


            retString = new string(dta);
            return retString;

        }


    }
}
