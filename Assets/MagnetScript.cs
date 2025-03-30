using UnityEngine;
using System.Collections.Generic;

public class MagnetScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public int Polarity;
    private List<LineRenderer> listRenderers = new List<LineRenderer>();


    void Start()
    {
        if (Polarity > 0)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.blue;
            CreateLineRenderers(Color.blue);
        }
        else if (Polarity < 0)
        {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            CreateLineRenderers(Color.red);
        }
        else
        {
            Debug.LogError("You forgor to enable magnet.. 🥺");
        }


    }

    void CreateLineRenderers(Color color)
    {
        listRenderers.Clear(); // Clear the list to avoid duplicates

        // Create a red line child
        GameObject redLineObj = new GameObject("RedLine");
        redLineObj.transform.parent = transform; // Attach to parent
        LineRenderer redLine = redLineObj.AddComponent<LineRenderer>();
        redLine.material = new Material(Shader.Find("Sprites/Default"));
        redLine.startColor = color;
        redLine.endColor = color;
        redLine.startWidth = 0.2f;
        redLine.endWidth = 0.2f;

        // Create a blue line child
        GameObject blueLineObj = new GameObject("BlueLine");
        blueLineObj.transform.parent = transform;
        LineRenderer blueLine = blueLineObj.AddComponent<LineRenderer>();
        blueLine.material = new Material(Shader.Find("Sprites/Default"));
        blueLine.startColor = color;
        blueLine.endColor = color;
        blueLine.startWidth = 0.2f;
        blueLine.endWidth = 0.2f;

        // Add to the list
        listRenderers.Add(redLine);
        listRenderers.Add(blueLine);

        // Disable both LineRenderers initially
        listRenderers[0].enabled = false;
        listRenderers[1].enabled = false;
    }

    public void OnTriggerEnter2D(Collider2D collision) // Fixed method name
    {
        int playerIndex = (collision.gameObject.tag == "P1") ? 0 : 1;
        listRenderers[playerIndex].gameObject.SetActive(true);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        int playerIndex = (collision.gameObject.tag == "P1") ? 0 : 1;
        listRenderers[playerIndex].gameObject.SetActive(false);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        int playerIndex = (collision.gameObject.tag == "P1") ? 0 : 1;
        if (collision.gameObject.GetComponent<PlayerMovement>().chargeState)
        {
            DrawArrow(collision, playerIndex);
        }
        else
        {
            LineRenderer lineRenderer = listRenderers[playerIndex];
            Color arrowColor = lineRenderer.startColor;
            arrowColor.a = 0;
            lineRenderer.startColor = arrowColor;
            lineRenderer.endColor = arrowColor;
        }
    }

    private void DrawArrow(Collider2D collision, int playerIndex)
    {
        LineRenderer lineRenderer = listRenderers[playerIndex];
        Vector3 worldPos = collision.transform.position;
        float distance = Vector3.Distance(transform.position, worldPos);

        // Normalize distance to range [0,1] for color interpolation
        float t = Mathf.Clamp01(1 - (distance / 10f)); // 10f is the max distance

        // Set line positions
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, worldPos);


        Color arrowColor = lineRenderer.startColor;
        arrowColor.a = t - 0.3f;
        lineRenderer.startColor = arrowColor;
        lineRenderer.endColor = arrowColor;
    }
}
