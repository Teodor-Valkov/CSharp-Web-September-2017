namespace _01.SchoolCompetition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            Dictionary<string, int> scores = new Dictionary<string, int>();
            Dictionary<string, SortedSet<string>> categories = new Dictionary<string, SortedSet<string>>();

            string input = string.Empty;
            while ((input = Console.ReadLine()) != "END")
            {
                string[] inputArgs = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string name = inputArgs[0];
                string category = inputArgs[1];
                int score = int.Parse(inputArgs[2]);

                if (!categories.ContainsKey(name))
                {
                    categories[name] = new SortedSet<string>();
                }

                if (!scores.ContainsKey(name))
                {
                    scores[name] = 0;
                }

                categories[name].Add(category);
                scores[name] += score;
            }

            Dictionary<string, SortedSet<string>> orderedCategories = categories
                .OrderBy(category => category.Key)
                .ToDictionary(category => category.Key, category => category.Value);

            Dictionary<string, int> orderedStudents = scores
                .OrderByDescending(student => student.Value)
                .ThenBy(student => student.Key)
                .ToDictionary(student => student.Key, student => student.Value);

            foreach (KeyValuePair<string, int> student in orderedStudents)
            {
                Console.WriteLine($"{student.Key}: {student.Value} [{string.Join(", ", orderedCategories[student.Key])}]");
            }
        }
    }
}