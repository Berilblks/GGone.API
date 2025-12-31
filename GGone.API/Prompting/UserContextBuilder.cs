namespace GGone.API.Prompting
{
    public class UserContextBuilder
    {
        public static string Build(string userMessage, double? bmi, string goal)
        {
            return $"[User Info: BMI: {bmi ?? 0}, Goal: {goal}]\n" +
                $"[Question]: {userMessage}";
        }
    }
}
