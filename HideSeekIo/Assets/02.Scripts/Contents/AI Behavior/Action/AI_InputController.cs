

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{
    [TaskCategory("EZ")]

    public class AI_InputController : Action
    {
        public SharedInputType sharedInputType;
        public SharedControllerInputType sharedControllerInputType;
        public SharedGameObject sharedTarget;
        PlayerController _playerController;
        public override void OnAwake()
        {
            _playerController = GetComponent<PlayerController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_playerController == null)
            {
                return TaskStatus.Failure;
            }
            var direction = (sharedTarget.Value.transform.position - this.transform.position).normalized;
            var inputVector2 = new Vector2(direction.x, direction.z);
            if (_playerController.playerShooter.baseWeapon)
            {
                var distance = Vector3.Distance(sharedTarget.Value.transform.position, this.transform.position);
                var mag = distance / _playerController.playerShooter.baseWeapon.AttackDistance;
                inputVector2 = inputVector2 * mag;
            }
            _playerController.playerInput.GetControllerInput(sharedInputType.Value).Call(sharedControllerInputType.Value, inputVector2);
            //p_playerController.playerInput.controllerInputDic[sharedInputType.Value].Call(sharedControllerInputType.Value, inputVector2);

            return TaskStatus.Success;
        }

        Vector2 GetInputVector2(Vector3 targetPos)
        {
            var direction = (targetPos - _playerController.transform.position).normalized;

            //Quaternion quaternion = Quaternion.Euler(0, 0, 0);
            //var result = quaternion * direction;

            return new Vector2(direction.x, direction.z);
        }
    }
}