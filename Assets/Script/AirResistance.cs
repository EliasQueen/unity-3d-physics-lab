using UnityEngine;

public class AirResistance : MonoBehaviour
{
    public float dragCoefficient = 0.47f; // coeficiente de resistência (para uma esfera)
    public float airDensity = 1.225f; // densidade do ar (em kg/m³)
    public float objectArea; // área de seção transversal (em m²)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SphereCollider sphereCollider = GetComponent<SphereCollider>();

        if (sphereCollider != null)
        {
            CalculateArea();
        }
    }

    public void CalculateArea()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        
        if (sphereCollider != null)
        {
            float radius = Mathf.Abs(sphereCollider.radius * transform.localScale.x);
            objectArea = Mathf.PI * radius * radius;
        }
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.velocity;
        float speed = velocity.magnitude;
        Vector3 dragForce = -0.5f * airDensity * speed * speed * dragCoefficient * objectArea * velocity.normalized;
        rb.AddForce(dragForce);
    }

    void OnDrawGizmos()
    {
        if (rb != null && rb.velocity.magnitude > 0.01f)
        {
            Gizmos.color = Color.red;
            Vector3 dragForce = -0.5f * airDensity * rb.velocity.magnitude * rb.velocity.magnitude * 
                            dragCoefficient * objectArea * rb.velocity.normalized;
            Gizmos.DrawLine(transform.position, transform.position + dragForce.normalized);
        }
    }
}
