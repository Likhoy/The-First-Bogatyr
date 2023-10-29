using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseMoneyOnDestroy : MonoBehaviour
{
    [SerializeField] private int moneyAmount;

    private void OnDestroy()
    {
        Player player = GameManager.Instance.GetPlayer();
        player.playerResources.AddMoney(moneyAmount);
    }
}
