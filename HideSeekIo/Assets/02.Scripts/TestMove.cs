using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    protected CharacterController _characterController;
    Animator animator;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

    }

    private void Update()
    {
        if(_characterController == null)
        {
            _characterController = this.gameObject.GetOrAddComponent<CharacterController>();
        }
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 move = new Vector2(h, v);
        Vector3 inputVector3 = UtillGame.GetInputVector3_ByCamera(move);
        Vector3 moveDistance = inputVector3.normalized * 2 * Time.deltaTime;
        if (!_characterController.isGrounded)
        {
            moveDistance.y -= 9.8f * Time.deltaTime;
        }
        _characterController.Move(moveDistance);
        UpdateSmoothRotate(inputVector3);
        if (animator)
        {
            animator.SetFloat("Speed", inputVector3.magnitude);
        }
    }

    protected void UpdateSmoothRotate(Vector3 inputVector3)
    {
        if (inputVector3.sqrMagnitude == 0) return;
        Quaternion quaternion = Quaternion.Euler(0, 0, 0);
        var newDirection = quaternion * inputVector3;
        Quaternion newRotation = Quaternion.LookRotation(newDirection);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, 5 * Time.deltaTime);
    }
}
