

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{
    [TaskCategory("EZ")]

    public class AI_InputController : Action
    {
        public SharedInputType sharedInputType;
        public SharedControllerInputType sharedControllerInputType;
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

            _playerController.playerInput.controllerInputDic[sharedInputType.Value].Call(sharedControllerInputType.Value, Vector2.zero);

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