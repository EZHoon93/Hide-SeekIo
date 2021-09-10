

using UnityEngine;
namespace BehaviorDesigner.Runtime.Tasks.Unity
{
    [TaskCategory("EZ")]

    public class CheckCoolTime : Conditional
    {
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

            if (_playerController.playerShooter.GetBaseWeaponRemainCoolTime() > 0)
            {
                return TaskStatus.Failure;
            }

            return TaskStatus.Success;


        }
    }
}