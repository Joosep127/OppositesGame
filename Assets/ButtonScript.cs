using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    public bool P1;
    public bool P2;
    public bool active = false;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.gameObject.CompareTag("P1") && P1) || (collision.gameObject.CompareTag("P2") && P2)) {
            active = true;
            gameObject.SetActive(false);
        }
    }
}

