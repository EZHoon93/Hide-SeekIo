using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlmostEngine.Screenshot
{
	/// <summary>
	/// This component is automatically added to the camera to wait for the end of the render pass and copy the framebuffer content.
	/// It is used only on multi-display settings.
	/// </summary>
	[RequireComponent(typeof(Camera))]
	public class MultiDisplayCameraCapture : MonoBehaviour
	{

		Texture2D m_TargetTexture;
		bool m_DoCopy = false;


		public void CaptureCamera (Texture2D targetTexture)
		{
			m_TargetTexture = targetTexture;
			m_DoCopy = true;
		}

		public bool CopyIsOver ()
		{
			return !m_DoCopy;
		}

		void OnRenderImage (RenderTexture src, RenderTexture dest)
		{
			Graphics.Blit (src, dest);

            if (m_DoCopy && m_TargetTexture != null)
            {
				m_TargetTexture.ReadPixels (new Rect (0, 0, m_TargetTexture.width, m_TargetTexture.height), 0, 0);
				m_TargetTexture.Apply (false);

				m_DoCopy = false;
			}
		
		}

	}
}
