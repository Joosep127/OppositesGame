using UnityEngine;

public class GoalDoorScript : MonoBehaviour
{

    public bool GoalState = false;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        GoalState = true;
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
        GoalState = false;
    }
}
