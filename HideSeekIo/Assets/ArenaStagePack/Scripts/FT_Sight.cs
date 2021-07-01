using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FT_Sight : FT_Sensor {
    //const float SIGHT_DIRECT_ANGLE =120.0f,SIGHT_MIN_DISTANCE=0.2f,SIGHT_MAX_DISTANCE=20.0f;//,SIGHT_INDIRECT_ANGLE = 80,SIGHT_INDIRECT_DISTANCE=20.0f;
    const float SIGHT_DIRECT_ANGLE = 120.0f, SIGHT_MIN_DISTANCE = 0.2f, SIGHT_MAX_DISTANCE = 10.0f;
    float height=2.0f;
	public LayerMask hitTestMask;
	Color fovColor;
	float TARGET_LOST_COOLDOWN=1.0f,ALERTED_COOLDOWN=10.0f,lastTargetTime=float.MinValue,lastAlertTime=float.MinValue;
	Vector3 _lastTargetPos;
	bool alerted=false;
    //bool somethingSpotted=false;
    public float lastSightTime =float.MinValue;
    public float SIGHT_DELAY_TIME =0.1f; //Time a object has to stay in sight to catch our attention
	public Color idleColor, alertedColor, attackColor;
	//TODO ADD the visual thingy
	protected override void StartSensor(){
		//InitFoV ();
		//material.SetColor ("_Color", Color.green);	
	}
	protected override void UpdateSensor(){
		GetTargetInSight ();
		//UpdateFoV ();
	}
	bool targetSpotted=false;
	void GetTargetInSight(){
		Collider[] overlapedObjects= Physics.OverlapSphere (transform.position, SIGHT_MAX_DISTANCE);
	


	
		for (int i=0; i<overlapedObjects.Length; i++) {
			Vector3 direction = overlapedObjects [i].transform.position - transform.position;
			float objAngle = Vector3.Angle (direction, transform.forward);
			if (overlapedObjects [i].tag == "Player") { 
				if ( objAngle < SIGHT_DIRECT_ANGLE && TargetInSight (overlapedObjects [i].transform, SIGHT_MAX_DISTANCE )) {
					npcBase.SetTargetPos(overlapedObjects [i].transform.position);
					//material.SetColor ("_Color", attackColor);	
				}
				else{
					//material.SetColor ("_Color", idleColor);	
				}
			/*	if (objAngle < SIGHT_INDIRECT_ANGLE && TargetInSight (overlapedObjects [i].transform, SIGHT_INDIRECT_DISTANCE / 2.0f)) {
					if(!somethingSpotted){
						lastSightTime=Time.time;
						somethingSpotted=true;
					}
					if(lastSightTime+SIGHT_DELAY_TIME>Time.time){ //Cant react yet
						return;
					}

					_lastTargetPos = overlapedObjects [i].transform.position;
					material.SetColor ("_Color", Color.magenta);
					npcBase.SetTargetPos (_lastTargetPos);
					lastAlertTime=Time.time;
					if(!alerted){
						//npcBase.SetStatusCondition(NPC_Condition.ALERTED, true);	
						alerted=true;
					}
				}
				if (alerted && objAngle < SIGHT_DIRECT_ANGLE && TargetInSight (overlapedObjects [i].transform, SIGHT_DIRECT_DISTANCE / 2.0f)) {																							
						
						_lastTargetPos = overlapedObjects [i].transform.position;
						material.SetColor ("_Color", Color.red);						
						npcBase.SetTargetPos (_lastTargetPos);
						lastTargetTime = Time.time;
						lastAlertTime=Time.time;
						if(!targetSpotted){
							//npcBase.SetStatusCondition(NPC_Condition.HAS_TARGET, true);								
							targetSpotted = true;
						}
						
				} */

			}	else{
				//material.SetColor ("_Color", idleColor);	
				//somethingSpotted=false;
			}			
		}
		if (alerted && lastAlertTime + ALERTED_COOLDOWN < Time.time) {
			material.SetColor ("_Color", Color.green);
			alerted=false;
			//npcBase.SetStatusCondition(NPC_Condition.ALERTED, false);	
		}
		if (alerted && targetSpotted && lastTargetTime + TARGET_LOST_COOLDOWN < Time.time) {
				if(alerted)
					material.SetColor ("_Color", Color.magenta);
				else
					material.SetColor ("_Color", Color.green);	
				targetSpotted=false;
				//npcBase.SetStatusCondition(NPC_Condition.HAS_TARGET, false);

		} 
				
	
		
	}
	bool TargetInSight(Transform target,float distance){
		Vector3 sightPosition = transform.position;
		sightPosition.y += height;
		RaycastHit hit = new RaycastHit ();
		Vector3 dir= target.position-sightPosition;
		//Debug.DrawRay (headTransform.position, dir);
		Physics.Raycast (sightPosition,dir,out hit,distance,hitTestMask);
		return (hit.collider != null && target.gameObject == hit.collider.gameObject);
	}

	int quality = 100;
	Mesh mesh;
	public Material material;	
/*	float angle_fov = 40;	
	float dist_min = 0.1f;
	float dist_max = 10.0f;*/
	void InitFoV()
	{
		mesh = new Mesh();
		mesh.vertices = new Vector3[2 * quality + 2];   // Could be of size [2 * quality + 2] if circle segment is continuous
		mesh.triangles = new int[3 * 2 * quality];
		
		Vector3[] normals = new Vector3[2 * quality + 2];
		Vector2[] uv = new Vector2[2 * quality + 2];
		
		for (int i = 0; i < uv.Length; i++)
			uv[i] = new Vector2(0, 0);
		for (int i = 0; i < normals.Length; i++)
			normals[i] = new Vector3(0, 1, 0);
		
		mesh.uv = uv;
		mesh.normals = normals;
		transform.Rotate (0.0f, 360 * -0.05f, 0.0f);
		//iTween.RotateBy (gameObject, iTween.Hash ("y", 0.1f, "time", 4.0f, "looptype", iTween.LoopType.pingPong,"easetype",iTween.EaseType.easeInOutSine));

	}
	float FoVHeight=0.1f;
	void UpdateFoV()
	{
		float angle_lookat = GetEnemyAngle();
		
		float angle_start = angle_lookat - SIGHT_DIRECT_ANGLE;
		float angle_end = angle_lookat + SIGHT_DIRECT_ANGLE;
		float angle_delta = (angle_end - angle_start) / quality;
		
		float angle_curr = angle_start;
		float angle_next = angle_start + angle_delta;
		
		Vector3 pos_curr_min = Vector3.zero;
		Vector3 pos_curr_max = Vector3.zero;
		
		Vector3 pos_next_min = Vector3.zero;
		Vector3 pos_next_max = Vector3.zero;
		

		
		Vector3[] vertices = new Vector3[4 * quality];   // Could be of size [2 * quality + 2] if circle segment is continuous
		int[] triangles = new int[3 * 2 * quality];
		Vector3 fovPos = transform.position;
		fovPos.y += FoVHeight;
		for (int i = 0; i < quality; i++)
		{
			
			Vector3 sphere_curr = new Vector3(
				Mathf.Sin(Mathf.Deg2Rad * (angle_curr)), 0.0f,   // Left handed CW
				Mathf.Cos(Mathf.Deg2Rad * (angle_curr)));
			
			Vector3 sphere_next = new Vector3(
				Mathf.Sin(Mathf.Deg2Rad * (angle_next)), 0.0f,
				Mathf.Cos(Mathf.Deg2Rad * (angle_next)));
			
			pos_curr_min = fovPos + sphere_curr * SIGHT_MIN_DISTANCE;
			pos_curr_max = fovPos + sphere_curr * SIGHT_MAX_DISTANCE;
			pos_next_min = fovPos + sphere_next * SIGHT_MIN_DISTANCE;
			pos_next_max = fovPos + sphere_next * SIGHT_MAX_DISTANCE;
		
			int a = 4 * i;
			int b = 4 * i + 1;
			int c = 4 * i + 2;
			int d = 4 * i + 3;

			RaycastHit currRay=new RaycastHit(),nextRay=new RaycastHit();
			Physics.Raycast(pos_curr_min,pos_curr_max-pos_curr_min,out currRay,SIGHT_MAX_DISTANCE-SIGHT_MIN_DISTANCE);
			Physics.Raycast(pos_next_min,pos_next_max-pos_next_min,out nextRay,SIGHT_MAX_DISTANCE-SIGHT_MIN_DISTANCE);
			
			if(currRay.collider!=null){
				float dist=Vector3.Distance(currRay.point,pos_curr_min)+SIGHT_MIN_DISTANCE;
				pos_curr_max = transform.position + sphere_curr * dist;
			}
			if(nextRay.collider!=null){
				float dist=Vector3.Distance(nextRay.point,pos_next_min)+SIGHT_MIN_DISTANCE;
				pos_next_max = transform.position + sphere_next * dist;
			}
		
			vertices[a] = pos_curr_min; 
			vertices[b] = pos_curr_max;
			vertices[c] = pos_next_max;
			vertices[d] = pos_next_min;
			
			triangles[6 * i] = a;       // Triangle1: abc
			triangles[6 * i + 1] = b;  
			triangles[6 * i + 2] = c;
			triangles[6 * i + 3] = c;   // Triangle2: cda
			triangles[6 * i + 4] = d;
			triangles[6 * i + 5] = a;
			
			angle_curr += angle_delta;
			angle_next += angle_delta;
			
		}
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
		Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
	}
	
	float GetEnemyAngle()
	{
		return 90 - Mathf.Rad2Deg * Mathf.Atan2(transform.forward.z, transform.forward.x); // Left handed CW. z = angle 0, x = angle 90
	}

}
