using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public static TimerScript instance;
    public Image timerBar;
    public GameObject timerBarGO;
    public float maxTime = 5f;
    float timeLeft;
    bool timerIsRunning;
    public static bool timeElapsed;

    private void Awake()
    {
        instance = this;
    }
    public void LaunchTimer()
    {

        timerIsRunning = true;
        timerBar = GetComponent<Image>();
        timerBarGO.SetActive(true);
        timeLeft = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerIsRunning)
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                timerBar.fillAmount = timeLeft / maxTime;
            }
            else
            {
                timeElapsed = true;
                timerBar.gameObject.SetActive(false);
            }
        }
    }
}
