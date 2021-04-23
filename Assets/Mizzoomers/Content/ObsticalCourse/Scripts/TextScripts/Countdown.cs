using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    public GameObject timerText;
    private int secondsLeft = 30;
    public bool countingDown = false;

    private void Start()
    {
        timerText.GetComponent<Text>().text = "00:" + secondsLeft;
    }

    private void Update()
    {
        if(countingDown == false && secondsLeft > 0)
        {
            StartCoroutine(TimerTake());
        }
    }

    IEnumerator TimerTake()
    {
        countingDown = true;
        yield return new WaitForSeconds(1);
        secondsLeft -= 1;
        if (secondsLeft >= 10) timerText.GetComponent<Text>().text = "00:" + secondsLeft;
        else timerText.GetComponent<Text>().text = "00:0" + secondsLeft;
        countingDown = false;
    }
}
