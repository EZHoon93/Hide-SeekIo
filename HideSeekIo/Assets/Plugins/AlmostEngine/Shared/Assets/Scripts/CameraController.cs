using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlmostEngine.Examples
{
	public class CameraController : MonoBehaviour
	{
		public bool m_MouseLookOnClickOnly = true;
		public float m_RotationCoeff = 200.0f;
		public float m_TranslationCoeff = 8.0f;
		public float m_TranslationMouseCoeff = 2.0f;
		public float m_TranslationMouseScrollCoeff = 25.0f;
		Transform m_Character;
		public Transform m_Head;
		Vector3 m_Mouse;

		void Start ()
		{
			m_Character = transform;
			if (m_Head == null) {
				m_Head = GetComponentInChildren<Camera> ().transform;
			}
			m_Mouse = Input.mousePosition;
		}

		void Update ()
		{
			// Keyboard
			float forward = Input.GetAxis ("Vertical") * Time.deltaTime * m_TranslationCoeff; 
			float left = Input.GetAxis ("Horizontal") * Time.deltaTime * m_TranslationCoeff;
			m_Character.transform.position += m_Head.transform.forward * forward + m_Head.transform.right * left;

			// Mouse plannar
			if (Input.GetMouseButtonDown (2)) {
				m_Mouse = Input.mousePosition;
			}
			if (Input.GetMouseButton (2)) {
				float up = -(Input.mousePosition - m_Mouse).y * m_TranslationMouseCoeff * Time.deltaTime;
				float right = -(Input.mousePosition - m_Mouse).x * m_TranslationMouseCoeff * Time.deltaTime;

				m_Character.transform.position += m_Head.transform.up * up + m_Head.transform.right * right;
				m_Mouse = Input.mousePosition;
			}

			// Mouse scroll
			float scroll = Input.mouseScrollDelta.y * m_TranslationMouseScrollCoeff * Time.deltaTime;
			m_Character.transform.position += m_Head.transform.forward * scroll;


			if (m_MouseLookOnClickOnly && !Input.GetMouseButton (1))
				return;
				
			// Mouse rotation
			float x = Input.GetAxis ("Mouse X") * Time.deltaTime * m_RotationCoeff;
			float y = -Input.GetAxis ("Mouse Y") * Time.deltaTime * m_RotationCoeff;
			m_Head.localRotation = ClampRotationAroundXAxis (m_Head.localRotation * Quaternion.AngleAxis (y, Vector3.right));
			m_Character.localRotation *= Quaternion.AngleAxis (x, Vector3.up);
		}

		Quaternion ClampRotationAroundXAxis (Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1.0f;

			float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

			angleX = Mathf.Clamp (angleX, -80f, 80f);

			q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

			return q;
		}
	}
}