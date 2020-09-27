using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    [SerializeField] AbilityLoadout _abilityLoadout;
    [SerializeField] Ability _mainAbility;
    [SerializeField] Ability _subAbility;

    private int timer = 0;

    private void Awake()
    {
        if(_mainAbility != null)
        {
            _abilityLoadout?.EquipAbility(_mainAbility);
        }
    }

    public void UseMain()
    {
        _abilityLoadout?.EquipAbility(_mainAbility);
        _abilityLoadout.UseAbility();
    }

    public void UseSub()
    {
        _abilityLoadout?.EquipAbility(_subAbility);
        _abilityLoadout.UseAbility();
    }
}
