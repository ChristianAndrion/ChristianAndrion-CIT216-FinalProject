using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class HealthBarScript : MonoBehaviour
{
    public RectTransform backgroundBar;
    public RectTransform bar;
    public float maxHealth;
    public float barHeight;
    public float barWidth;
    private float currentHealth;
  

    public float Health
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth -= value;
            if(currentHealth<=0)
            {
                //TODO - Insert Death Script
            }
        }
    } 

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void UpdateHealth(float health)
    {
        Health = health;
        float newWidth = (Health/maxHealth) * barWidth;

        bar.sizeDelta = new Vector2(newWidth,barHeight);
        
    }
}
