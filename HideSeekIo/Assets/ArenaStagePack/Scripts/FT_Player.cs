using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum FPlayerWeaponType{KNIFE,PISTOL,NULL}

public class FT_Player : MonoBehaviour {

	Rigidbody myRigidBody;
	public float moveSpeed=10.0f;
	public Transform hitTestPivot,gunPivot;
	public GameObject mousePointer,proyectilePrefab;
	//public Animator animator;
	int hashSpeed;
	float attackTime=0.4f;
	 FPlayerWeaponType currentWeapon=FPlayerWeaponType.NULL;
    FT_Timer attackTimer = new FT_Timer();

    //Tank
    [Header ("[---]")]
    public int playerHealth = 0;
    public int playerLife;
    public GameObject healthBar;

    public Transform turretTr;

    public float m_TurnSpeed = 180f;
    private float m_MovementInputValue;
    private float m_TurnInputValue;

    //cam test
    private float camDist;
    

    // Use this for initialization
    void Awake() {

	}
	void Start () {
		FSetWeapon (FPlayerWeaponType.PISTOL);
		myRigidBody = GetComponent<Rigidbody> ();
		hashSpeed = Animator.StringToHash ("Speed");
		attackTimer.StartTimer (0.1f);
        //	Cursor.visible = false;

        //id3644
        camDist = Vector3.Distance(Camera.main.transform.position, transform.position);
        playerLife = playerHealth;
        Image hpBar = healthBar.GetComponent<Image>();
        hpBar.transform.localScale = new Vector3(1, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
    }
	
	// Update is called once per frame
	void Update () {
		//animator.SetFloat (hashSpeed, myRigidBody.velocity .magnitude);
		float inputHorizontal = Input.GetAxis ("Horizontal");
		float inputVertical = Input.GetAxis ("Vertical");

        float speedY = inputVertical > 0.1 ? Mathf.Clamp ((inputVertical * moveSpeed), moveSpeed / 2.0f, moveSpeed) : 0.0f;
		float speedX = inputHorizontal > 0.1 ? Mathf.Clamp ((inputHorizontal * moveSpeed), moveSpeed / 2.0f, moveSpeed) : 0.0f;
		Vector3 newVelocity=new Vector3(inputVertical*moveSpeed, 0.0f, inputHorizontal*-moveSpeed);
        myRigidBody.velocity = newVelocity;

        m_MovementInputValue = inputVertical;   //id3644
        m_TurnInputValue = inputHorizontal;   //id3644

        //transform.rotation = Quaternion.Euler(newVelocity);
        float deltaY = -inputHorizontal;
        float deltaX = inputVertical;
        float angleInDegrees = Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI;
        transform.eulerAngles = new Vector3(0, -angleInDegrees, 0);

        switch (currentWeapon) {
			case FPlayerWeaponType.KNIFE:
				if (Input.GetMouseButton (0) && attackTimer.IsFinished()) {
					Attack();
				}
			break;
			case FPlayerWeaponType.PISTOL:
				if (Input.GetMouseButtonDown (0) && attackTimer.IsFinished()) {

					Attack();
				}
			break;
		}

        /*
		if (Input.GetKeyDown (KeyCode.Alpha1))
			SetWeapon (PlayerWeaponType.KNIFE);
		if (Input.GetKeyDown (KeyCode.Alpha2))
			SetWeapon (PlayerWeaponType.PISTOL);
        */

		attackTimer.UpdateTimer ();
		UpdateAim ();
	}

    private void FixedUpdate()
    {
        //Move();
        //Turn();
    }

    public void HitDamage()
    {
        if (playerLife > 1)
        {
            playerLife -= 1;
            float point = 1.0f / playerHealth;
            //float point = 0.2f;
            Image hpBar = healthBar.GetComponent<Image>();
            hpBar.transform.localScale = new Vector3(hpBar.transform.localScale.x - point, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
        }
        else
        {
            Image hpBar = healthBar.GetComponent<Image>();
            hpBar.transform.localScale = new Vector3(0, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
            DamagePlayer();
        }
    }

    public void DamagePlayer(){
		//animator.SetBool ("Dead", true);
		//animator.transform.parent = null;
		this.enabled = false;
		myRigidBody.isKinematic = true;
        FT_Manager.RegisterPlayerDeath ();
		gameObject.GetComponent<Collider> ().enabled = false;
        FT_Cam.ToggleShake (0.3f);
		//Vector3 pos = animator.transform.position;
		//pos.y = 0.2f;
		//animator.transform.position = pos;
	}

	void UpdateAim(){

        Vector3 mousePosOffset = Input.mousePosition + new Vector3(0,0, camDist);   //id3644
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosOffset);   //id3644
        Vector3 mousePosRevision = new Vector3(mousePos.x + mousePos.y, transform.position.y, mousePos.z);
        mousePointer.transform.position = mousePosRevision;

        //Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        //mousePos.y = transform.position.y;
        //mousePointer.transform.position = mousePos;
        float deltaY = mousePos.z - transform.position.z;
		float deltaX = mousePos.x - transform.position.x;
		float angleInDegrees = Mathf.Atan2 (deltaY, deltaX) * 180 / Mathf.PI;
		//transform.eulerAngles = new Vector3 (0, -angleInDegrees, 0);

        turretTr.eulerAngles = new Vector3(0, -angleInDegrees + 90, 0);
    }
	public void Attack(){
		switch (currentWeapon) {
			case FPlayerWeaponType.KNIFE:							
				Invoke ("DoHitTest",0.2f);				
			break;
			case FPlayerWeaponType.PISTOL:
                FT_Cam.ToggleShake (0.1f);
				GameObject bullet=GameObject.Instantiate(proyectilePrefab, gunPivot.position,gunPivot.rotation) as GameObject;
				bullet.transform.LookAt(mousePointer.transform);
				bullet.transform.Rotate(0,Random.Range(-7.5f,7.5f),0);
				AlertEnemies();
			break;
		}
		//animator.SetBool ("Attack", true);
		CancelInvoke ("AttackOver");
		Invoke ("AttackOver", attackTime);
		attackTimer.StartTimer (attackTime);

	}
	void AlertEnemies(){
		RaycastHit[] hits=Physics.SphereCastAll (hitTestPivot.position,20.0f, hitTestPivot.up);
		foreach (RaycastHit hit in hits) {
			if (hit.collider != null && hit.collider.tag == "Enemy") {
				hit.collider.GetComponent<FT_Enemy>().SetAlertPos(transform.position);
			}
		}
	}
	public void DoHitTest(){




		RaycastHit[] hits=Physics.SphereCastAll (hitTestPivot.position,2.0f, hitTestPivot.up);
		foreach(RaycastHit hit in hits){
			if (hit.collider!=null && hit.collider.tag == "Enemy") {
				RaycastHit forwarHit= new RaycastHit();
				Physics.Raycast(hitTestPivot.position,hit.transform.position-transform.position,out forwarHit);
				if (forwarHit.collider!=null && forwarHit.collider.tag == "Enemy") {
					forwarHit.collider.GetComponent<FT_Enemy>().Damage();
				}
			}
		}
	}
	void AttackOver(){
		//animator.SetBool ("Attack", false);
	}
	
	void FSetWeapon(FPlayerWeaponType _weaponType){
		if (_weaponType != currentWeapon) {
			currentWeapon = _weaponType;
			//animator.SetTrigger ("WeaponChange");
			switch (_weaponType) {
			case FPlayerWeaponType.KNIFE:
				attackTime=0.4f;
				//animator.SetInteger ("WeaponType", 0);
				break;
			case FPlayerWeaponType.PISTOL:
				attackTime=0.1f;
				//animator.SetInteger ("WeaponType", 3);
				break;
			}
		}
        FT_Manager.SelectWeapon (_weaponType);
	}

    private void Move()
    {
        // Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
        //Vector3 movement = transform.forward * m_MovementInputValue * moveSpeed * Time.deltaTime;
        Vector3 movement = transform.right * m_MovementInputValue * moveSpeed * Time.deltaTime;

        // Apply this movement to the rigidbody's position.
        myRigidBody.MovePosition(myRigidBody.position + movement);
    }

    private void Turn()
    {
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        myRigidBody.MoveRotation(myRigidBody.rotation * turnRotation);
    }
}
