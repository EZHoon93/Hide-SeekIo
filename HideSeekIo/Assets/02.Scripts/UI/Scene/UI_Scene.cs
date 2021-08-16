using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
	public override void Init()
	{
		print("UOScene This");
		Managers.UI.SceneUI = this;
		Managers.UI.SetCanvas(gameObject, false);
	}
}
