using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text text;
    bool gameHasEnded = false;

    [SerializeField]
    float restartDelay = 2f;

    public void EndGame (bool won)
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            if (won)
            {
                text.color = Color.green;
                text.text = "Good job! :)";
            } else
            {
                text.color = Color.red;
                text.text = "Game over, try again";
            }
            Debug.Log("Game over!");
            Invoke("Restart", restartDelay);
        }
    }

    void Restart ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
