using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    // [SerializeField] private GameObject finishedText;
    // [SerializeField] private GameObject timerText;
    // private bool stop = false;

    private void Start()
    {
        // finishedText.GetComponent<Text>().text = "";
    }

    private void Update()
    {
        // if (!stop)
        // {
        //     float t = Time.time;
        //     string minutes = ((int)t / 60).ToString();
        //     string seconds = (t % 60).ToString("f2");
        //
        //     timerText.GetComponent<Text>().text = minutes + ":" + seconds;
        // }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other);
        // if (other.tag == "Finish")
        // {
        //     stop = true;
        //     finishedText.GetComponent<Text>().text = "Finished!";
        // }
    }
}
