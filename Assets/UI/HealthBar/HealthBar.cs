using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] DamageableCharacter target;
    public Slider healthSlider;

    void Start()
    {
        healthSlider.maxValue = target.MaxHealth;
        healthSlider.value = target.Health;

        target.OnHealthChanged += OnHealthChanged;
    }
    
    public void OnHealthChanged(int health, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = health;
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}
