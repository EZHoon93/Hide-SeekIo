using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace AlmostEngine.Screenshot.Extra
{
    public class ScreenshotResize
    {
        public static void ResizeScreenshot(ScreenshotResolution res, int width, int height, bool preserveOriginalRatio)
        {
            if (res == null || res.m_Texture == null)
            {
                Debug.LogError("Can not resize, null texture.");
                return;
            }
            if (preserveOriginalRatio)
            {
                float ratio = (float)res.m_Width / (float)res.m_Height;
                if (width > 0)
                {
                    height = (int)(width / ratio);
                }
                else
                {
                    width = (int)(height * ratio);
                }
            }
            var resized = ResizeTexture(res.m_Texture, width, height);

            // Replace the texture
            GameObject.DestroyImmediate(res.m_Texture);
            res.m_Texture = resized;
        }

        public static Texture2D ResizeTexture(Texture2D texture2D, int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 24);
            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);
            Texture2D result = new Texture2D(width, height);
            result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            result.Apply();
            return result;
        }
    }
}
