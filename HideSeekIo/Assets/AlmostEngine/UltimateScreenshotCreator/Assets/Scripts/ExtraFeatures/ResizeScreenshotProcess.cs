using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace AlmostEngine.Screenshot.Extra
{
    public class ResizeScreenshotProcess : ScreenshotProcess
    {
        public int m_Width = 800;
        public int m_Height = 600;
        public bool m_PreserveOriginalRatio = true;

        public override void Process(ScreenshotResolution res)
        {
            ScreenshotResize.ResizeScreenshot(res, m_Width, m_Height, m_PreserveOriginalRatio);
        }
    }
}

