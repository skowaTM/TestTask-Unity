using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthbar;
    [SerializeField, Range(0.1f, 2f)] private float reduceSpeed = 1f;

    private float healthTargetVal = 1f;


    public void UpdateHealthBar(float curHP, float maxHP)
    {
        healthTargetVal = Mathf.Clamp(curHP / maxHP, 0f, 1f);
    }


    private void Update()
    {
        healthbar.fillAmount = Mathf.MoveTowards(healthbar.fillAmount, healthTargetVal, reduceSpeed * Time.deltaTime);
    }
}
