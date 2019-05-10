using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public Text finishText;

    private void Start()
    {
        finishText.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            finishText.text = "Level completed!";
        }
    }
}