using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Sensors change a NPC_Base status condition
/// </summary>
public class FT_Sensor : MonoBehaviour {
    
	public FT_Enemy npcBase;
    protected List<GameObject> sensedObjects=new List<GameObject>();
    //public List<GameObject> sensedObjects = new List<GameObject>();

    void Start () {
		if (npcBase == null)
			npcBase = gameObject.GetComponent<FT_Enemy> ();
		StartSensor ();
	}

	void Update () {
		UpdateSensor ();
	}
	protected virtual void StartSensor(){}
	protected virtual void UpdateSensor(){}

	protected List<GameObject> GetSensedObjects(){
		return sensedObjects;
	}
    
}

