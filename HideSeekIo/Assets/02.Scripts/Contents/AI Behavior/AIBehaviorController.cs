

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{

    public class AIBehaviorController : Action
    {
        public SharedInputType sharedInputType;
        public SharedControllerInputType sharedControllerInputType;
        public SharedGameObject targetObject;
        public SharedFloat attackDistance;

        PlayerController _playerController;

        IAttack _attack;
        

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
            var direction = GetInputVector2(targetObject.Value.transform.position);
            var distance = Vector3.Distance(targetObject.Value.transform.position, _playerController.transform.position);
            //var inputVector = direction / _playerController.GetAttackBase().currentAttack.AttackDistance;
            //Debug.Log(_playerController.GetAttackBase().currentAttack.AttackDistance + "어택디스턴스");
            //Debug.Log(attackDistance.Value +"/"+direction + "/ " + inputVector + "/" + inputVector * distance +"/"+ distance);

            //_playerController.inputBase.controllerInputDic[sharedInputType.Value].Call(sharedControllerInputType.Value, inputVector * distance);

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