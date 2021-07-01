using UnityEngine;
using System.Collections;
public enum FNPC_EnemyState{IDLE_STATIC,IDLE_ROAMER,IDLE_PATROL,INSPECT,ATTACK,FIND_WEAPON,KNOCKED_OUT,DEAD,NONE}
public enum FNPC_WeaponType{KNIFE,RIFLE,SHOTGUN}

public class FT_Enemy : MonoBehaviour {

	public float inspectTimeout; //Once the npc reaches the destination, how much time unitl in goes back.
	public UnityEngine.AI.NavMeshAgent navMeshAgent;
    public Vector3 speed = new Vector4(6, 7, 16);   //IDLE PATROL:X, IDLE ROAMER:Y, INSPECT:Z.
    //public Animator npcAnimator;

    public GameObject proyectilePrefab;
	delegate void InitState();
	delegate void UpdateState();
	delegate void EndState();
	InitState _initState;
	InitState _updateState;
	InitState _endState;
	public FNPC_WeaponType weaponType=FNPC_WeaponType.KNIFE;
	public FNPC_EnemyState idleState=FNPC_EnemyState.IDLE_ROAMER;
	FNPC_EnemyState currentState=FNPC_EnemyState.NONE;
	Vector3 targetPos,startingPos;
	public LayerMask hitTestLayer;
	float weaponRange;
	public Transform weaponPivot;
	float weaponActionTime,weaponTime;
	int hashSpeed;
    //public NPC_PatrolNode patrolNode; //id3644
    // Use this for initialization

    //id3644
    [Header ("[---]")]
    public GameObject meleeWeapon;
    public GameObject rangedWeapon;

    public GameObject exp;

	void Start () {
		startingPos = transform.position;
		hashSpeed = Animator.StringToHash ("Speed");
		SetWeapon (weaponType);
		SetState (idleState);
        FT_Manager.AddToEnemyCount ();
	}
	void SetWeapon(FNPC_WeaponType newWeapon){
        //npcAnimator.SetTrigger("WeaponChange");
        //npcAnimator.SetInteger ("WeaponType", (int)weaponType);
        toggleWeapon();

        switch (weaponType) {
			case FNPC_WeaponType.KNIFE:
				weaponRange=1.0f;
				weaponActionTime=0.2f;
				weaponTime=0.4f;
                break;
			case FNPC_WeaponType.RIFLE:
				weaponRange=20.0f;
				weaponActionTime=0.025f;
				weaponTime=0.05f;
                break;
            case FNPC_WeaponType.SHOTGUN:
                weaponRange=20.0f;
                weaponActionTime=0.35f;
                weaponTime=0.75f;
                break;
		}
	}
    public void toggleWeapon()
    {
        if(meleeWeapon != null)
            meleeWeapon.SetActive(false);

        if (rangedWeapon != null)
            rangedWeapon.SetActive(false);

        switch (weaponType)
        {
            case FNPC_WeaponType.KNIFE:
                if (meleeWeapon != null)
                    meleeWeapon.SetActive(true);

                break;
            case FNPC_WeaponType.RIFLE:
                if (rangedWeapon != null)
                    rangedWeapon.SetActive(true);

                break;
            case FNPC_WeaponType.SHOTGUN:
                if (rangedWeapon != null)
                    rangedWeapon.SetActive(true);

                break;
        }
    }

	// Update is called once per frame
	void Update () {
		_updateState ();

		//npcAnimator.SetFloat (hashSpeed, navMeshAgent.velocity.magnitude);
	}
	public void SetState(FNPC_EnemyState newState){
		if (currentState != newState) {
			if(_endState!=null)
				_endState();
			switch(newState){
				case FNPC_EnemyState.IDLE_STATIC:  _initState=StateInit_IdleStatic; 	_updateState=StateUpdate_IdleStatic; 	_endState=StateEnd_IdleStatic; 	break;				
				case FNPC_EnemyState.IDLE_ROAMER:  _initState=StateInit_IdleRoamer; 	_updateState=StateUpdate_IdleRoamer; 	_endState=StateEnd_IdleRoamer; 	break;			
				case FNPC_EnemyState.IDLE_PATROL:  _initState=StateInit_IdlePatrol; 	_updateState=StateUpdate_IdlePatrol; 	_endState=StateEnd_IdlePatrol; 	break;			
				case FNPC_EnemyState.INSPECT:  _initState=StateInit_Inspect; 	_updateState=StateUpdate_Inspect; 	_endState=StateEnd_Inspect; 	break;			
				case FNPC_EnemyState.ATTACK:  _initState=StateInit_Attack; 	_updateState=StateUpdate_Attack; 	_endState=StateEnd_Attack; 	break;			
			}
			_initState();			
			currentState=newState;					
		}
	}

	void UpdateSensors(){
		
	}

	///////////////////////////////////////////////////////// STATE: IDLE STATIC


	void StateInit_IdleStatic(){	
		navMeshAgent.SetDestination (startingPos);
        //navMeshAgent.Resume ();
        navMeshAgent.isStopped = false;
    }
	void StateUpdate_IdleStatic(){	

		
	}
	void StateEnd_IdleStatic(){	
	}
	///////////////////////////////////////////////////////// STATE: IDLE PATROL
	
	
	void StateInit_IdlePatrol(){	
		//navMeshAgent.speed = 6.0f;
        navMeshAgent.speed = speed.x;
        //navMeshAgent.SetDestination (patrolNode.GetPosition ());  id3644
    }
	void StateUpdate_IdlePatrol(){	
		if (HasReachedMyDestination ()) {
			//patrolNode=patrolNode.nextNode;   //id3644
			//navMeshAgent.SetDestination (patrolNode.GetPosition ());  //id3644
		}
		
	}
	void StateEnd_IdlePatrol(){	
	}

	///////////////////////////////////////////////////////// STATE: IDLE ROAMER


	FT_Timer idleTimer=new FT_Timer();
    FT_Timer idleRotateTimer =new FT_Timer();
	bool idleWaiting,idleMoving;
	void StateInit_IdleRoamer(){
        //navMeshAgent.speed = 7.0f;
        navMeshAgent.speed = speed.y;

        idleTimer.StartTimer (Random.Range (2.0f, 4.0f));
		RandomRotate ();
		AdvanceIdle ();
		idleWaiting = false;
		idleMoving = true;

	}
	void StateUpdate_IdleRoamer(){	
	
		idleTimer.UpdateTimer ();
	
		if (idleMoving) {
			if (HasReachedMyDestination ()) {
				AdvanceIdle();

			}
		} else if(idleWaiting){
			idleRotateTimer.UpdateTimer ();
			if(	idleRotateTimer.IsFinished()){
				RandomRotate();
				idleRotateTimer.StartTimer(Random.Range(1.5f,3.25f));
			}
		
		}
		if (idleTimer.IsFinished ()) {
			if(idleMoving){
                //navMeshAgent.Stop();
                navMeshAgent.isStopped = true;
                float waitTime=Random.Range (2.5f,6.5f);
				float randomTurnTime=waitTime/2.0f;
				idleRotateTimer.StartTimer (randomTurnTime);
				idleTimer.StartTimer (waitTime);

			
			}
			else if(idleWaiting){
				idleTimer.StartTimer (Random.Range (2.0f, 4.0f));

				AdvanceIdle();
			}

			idleMoving=!idleMoving;
			idleWaiting=!idleMoving;

		}

	}
	void StateEnd_IdleRoamer(){	
	}


	void RayDebug(){
		RaycastHit hit = new RaycastHit ();
		Physics.Raycast (transform.position, transform.forward*5.0f, out hit,50.0f,hitTestLayer);

		Debug.DrawLine(transform.position, hit.point, Color.red);
		Vector3 dir =  hit.point-transform.position;
		Vector3 reflectedVector = Vector3.Reflect (dir,hit.normal);	
		Debug.DrawRay (hit.point, reflectedVector*5.0f, Color.green);				
	}

	void AdvanceIdle(){

		RaycastHit hit = new RaycastHit ();
		Physics.Raycast (transform.position, transform.forward*5.0f, out hit,50.0f,hitTestLayer);
		//Debug.DrawRay (transform.position, transform.forward, Color.red);

		if (hit.distance < 3.0f) {
			Vector3 dir =  hit.point-transform.position;
			Vector3 reflectedVector = Vector3.Reflect (dir,hit.normal);	
			Physics.Raycast (transform.position,reflectedVector, out hit,50.0f,hitTestLayer);
		}
        
        //navMeshAgent.Resume();
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination (hit.point);

	
	}
    ///////////////////////////////////////////////////////// STATE: INSPECT
    FT_Timer inspectTimer = new FT_Timer();
    FT_Timer inspectTurnTimer = new FT_Timer();
	bool inspectWait;
	void StateInit_Inspect(){
        //navMeshAgent.speed = 16.0f;
        navMeshAgent.speed = speed.z;
		//navMeshAgent.Resume ();
        navMeshAgent.isStopped = false;
        inspectTimer.StopTimer ();
		inspectWait = false;
	}
	void StateUpdate_Inspect(){	


		if (HasReachedMyDestination () && !inspectWait) {
			inspectWait=true;
			inspectTimer.StartTimer (2.0f);
			inspectTurnTimer.StartTimer(1.0f);
		}
		navMeshAgent.SetDestination (targetPos);
		RaycastHit hit = new RaycastHit ();
		Physics.Raycast (transform.position,transform.forward, out hit,weaponRange,hitTestLayer);

		if (hit.collider != null && hit.collider.tag == "Player") {
			SetState(FNPC_EnemyState.ATTACK);
		}
		if (inspectWait) {
			inspectTimer.UpdateTimer ();
			inspectTurnTimer.UpdateTimer();
			if (inspectTurnTimer.IsFinished ()) {
				RandomRotate ();
				inspectTurnTimer.StartTimer (Random.Range (0.5f, 1.25f));
			}
			if (inspectTimer.IsFinished ())
				SetState (idleState);
		}
	}
	void StateEnd_Inspect(){	
	}

    ///////////////////////////////////////////////////////// STATE: ATTACK
    FT_Timer attackActionTimer =new FT_Timer();
	bool actionDone;
	void StateInit_Attack(){
        //navMeshAgent.Stop ();
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
		//npcAnimator.SetBool ("Attack", true);		
		CancelInvoke ("AttackAction");
		Invoke ("AttackAction", weaponActionTime);
		attackActionTimer.StartTimer (weaponTime);

		actionDone = false;
	}
	void StateUpdate_Attack(){	
		attackActionTimer.UpdateTimer ();
		if (!actionDone && attackActionTimer.IsFinished ()) {
			EndAttack();

			actionDone=true;
		}
	}
	void StateEnd_Attack(){	
		//npcAnimator.SetBool ("Attack", false);
	}
	void EndAttack(){
		SetState (FNPC_EnemyState.INSPECT);
	}
	void AttackAction(){
		switch (weaponType) {
			case FNPC_WeaponType.KNIFE:
			RaycastHit[] hits=Physics.SphereCastAll (weaponPivot.position,2.0f, weaponPivot.forward);
			foreach(RaycastHit hit in hits){
				if (hit.collider!=null && hit.collider.tag == "Player") {
                        //hit.collider.GetComponent<TP_Player>().DamagePlayer();
                        hit.collider.GetComponent<FT_Player>().HitDamage();
                    }
			}
			break;
			case FNPC_WeaponType.RIFLE:
				GameObject bullet=GameObject.Instantiate(proyectilePrefab, weaponPivot.position,weaponPivot.rotation) as GameObject;
				bullet.transform.Rotate(0,Random.Range(-7.5f,7.5f),0);
			break;
			case FNPC_WeaponType.SHOTGUN:
				for(int i=0;i<5;i++){
					GameObject birdshot=GameObject.Instantiate(proyectilePrefab, weaponPivot.position,weaponPivot.rotation) as GameObject;
					birdshot.transform.Rotate(0,Random.Range(-15,15),0);
				}
			break;
		}
	}
	////////////////////////// MISC FUNCTIONS //////////////////////////

	void RandomRotate(){
		float randomAngle =Random.Range (45, 180);
		float randomSign = Random.Range (0, 2);
		if (randomSign == 0)
			randomAngle *= -1;

		transform.Rotate (0, randomAngle, 0);
	}
	/*float randomMoveInnerRadius=0.5f, randomMoveOuterRadius=10.0f;
	private Vector3 GetRandomPoint(){	
		Vector3 newPos;
		//do{
			newPos=Random.insideUnitSphere * randomMoveOuterRadius;
		//}while(newPos.x <randomMoveInnerRadius && newPos.y<randomMoveInnerRadius);
		Vector3 finalPos = transform.position + newPos;

		return finalPos;
	}*/
	public bool HasReachedMyDestination(){
		float dist = Vector3.Distance (transform.position, navMeshAgent.destination);
		if ( dist<= 1.5f) {
			return 	true;
		}
		
		return false;
	}
	////////////////////////// PUBLIC FUNCTIONS //////////////////////////
	public void SetAlertPos(Vector3 newPos){
		if (idleState != FNPC_EnemyState.IDLE_STATIC) {
			SetTargetPos(newPos);
		}
	}
	public void SetTargetPos(Vector3 newPos){
		targetPos = newPos;
		if (currentState != FNPC_EnemyState.ATTACK ) {
			SetState (FNPC_EnemyState.INSPECT);
		}
	}
	public void Damage(){
		navMeshAgent.velocity = Vector3.zero;
		//navMeshAgent.Stop ();
		//npcAnimator.SetBool ("Dead", true);
        FT_Manager.AddScore (1);
        //npcAnimator.transform.parent = null;
        //Vector3 pos = npcAnimator.transform.position;
        //pos.y = 0.2f;
        //npcAnimator.transform.position = pos;

        Vector3 deadPos = new Vector3(transform.position.x, 0, transform.position.z);
        Instantiate(exp, deadPos, Quaternion.identity);

        FT_Manager.RemoveEnemy ();		
		Destroy (gameObject);
	}

}
