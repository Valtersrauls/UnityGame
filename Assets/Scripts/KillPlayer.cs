using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillPlayer : MonoBehaviour
{
    public Text deathText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy") {
            Destroy(gameObject);
            deathText.text = "You died!";
        }
    }
}
