using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlmostEngine {
	public class MultiDisplayUtils : MonoBehaviour
	{

		public static bool IsMultiDisplay ()
		{
			#if (UNITY_5_6_OR_NEWER)
			
					if (Display.displays.Length == 1)
						return false;
					
					for (int i = 1; i < Display.displays.Length; ++i) {
						if (Display.displays[i].active)
							return true;
					}
					
					return false;

			#else

			return Display.displays.Length > 1;

			#endif
		}
	}
}