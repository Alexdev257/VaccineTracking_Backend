using System.Text;

namespace ClassLib.Helpers
{
    public class VerifyCodeHelper
    {
        public static string GenerateSixRandomCode()
        {
            Random random = new Random();
            string characters = "0123456789";
            StringBuilder result = new StringBuilder(6);
            for (int i = 0; i < 6; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }
    }
}
