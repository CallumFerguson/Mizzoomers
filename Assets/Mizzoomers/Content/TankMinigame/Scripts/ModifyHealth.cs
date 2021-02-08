using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ModifyHealth : MonoBehaviour
{
	public Text HealthText;
	private int health;
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void ChangeHealth(int change)
	{
		health = health + change;
		HealthText.text = "HP: " + health.ToString();
	}
}
