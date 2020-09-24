using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] int _damageAmount = 1;

    private void OnCollisionEnter(Collision other)
    {
        PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>();
        if(player != null)
        {
            PlayerImpact(player);
        }
    }

    private void PlayerImpact (PlayerHealth player)
    {
        player.DecreaseHealth(_damageAmount);
    }
}
