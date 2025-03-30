using UnityEngine;

public class TailController : MonoBehaviour
{
    public Rigidbody2D[] tailSegments;
    public float tailForce = 5f;
    public float wiggleSpeed = 2f;

    void Update()
    {
        float wave = Mathf.Sin(Time.time * wiggleSpeed) * tailForce;
        foreach (var segment in tailSegments)
        {
            segment.AddTorque(wave);
        }
    }
}