using System;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownDisplay: MonoBehaviour
{
    [SerializeField] private Image _coolDownImage;
    [SerializeField] private PlayerMovement _playerMovement;

    private void Update()
    {
        float fill = Mathf.InverseLerp(0, _playerMovement.DashCoolDown,_playerMovement.CurrentDashCoolDown);
        if (Mathf.Approximately(1f,fill))
        {
            fill = 0;
        }
        
        _coolDownImage.fillAmount = fill;
    }
}
