namespace GGone.API.Prompting
{
    public class UserContextBuilder
    {
        public static string Build(string userMessage, double? bmi, string goal)
        {
            return $"[Kullanıcı Bilgisi: BMI: {bmi ?? 0}, Hedef: {goal}]\n" +
                $"[Soru]: {userMessage}";
        }
    }
}
