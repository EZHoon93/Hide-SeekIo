using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    protected CharacterController _characterController;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

    }

    private void Update()
    {


    }

    private void FixedUpdate()
    {
        var moveVector = new Vector3( UltimateJoystick.GetHorizontalAxis("Move"),0 , UltimateJoystick.GetVerticalAxis("Move"));

        Vector3 moveDistance = moveVector.normalized * 2 * Time.deltaTime;
        if (!_characterController.isGrounded)
        {
            moveDistance.y -= 9.8f * Time.deltaTime;
        }
        _characterController.Move(moveDistance);
    }
}
