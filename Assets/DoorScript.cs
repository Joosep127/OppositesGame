using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    bool isOpen = false;

    public bool P1;
    public bool P2;

    private bool hasP1 = false;
    private bool hasP2 = false;

    public ButtonScript[] buttons;

    void Update()
    {
        if (!isOpen && AreAllButtonsActive())
        {
            isOpen = true;
        }
    }
    
    private bool AreAllButtonsActive()  
    {
        foreach (ButtonScript button in buttons)
        {
            if (!button.active)
            {
                return false; 
            }
        }
        return true;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)    
    {
        if ((collision.gameObject.CompareTag("P1") || !P1)) {
            hasP1 = true;
        } else if ((collision.gameObject.CompareTag("P2") || !P2)) {
            hasP2 = true;
        }

        CheckDoorCondition(collision);
    }
    
    private void OnTriggerExit2D(Collider2D collision)    
    {
        if ((collision.gameObject.CompareTag("P1") || !P1)) {
            hasP1 = false;
        } else if ((collision.gameObject.CompareTag("P2") || !P2)) {
            hasP2 = false;
        }
    }

    private void CheckDoorCondition(Collider2D collision)
    {
        if (hasP1 && hasP2) 
        {
            ActivateDoor();
        }
    }

    private void ActivateDoor ()
    {
        NextLevel();
    }

    private void NextLevel ()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
