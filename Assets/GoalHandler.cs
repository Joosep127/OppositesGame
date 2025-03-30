using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalHandler : MonoBehaviour
{
    public GameObject d1;
    public GameObject d2;

    void Update()
    {
        if (d1.GetComponent<GoalDoorScript>().GoalState && d2.GetComponent<GoalDoorScript>().GoalState)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
