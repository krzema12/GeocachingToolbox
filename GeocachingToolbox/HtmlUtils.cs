using System.Text.RegularExpressions;
using GeocachingToolbox.GeocachingCom;
using System.Collections.Generic;

namespace GeocachingToolbox
{
    public class HtmlUtils
    {
        public static string[] ExtractViewstates(string page)
        {
            var fieldcount = TextUtils.Parse(page, GCConstants.PATTERN_VIEWSTATEFIELDCOUNT);
            int count;
            if (!int.TryParse(fieldcount, out count))
                count = 1;
            string[] viewstates = new string[count];

            Regex regex = new Regex(GCConstants.PATTERN_VIEWSTATES, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var viewstateMatches = regex.Matches(page);
            int index = 0;
            foreach (Match viewstateMatch in viewstateMatches)
            {
                viewstates[index++] = viewstateMatch.Groups[2].Value;
            }

            return viewstates;
        }

        public static void AppendViewstates(IDictionary<string, string> formParameters, string[] viewstates)
        {
            if (viewstates == null || viewstates.Length == 0)
                return;
            formParameters["__VIEWSTATE"] = viewstates[0];
            if (viewstates.Length > 1)
            {
                for (int i = 1; i < viewstates.Length; i++)
                {
                    formParameters["__VIEWSTATE" + i] = viewstates[i];
                }
                formParameters["__VIEWSTATEFIELDCOUNT"] = viewstates.Length.ToString();
            }
        }

        /*
           static void putViewstates(final Parameters params, final String[] viewstates) {
        if (ArrayUtils.isEmpty(viewstates)) {
            return;
        }
        params.put("__VIEWSTATE", viewstates[0]);
        if (viewstates.length > 1) {
            for (int i = 1; i < viewstates.length; i++) {
                params.put("__VIEWSTATE" + i, viewstates[i]);
            }
            params.put("__VIEWSTATEFIELDCOUNT", String.valueOf(viewstates.length));
        }
}
         */

    }
}