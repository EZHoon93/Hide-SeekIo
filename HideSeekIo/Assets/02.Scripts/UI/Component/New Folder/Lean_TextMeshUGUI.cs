using System.Collections;

using UnityEngine;
using Lean.Localization;
using TMPro;

public class Lean_TextMeshUGUI : LeanLocalizedBehaviour
{
	public string FallbackText;

	public override void UpdateTranslation(LeanTranslation translation)
	{

		// Get the TextMesh component attached to this GameObject
		var text = GetComponent<TextMeshProUGUI>();

		// Use translation?
		if (translation != null )
		{
			text.text = LeanTranslation.FormatText((string)translation.Data, text.text, this);

		}
		// Use fallback?
		else
		{
			text.text = LeanTranslation.FormatText(FallbackText, text.text, this);

		}
	}

	protected virtual void Awake()
	{
		// Should we set FallbackText?
		if (string.IsNullOrEmpty(FallbackText) == true)
		{
			// Get the TextMeshProUGUI component attached to this GameObject
			var text = GetComponent<TextMeshProUGUI>();

			// Copy current text to fallback
			FallbackText = text.text;
		}
	}

}
