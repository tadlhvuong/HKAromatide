using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Security.Claims;

namespace HKShared.Helpers
{
    public static class Common
    {
        static Random rdm = new Random();
        static DateTime utcStart = new DateTime(1970, 1, 1);

        public static readonly char[] LineDelimiters = new char[] { '\r', '\n' };

        public static float RandomF()
        {
            return (float)(2 * rdm.NextDouble() - 1);
        }

        public static int Random()
        {
            return rdm.Next(0, int.MaxValue);
        }

        public static int Random(int maxValue)
        {
            return rdm.Next(0, maxValue);
        }

        public static int Random(int minValue, int maxValue)
        {
            return rdm.Next(minValue, maxValue);
        }

        public static string Random_Mix(int _size)
        {
            byte[] res = new byte[_size];
            byte achar = 0;
            for (int i = 0; i < _size; i++)
            {
                do
                {
                    achar = (byte)(rdm.Next(64) + 48);
                } while (((achar < 50) || (achar > 57)) && ((achar < 65) || (achar > 90) || (achar == 73) || (achar == 79)));
                res[i] = achar;
            }

            return Encoding.ASCII.GetString(res);
        }

        public static string Random_Num(int _size)
        {
            byte[] res = new byte[_size];
            byte achar = 0;
            for (int i = 0; i < _size; i++)
            {
                do
                {
                    achar = (byte)(rdm.Next(64) + 48);
                } while ((achar < 48) || (achar > 57));
                res[i] = achar;
            }

            return Encoding.ASCII.GetString(res);
        }

        public static string Random_Guid()
        {
            Guid result = Guid.NewGuid();
            return result.ToString();
        }
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException(nameof(principal));

            return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        public static string GET_IP()
        {
            String hostName = string.Empty;
            hostName = Dns.GetHostName();
            IPHostEntry myIP = Dns.GetHostEntry(hostName);
            IPAddress[] address = myIP.AddressList;
            if (address == null) return null;
            return address[address.Length - 1].ToString();
        }

        public static int IP_String2Int(string ipString)
        {
            IPAddress ipAddress = IPAddress.Loopback;
            IPAddress.TryParse(ipString, out ipAddress);

            byte[] bytes = ipAddress.GetAddressBytes();
            if (bytes.Length != 4)
                return 0;

            return BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0);
        }

        public static string IP_Int2String(int ipInteger)
        {
            try
            {
                byte[] ipbytes = BitConverter.GetBytes(ipInteger);
                IPAddress ipAddress = new IPAddress(ipbytes);
                return ipAddress.ToString();
            }
            catch
            {
                return "0.0.0.0";
            }
        }

        public static long GetUnixUtcTime()
        {
            return (long)((DateTime.UtcNow - utcStart).TotalSeconds);
        }

        public static long GetUnixLocalTime()
        {
            return (long)((DateTime.Now - utcStart).TotalSeconds);
        }

        public static string HashMD5(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] output = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < output.Length; i++)
                    sBuilder.Append(output[i].ToString("x2"));
                return sBuilder.ToString();
            }
        }

        public static string HashHMAC_SHA1(string input, string key, bool raw = false)
        {
            byte[] datBytes = Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = Encoding.Default.GetBytes(key);

            using (HMACSHA1 hMac = new HMACSHA1(keyBytes))
            {
                byte[] output = hMac.ComputeHash(datBytes);
                if (raw)
                    return Encoding.Default.GetString(output);
                else
                {
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < output.Length; i++)
                        sBuilder.Append(output[i].ToString("x2"));

                    return sBuilder.ToString();
                }
            }
        }

        public static string HashHMAC_SHA256(string input, string key, bool raw = false)
        {
            byte[] datBytes = Encoding.UTF8.GetBytes(input);
            byte[] keyBytes = Encoding.Default.GetBytes(key);

            using (HMACSHA256 hMac = new HMACSHA256(keyBytes))
            {
                byte[] output = hMac.ComputeHash(datBytes);
                if (raw)
                    return Encoding.Default.GetString(output);
                else
                {
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < output.Length; i++)
                        sBuilder.Append(output[i].ToString("x2"));

                    return sBuilder.ToString();
                }
            }
        }

        public static string NormalizeVietnamese(string orgText)
        {
            string newText = orgText.Normalize(NormalizationForm.FormD);

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            newText = regex.Replace(newText, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').Replace('\u0020', '-');
            newText = Regex.Replace(newText, "[^0-9a-zA-Z_-]+", "");
            return newText;
        }

        public static string SubString(string source, int maxChars = 30)
        {
            if (string.IsNullOrEmpty(source))
                return "";

            if (source.Length <= maxChars)
                return source;

            return source.Substring(0, maxChars) + "...";
        }

        public static string GetEnumText<EnumType>(EnumType enumValue)
        {
            Type enumType = typeof(EnumType);
            if (!enumType.IsEnum)
                return enumValue.ToString();

            var memInfo = enumType.GetMember(enumValue.ToString());
            if (memInfo == null || memInfo.Length < 1)
                return enumValue.ToString();

            var attributes = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes == null || attributes.Length < 1)
                return enumValue.ToString();

            return ((DisplayAttribute)attributes[0]).Name;
        }

        public static string ConvertEmailToName(string email)
        {
            var index = email.IndexOf('@');
            var usernmae = email.Substring(0, index);
            return email.Substring(0, index);
        }

        //public static Geography LatLonToDbGeography(double latitude, double longitude)
        //{
        //    var point = string.Format("POINT({1} {0})", latitude, longitude);
        //    return DbGeography.FromText(point);
        //}
    }
}
