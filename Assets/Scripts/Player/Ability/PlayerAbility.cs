using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] AbilityLoadout _abilityLoadout;
    [SerializeField] Ability _mainAbility;

    private void Awake()
    {
        if(_mainAbility != null)
        {
            _abilityLoadout?.EquipAbility(_mainAbility);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _abilityLoadout.UseAbility();
        }
    }
}
