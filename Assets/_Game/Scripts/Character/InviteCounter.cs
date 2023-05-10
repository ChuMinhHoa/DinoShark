using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InviteCounter : MonoBehaviour
{
    [SerializeField] Slider slider;
    float maxInviteTime;
    float remain;
    private void OnEnable()
    {
        maxInviteTime = ProfileManager.Instance.playerData.GetInviteSave().GetInviteRemain();
        remain = maxInviteTime;
    }

    private void Update()
    {
        remain -= Time.deltaTime;
        slider.value = remain / maxInviteTime;
    }
}
