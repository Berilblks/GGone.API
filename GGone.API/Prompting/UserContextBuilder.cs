namespace GGone.API.Prompting
{
    public class UserContextBuilder
    {
        public static string Build(string userMessage, double? bmi, double weight, double height, int age, string gender, string? currentDietPlan)
        {
            var context = $"[User Info -> BMI: {bmi ?? 0:F1}, Weight: {weight}kg, Height: {height}cm, Age: {age}, Gender: {gender}]\n" +
                $"[System Note]: You ALREADY know the user's weight, height, age and gender from the info above. DO NOT ask for them again.\n";

            if (!string.IsNullOrEmpty(currentDietPlan))
            {
                context += $"[CURRENT DIET PLAN]: {currentDietPlan}\n" +
                           $"[System Note]: If the user asks to UPDATE or CHANGE their diet, use the plan above as the reference. " +
                           $"Apply changes ONLY to the requested days/meals and keep the rest same if needed, or regenerate if requested.\n";
            }

            context += $"[Question]: {userMessage}";
            return context;
        }
    }
}
