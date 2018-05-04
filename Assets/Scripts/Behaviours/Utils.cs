using UnityEngine;

namespace SmallTricks
{
    public class Utils
    {
        public static Color TRANSPARENT = new Color(0, 0, 0, 0);

        public static Color ConvertGreyscale(Color color)
        {
            float grey = color.grayscale * (Configs.AUTO_GREY_SCALE_RANGE_E - Configs.AUTO_GREY_SCALE_RANGE_S) + Configs.AUTO_GREY_SCALE_RANGE_S;
            return new Color(grey, grey, grey, color.a);
        }

        public static int GetResponseCode(WWW request)
        {
            int ret = 0;
            if (request.responseHeaders == null)
            {
                Debug.LogWarning("no response headers.");
            }
            else
            {
                if (!request.responseHeaders.ContainsKey("STATUS"))
                {
                    Debug.LogWarning("response headers has no STATUS.");
                }
                else
                {
                    ret = ParseResponseCode(request.responseHeaders["STATUS"]);
                }
            }
            return ret;
        }

        private static int ParseResponseCode(string statusLine)
        {
            int ret = 0;
            string[] components = statusLine.Split(' ');
            if (components.Length < 3)
            {
                Debug.LogWarning("invalid response status: " + statusLine);
            }
            else
            {
                if (!int.TryParse(components[1], out ret))
                {
                    Debug.LogWarning("invalid response code: " + components[1]);
                }
            }
            return ret;
        }
    }
}
