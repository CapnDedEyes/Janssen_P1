using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] ThirdPersonMovement _thirdPersonMovement;

    [SerializeField] int _maxHealth = 3;
    [SerializeField] Slider _slider;
    [SerializeField] GameObject healthBar;
    int _currentHealth;

    private void Awake()
    {
        _thirdPersonMovement = gameObject.GetComponent<ThirdPersonMovement>();
    }

    private void Start()
    {
        _slider.maxValue = _maxHealth;
        _slider.value = _currentHealth;
        _currentHealth = _maxHealth;
    }

    public void Update()
    {
        _slider.value = _currentHealth;
    }

    public void DecreaseHealth (int amount)
    {
        if(_currentHealth > 0)
        {
            _currentHealth -= amount;
            //Debug.Log("Player's health: " + _currentHealth);
            _thirdPersonMovement.CheckIfDamaged();
        }
        
        if(_currentHealth <= 0)
        {
            Kill();
        }
    }

    public void Kill ()
    {
        healthBar.SetActive(false);
        _thirdPersonMovement.CheckIfDead();
    }
}
