using UnityEngine;

public class MagnetScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int Polarity;
    void Start()
    {
        if (Polarity < 0)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }
        else if (Polarity > 0)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            Debug.LogError("You forgor to enable magnet.. 🥺");
        }
    }
}
