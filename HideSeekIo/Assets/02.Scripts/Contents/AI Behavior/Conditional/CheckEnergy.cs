

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{
    [TaskCategory("EZ")]

    public class CheckEnergy : Conditional
    {
        public SharedFloat checkEnergy;
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

            if( _playerController.playerShooter.currentEnergy >= checkEnergy.Value)
            {
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;


        }
    }
}