using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public GameObject timerText;
    public bool stop = false;


    private void Update()
    {
        if (!stop)
        {
            float t = Time.time;
            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f2");

            timerText.GetComponent<Text>().text = minutes + ":" + seconds;
        }
    }
}
