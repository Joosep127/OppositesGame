using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    int playerCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("P1") || other.CompareTag("P2"))
        {
            playerCount++;
            CheckDoorCondition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("P1") || other.CompareTag("P2"))
        {
            playerCount--;
        }
    }

    private void CheckDoorCondition()
    {
        if (playerCount >= 2)
        {
            Debug.Log("Two players are at the door!");
            NextLevel();
        }
    }

    private void NextLevel ()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
