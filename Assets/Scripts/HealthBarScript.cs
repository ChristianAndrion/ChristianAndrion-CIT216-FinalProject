using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class HealthBarScript : MonoBehaviour
{
    public RectTransform backgroundBar;
    public RectTransform bar;
    public float maxBarValue;
    public float barHeight;
    public float barWidth;

    private float currentBarValue;
  
    public float BarValue
    {
        get
        {
            return currentBarValue;
        }
        set
        {
            currentBarValue -= value;
            if(currentBarValue<=0)
            {
                SceneManager.LoadScene("GameOverScene");
            }
        }
    } 

    void Start()
    {
        currentBarValue = maxBarValue;
    }

    public void UpdateBarValue(float value)
    {
        BarValue = value;
        float newWidth = (BarValue/maxBarValue) * barWidth;

        bar.sizeDelta = new Vector2(newWidth,barHeight);
        
    }
}
