using System;

using Photon.Pun;

using UnityEngine;

public class MoveEnergyController : MonoBehaviour
{
    [SerializeField] float _currentEnergy;
    [SerializeField] float _maxEnergy;

    [SerializeField] PlayerController _playerController;
    [SerializeField] PlayerMove _playerMove;
    [SerializeField]  PlayerUI _playerUI;


    private void Awake()
    {
        this.gameObject.SetActive(false);
    }
    public void OnPhotonInstantiate(PlayerController playerController)
    {
        _playerController.playerUI.SetActiveMoveEnergyUI(false);
        this.gameObject.SetActive(false);
    }

    public void OnPreNetDestroy(PhotonView rootView)
    {

    }

    public void ChangeOwnerShipOnUser(bool isMyCharacter)
    {
        if (_playerController.Team == Define.Team.Hide && isMyCharacter)
        {
            this.gameObject.SetActive(true);
            _playerUI.ChangeMaxMoveEnergy(_maxEnergy);
            _playerController.playerUI.SetActiveMoveEnergyUI(false);
            return;
        }
    }

    private void Update()
    {
        if(_playerMove.State == PlayerMove.MoveState.Run)
        {
            _currentEnergy = Mathf.Clamp(_currentEnergy + Time.deltaTime, 0, _maxEnergy);
        }
        else
        {
            _currentEnergy = Mathf.Clamp(_currentEnergy - Time.deltaTime, 0, _maxEnergy);
        }

        _playerUI.ChangeCurrentMoveEnergy(_currentEnergy);

    }
}
