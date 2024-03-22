using System.Security.Cryptography;
using System.Text;

namespace EukairiaWeb.Helpers
{
    public class SecurityHelper
    {

        public static string HashValue( string input)
        {
            var hashAlgorithm = SHA256.Create();
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
