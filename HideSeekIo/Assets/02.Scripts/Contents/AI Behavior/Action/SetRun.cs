

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{
    [TaskCategory("EZ")]

    public class SetRun : Action
    {
        PlayerController _playerController;
        public SharedBool setValue;

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

            //_playerController.playerMove.Run = setValue.Value;

            return TaskStatus.Success;


        }
    }
}