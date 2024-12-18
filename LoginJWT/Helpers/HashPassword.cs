using System.Security.Cryptography;
using System.Text;


namespace LoginJWT.Helpers
{
     public static class HashPassword
     {
          public static string GenerateHash(string password)
          {
               if (string.IsNullOrEmpty(password))
               {
                    throw new ArgumentException("The password is required.");
               }
               using var sha256 = SHA256.Create();
               byte[] encryptResult = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
               var sb = new StringBuilder(encryptResult.Length * 2);

               foreach (byte b in encryptResult)
               {
                    sb.AppendFormat("{0:x2}", b);
               }
               return sb.ToString();
          }
     }
}