using System;
using System.Text.RegularExpressions;

namespace GeocachingToolbox
{
    public static class TextComparisonTools
    {
        private const int ADD_DELETE_COST = 1;
        private const int CHANGE_COST = 3;
        private const int NO_CHANGE_COST = 0;

        public static float NormalizedEditDistance(string text1, string text2)
        {
            text1 = text1.ToLower().Trim();
            text1 = Regex.Replace(text1, @"[^A-Za-z0-9 ]+", "");
            text2 = text2.ToLower().Trim();
            text2 = Regex.Replace(text2, @"[^A-Za-z0-9 ]+", "");
            return (float)LevenshteinDistance(text1, text2)*2.0f/(text1.Length + text2.Length);
        }

        public static int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
            {
                return m * ADD_DELETE_COST;
            }

            if (m == 0)
            {
                return n * ADD_DELETE_COST;
            }

            for (int i = 0; i <= n; i++)
            {
                d[i, 0] = i * ADD_DELETE_COST;
            }

            for (int j = 0; j <= m; j++)
            {
                d[0, j] = j * ADD_DELETE_COST;
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? NO_CHANGE_COST : CHANGE_COST;

                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}
