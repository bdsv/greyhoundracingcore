using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Net;

namespace GreyhoundBE.BO
{
    public class Tools
    {
        private const bool IsInDebugMode = true;

        #region Private Member Variables

        private static string DEBUG_FILE_NAME = System.Environment.GetEnvironmentVariable("Temp") + "\\greyhoundBE.log";
        private const String DATETIME_FORMAT = "dd-MM-yyyy HH:mm:ss";
        //The code of Microsoft English language
        const int EN_CODE = 1033;
        #endregion

        #region Private Properties
        public const String ONE_HOUR = "1h";
        public const String ONE_DAY = "1d";
        public const String ONE_WEEK = "1w";
        public const String ONE_MONTH = "1m";
        public const String SIX_MONTHS = "6m";
        public const String ONE_YEAR = "1y";
        public const String EVER = "e";
        #endregion

        #region Private Methods

        #endregion

        #region Constructors

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        private static byte[] m_btaTripleDESInitVector = { 50, 51, 52, 53, 54, 55, 56, 57 };
        // 156834191816975439751813176864185748956835101549

        private static byte[] m_btaTripleDESKey = { 15, 68, 34, 19, 18, 16, 97, 54, 39, 75, 18, 13, 17, 68, 64, 18, 57, 48, 95, 68, 35, 10, 15, 49 };
        //private static byte[] m_btaTripleDESKey = { 1, 6, 3, 1, 1, 1, 9, 5, 3, 7, 1, 1, 1, 6, 6, 1, 5, 4, 9, 6, 3, 1, 1, 4 };

        private const bool m_bDEBUG = true;
        private static string m_strDebugFileName = System.Environment.GetEnvironmentVariable("Temp") + "\\greyhoundBE.log";

        public static void printDbg(string str_class_name, string str_method_name, string str_debug_message)
        {
            if (m_bDEBUG)
            {
                File.AppendAllText(Tools.m_strDebugFileName, "[" + System.DateTime.Now + "] " +
                                        str_class_name + ":" +
                                        str_method_name + ": " +
                                        str_debug_message + "\r\n");
            }
        }

        public static string DecryptData(string str_data)
        {
            try
            {
                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                MemoryStream cMemStream = new MemoryStream(Convert.FromBase64String(str_data));

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cCryptoStreamDecrypt = new CryptoStream(cMemStream,
                    new TripleDESCryptoServiceProvider().CreateDecryptor(m_btaTripleDESKey, m_btaTripleDESInitVector),
                    CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] btDecryptData = new byte[str_data.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                cCryptoStreamDecrypt.Read(btDecryptData, 0, btDecryptData.Length);

                //Convert the buffer into a string and return it.
                return new ASCIIEncoding().GetString(btDecryptData).TrimEnd('\0');
            }
            catch (Exception cExc)
            {
                Tools.printDbg(new System.Diagnostics.StackFrame().GetMethod().DeclaringType.Name, new System.Diagnostics.StackFrame().GetMethod().Name, cExc.Message);
                return "";
            }
        }

        /**
         * @brief Decrypt data from memory with 3DES algorithm
         * @param[in] bt_data encrypted data
         * @param[in] bt_key symmetric key
         * @param[in] bt_init_vector initialization vector for the algorithm
         * @return string with the decrypted data
         */
        public static string DecryptDataFromMemory(byte[] bt_data, byte[] bt_key, byte[] bt_init_vector)
        {
            try
            {
                // Create a new MemoryStream using the passed array of encrypted data.
                MemoryStream cMemStream = new MemoryStream(bt_data);
                TripleDESCryptoServiceProvider cTripleDES = new TripleDESCryptoServiceProvider();
                cTripleDES.Padding = PaddingMode.Zeros;

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cCryptoStreamDecrypt = new CryptoStream(cMemStream,
                                                cTripleDES.CreateDecryptor(bt_key, bt_init_vector),
                                                CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] btDecryptData = new byte[bt_data.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                cCryptoStreamDecrypt.Read(btDecryptData, 0, btDecryptData.Length);

                // Close the streams
                cCryptoStreamDecrypt.Close();
                cMemStream.Close();

                //Convert the buffer into a string and return it.
                return new ASCIIEncoding().GetString(btDecryptData);
            }
            catch (CryptographicException cException)
            {
                Tools.printDbg(new System.Diagnostics.StackFrame().GetMethod().DeclaringType.Name, new System.Diagnostics.StackFrame().GetMethod().Name, cException.Message);
                return null;
            }
        }

        public static string EncryptData(string str_data)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream cMemStream = new MemoryStream();

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cCryptoStreamEncrypt = new CryptoStream(cMemStream,
                    new TripleDESCryptoServiceProvider().CreateEncryptor(m_btaTripleDESKey, m_btaTripleDESInitVector),
                    CryptoStreamMode.Write);

                // Convert the passed string to a byte array.
                byte[] btToEncrypt = new ASCIIEncoding().GetBytes(str_data);

                // Write the byte array to the crypto stream and flush it.
                cCryptoStreamEncrypt.Write(btToEncrypt, 0, btToEncrypt.Length);
                cCryptoStreamEncrypt.FlushFinalBlock();

                // Get an array of bytes from the 
                // MemoryStream that holds the 
                // encrypted data.
                byte[] btEncryptedData = cMemStream.ToArray();

                // Close the streams.
                cCryptoStreamEncrypt.Close();
                cMemStream.Close();

                // Return the encrypted buffer.
                return Convert.ToBase64String(btEncryptedData);
            }
            catch (Exception cExc)
            {
                Tools.printDbg(new System.Diagnostics.StackFrame().GetMethod().DeclaringType.Name, new System.Diagnostics.StackFrame().GetMethod().Name, cExc.Message);
                return null;
            }
        }

        /**
         * @brief Encrypt data from memory with 3DES algorithm
         * @param[in] str_data encrypted data
         * @param[in] bt_key symmetric key
         * @param[in] bt_init_vector initialization vector for the algorithm
         * @return Byte array with the encrypted data
         */
        public static byte[] EncryptDataToMemory(string str_data, byte[] bt_key, byte[] bt_init_vector)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream cMemStream = new MemoryStream();
                TripleDESCryptoServiceProvider cTripleDES = new TripleDESCryptoServiceProvider();
                cTripleDES.Padding = PaddingMode.Zeros;

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cCryptoStreamEncrypt = new CryptoStream(cMemStream,
                                    cTripleDES.CreateEncryptor(bt_key, bt_init_vector),
                                    CryptoStreamMode.Write);

                // Convert the passed string to a byte array.
                byte[] btToEncrypt = new ASCIIEncoding().GetBytes(str_data);

                // Write the byte array to the crypto stream and flush it.
                cCryptoStreamEncrypt.Write(btToEncrypt, 0, btToEncrypt.Length);
                cCryptoStreamEncrypt.FlushFinalBlock();

                // Get an array of bytes from the MemoryStream that holds the 
                // encrypted data.
                byte[] btEncryptedData = cMemStream.ToArray();

                // Close the streams.
                cCryptoStreamEncrypt.Close();
                cMemStream.Close();

                // Return the encrypted buffer.
                return btEncryptedData;
            }
            catch (CryptographicException cExc)
            {
                Tools.printDbg(new System.Diagnostics.StackFrame().GetMethod().DeclaringType.Name, new System.Diagnostics.StackFrame().GetMethod().Name, cExc.Message);
                return null;
            }
        }

        /**
         * @brief Convert hexadecimal string to ascii
         * @param[in] str_hex a string in hexadecimal
         * @return string with the conversion from hex values to characters
         */
        public static string FromHexString(string str_hex)
        {
            // The final string shall have half the length of the hexadecimal string
            StringBuilder strData = new StringBuilder((str_hex.Length / 2) + 1);

            //first take two hex value using substring.
            //then convert Hex value into ASCII.
            //then convert ASCII value into character.
            for (int nI = 0; nI < str_hex.Length; nI += 2)
            {
                strData.Append(System.Convert.ToChar(System.Convert.ToUInt32(str_hex.Substring(nI, 2), 16)).ToString());
            }

            return strData.ToString();
        }

        /**
         * @brief Convert an ascii string to an hexadecimal string
         * @param[in] str_chars string in hexadecimal
         * @return string with the conversion from ascii characters to hex values
         */
        public static string ToHexString(string str_chars)
        {
            UTF8Encoding cEncoder = new UTF8Encoding();
            byte[] btByteArray = cEncoder.GetBytes(str_chars);
            // The ASCII string lenght is the double of the hex string
            StringBuilder strHexData = new StringBuilder((str_chars.Length * 2) + 1);

            foreach (byte btByte in btByteArray)
            {
                strHexData.Append(btByte.ToString("X2"));
            }

            return strHexData.ToString();
        }

        public static bool sendMail(String user_email, String reference)
        {
            try
            {
                var fromAddress = new MailAddress("greyhounds.racing@gmail.com", "Greyhound Racing");
                const string fromPassword = "internet2010";

                const string subject = "Greyhound Racing Registration";
                string body = "Please click on the link bellow to confirm your registration.\r\n"
                            + "\r\nhttp://duvallnetwork.no-ip.org/Greyhound/ConfirmRegistration.aspx?user_id="
                            + reference + "&email=" + user_email + "\r\n";

                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword),
                    Timeout = 10000
                };
                using (var message = new MailMessage(fromAddress.Address, user_email)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                return true;
            }
            catch (Exception ex)
            {
                Tools.printDbg(
                    new System.Diagnostics.StackFrame().GetMethod().DeclaringType.Name,
                    new System.Diagnostics.StackFrame().GetMethod().Name,
                    ex.StackTrace);
            }
            return false;
        }

        /**
         * @brief Prints debug information to a file, if debug mode is active.
         * @param StackFrame stackFrame The current stackframe. (E.g. new System.Diagnostics.StackFrame())
         * @param string debugMessage The debug message
         * @param Exception e
         **/
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void printDebug(StackFrame stackFrame, string debugMessage, Exception e)
        {
            if (IsInDebugMode)
            {
                if (stackFrame == null)
                    stackFrame = new System.Diagnostics.StackFrame();

                string exceptionInformation = "";
                if (e != null)
                {
                    exceptionInformation = "\r\n" +
                                           e.ToString() +
                                           "\r\n" +
                                           e.Message +
                                           "\r\n" +
                                           e.StackTrace;
                }
                File.AppendAllText(DEBUG_FILE_NAME, "\r\n[" + System.DateTime.Now + "] " +
                                        stackFrame.GetMethod().DeclaringType.Name + ":" +
                                        stackFrame.GetMethod().Name + ": " +
                                        stackFrame.GetFileLineNumber() + ": " +
                                        debugMessage + exceptionInformation);
            }
        }

        /**
         * @brief Prints debug information to a file, if debug mode is active.
         * @param StackFrame stackFrame The current stackframe. (E.g. new System.Diagnostics.StackFrame())
         * @param string debugMessage The debug message
         * @param Exception e
         **/
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void printDebug(string debugMessage)
        {
            Tools.printDebug(new System.Diagnostics.StackFrame(), debugMessage, null);
        }

        /**
         * @brief Compares startRec and endRec and swap them if they need to be swaped
         * @param ref int startRec The start index of the records
         * @param ref int endRec The end index of the records
         **/
        public static String normalizeResult(String result)
        {
            result = result.Replace("},]}", "}]}");
            result = result.Replace("},}", "}}");
            result = result.Replace("},]", "}]");
            result = result.Replace("],]", "]]");
            result = result.Replace("],}", "]}");
            result = result.Replace(",,", ",");

            return result;
        }

        /**
         * @brief Compares startRec and endRec and swap them if they need to be swaped
         * @param ref int startRec The start index of the records
         * @param ref int endRec The end index of the records
         **/
        public static void normalizeRanges(ref int startRec, ref int endRec)
        {
            // RANGES VALIDATION
            if (startRec < 0) startRec = 1;
            if (endRec < 0) endRec = 100;
            if (startRec > endRec) { int tmp = startRec; startRec = endRec; endRec = tmp; }
            if (endRec > startRec + 99) endRec = startRec + 99;
        }

        /**
         * @brief Converts a string to Decimal.
         * @param string input The input string
         * @return Decimal conversion
         **/
        public static Decimal StringToDecimal(string input)
        {
            if (input.Equals(""))
                return new Decimal(0);

            input = input.Replace(',', '.');
            int factor = 1;
            if (input.StartsWith("-"))
            {
                input = input.Substring(1);
                factor = -1;
            }
            decimal value = Decimal.Parse(input, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

            return Convert.ToDecimal(value * factor);
        }

        /**
         * @brief Converts a string to boolean.
         * @param string input The input string
         * @return bool False if string is "0" or true otherwise
         **/
        public static bool StringToBool(string input)
        {
            return ((input.CompareTo("1") == 0) || (String.Compare(input, "true", true) == 0));
        }

        /**
         * @brief Converts a string (true or false) to a string (0 or 1)
         * @param string input The input string
         * @return bool "1" if string is "true", "0" if string is false
         **/
        public static String StringBoolToStringInt(string input)
        {
            String strRet = "0";
            if (input.CompareTo("true") == 0)
            {
                strRet = "1";
            }
            return strRet;
        }
        /**
         * @brief Converts a boolean to a 0/1 String.
         * @param bool input The input boolean
         * @return string "1" if bool is true or "0" otherwise
         **/
        public static string BoolToString(bool input)
        {
            string res = "0";
            if (input)
            {
                res = "1";
            }
            return res;
        }

        /**
         * @brief Converts a boolean into an int
         * @param bool input The input boolean value
         * @returns 1 if input is true, 0 otherwise
         **/
        public static int BoolToInt(bool input)
        {
            int res = 0;
            if (input)
            {
                res = 1;
            }
            return res;
        }

        /**
         * @brief Converts an int to a boolean
         * @param int input The input int
         * @returns bool True if input is 1 or False otherwise
         **/
        public static bool IntToBool(int input)
        {
            return (input == 1);
        }

        /**
         * @brief Converts a DateTime object to a string
         * @param DateTime date The date to convert
         * @param string format The format of the date
         * @returns String The converted date
         **/
        public static string DateTimeToString(DateTime date, string format)
        {
            return date.ToString(format);
        }

        /**
         * @brief Converts a string to DateTime object
         * @param string date The date to convert
         * @param string format The format of the date
         * @returns DateTime The converted DateTime object
         **/
        public static DateTime StringToDateTime(string date, string format)
        {
            DateTime res = new DateTime();
            int year = -1, month = -1, day = -1, hours = -1, minutes = -1, seconds = -1;

            try
            {
                if (date.EndsWith("\n"))
                {
                    date = date.Substring(0, date.Length - 1);
                }
                CultureInfo provider = CultureInfo.InvariantCulture;
                res = DateTime.ParseExact(date, format, provider);
            }
            catch (Exception)
            {
                try
                {
                    string[] strDD;
                    string[] strToken;

                    strDD = date.Split(' ');
                    strToken = strDD[0].Split('-');

                    year = Convert.ToInt32(strToken[2]);
                    month = Convert.ToInt32(strToken[1]);
                    day = Convert.ToInt32(strToken[0]);

                    strToken = strDD[1].Split(':');
                    hours = Convert.ToInt32(strToken[0]);
                    minutes = Convert.ToInt32(strToken[1]);
                    seconds = Convert.ToInt32(strToken[2]);

                    res = new DateTime(year, month, day, hours, minutes, seconds);


                }
                catch (Exception)
                {
                    try
                    {
                        string[] strDD;
                        string[] strToken;

                        strDD = date.Split(' ');
                        strToken = strDD[0].Split('/');
                        year = Convert.ToInt32(strToken[2]);
                        month = Convert.ToInt32(strToken[1]);
                        day = Convert.ToInt32(strToken[0]);

                        strToken = strDD[1].Split(':');
                        hours = Convert.ToInt32(strToken[0]);
                        minutes = Convert.ToInt32(strToken[1]);
                        seconds = Convert.ToInt32(strToken[2]);

                        res = new DateTime(year, month, day, hours, minutes, seconds);
                    }
                    catch (Exception)
                    {
                        //Tools.printDebug(new System.Diagnostics.StackFrame(), "THIRD FORMAT (" + date + "|" + format + ") Will try fourth format", exception_third_format);
                        try
                        {
                            //Fri Oct 09 16:15:49 2009
                            format = "ddd MMM dd HH:mm:ss yyyy";
                            CultureInfo provider = CultureInfo.InvariantCulture;
                            res = DateTime.ParseExact(date, format, provider);
                        }
                        catch (System.Exception e)
                        {
                            Tools.printDebug(new System.Diagnostics.StackFrame(), "FORTH FORMAT (" + date + "|" + format + ") Aborting", e);
                        }
                    }
                }
            }
            return res;
        }

        public static void AddZerosToDate(ref String str_month,
                                        ref String str_day,
                                        ref String str_hour,
                                        ref String str_min,
                                        ref String str_sec)
        {
            if (Convert.ToInt32(str_month) < 10)
            {
                str_month = "0" + str_month;
            }

            if (Convert.ToInt32(str_day) < 10)
            {
                str_day = "0" + str_day;
            }

            if (Convert.ToInt32(str_hour) < 10)
            {
                str_hour = "0" + str_hour;
            }

            if (Convert.ToInt32(str_min) < 10)
            {
                str_min = "0" + str_min;
            }

            if (Convert.ToInt32(str_sec) < 10)
            {
                str_sec = "0" + str_sec;
            }
        }

        public static bool CheckDateRangeWithSelectedDate(String str_sd, DateTime item_date)
        {
            DateTime currentDate = DateTime.Now;
            bool bInChoosenServerdate = false;

            if (str_sd.CompareTo(ONE_HOUR) == 0)
            {
                currentDate = currentDate.AddHours(-1);
            }
            else if (str_sd.CompareTo(ONE_DAY) == 0)
            {
                currentDate = currentDate.AddDays(-1);
            }
            else if (str_sd.CompareTo(ONE_WEEK) == 0)
            {
                currentDate = currentDate.AddDays(-7);
            }
            else if (str_sd.CompareTo(ONE_MONTH) == 0)
            {
                currentDate = currentDate.AddMonths(-1);
            }
            else if (str_sd.CompareTo(SIX_MONTHS) == 0)
            {
                currentDate = currentDate.AddMonths(-6);
            }
            else if (str_sd.CompareTo(ONE_YEAR) == 0)
            {
                currentDate = currentDate.AddYears(-1);
            }
            else if ((str_sd.CompareTo(EVER) == 0)
                || (str_sd.CompareTo("") == 0))
            {
                currentDate = new DateTime();
            }

            bInChoosenServerdate = item_date >= currentDate;
            return bInChoosenServerdate;

        }
        #endregion
    }
}
