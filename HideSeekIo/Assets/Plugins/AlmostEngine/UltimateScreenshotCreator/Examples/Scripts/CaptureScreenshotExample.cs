using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AlmostEngine.Screenshot;

namespace AlmostEngine.Examples
{
	public class CaptureScreenshotExample : MonoBehaviour
	{
		public int m_Width = 800;
		public int m_Height = 600;
		public int m_Scale = 1;
		public string m_FullpathA = "";
		public string m_FullpathB = "";
		public KeyCode m_ShortcutA = KeyCode.F6;
		public KeyCode m_ShortcutB = KeyCode.F7;

		void Update ()
		{
			if (Input.GetKeyDown (m_ShortcutA)) {
				// Capture the current screen at its current resolution, including UI
				SimpleScreenshotCapture.CaptureScreenToFile (m_FullpathA);
			}
			if (Input.GetKeyDown (m_ShortcutB)) {
				// Capture the screen at a custom resolution using render to texture.
				// You must specify the list of cameras to be used in that mode.
				// Here we use Camera.main, the first scene camera tagged as "MainCamera"
				SimpleScreenshotCapture.CaptureCameraToFile (m_FullpathB, m_Width, m_Height, Camera.main, TextureExporter.ImageFileFormat.JPG, 70, 8);
			}
		}
	}
}

