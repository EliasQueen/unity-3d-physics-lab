using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UIElements;

public class Cubo2 : MonoBehaviour
{
    [Header("Propriedades do Cubo")]
    public int Casas = 1;
    private float initialVelocity;
    public Vector3 fLastVel;
    public Vector3 fLastPos;

    [Header("Outro Cubo")]
    public GameObject Cubo1;
    private Vector3 originalPosition;
    private new Rigidbody rigidbody;
    private new Transform transform;

    void Start() {
        Casas = PlayerPrefs.HasKey("decimal_places") ? int.Parse(PlayerPrefs.GetString("decimal_places")) : 1;

        PropertiesHandler();
    }

    void PropertiesHandler() {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        originalPosition = transform.position;

        Casas = Math.Clamp(Casas, 1, 3);
        initialVelocity = 1.5f / Casas;
        
        float escala = 0.1f * Casas;

        transform.localScale = new Vector3(escala, escala, escala);
        rigidbody.mass = (float) Math.Pow(100, Casas - 1);

        originalPosition.y = 1 + (escala / 2);
        transform.position = originalPosition;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Parede 2"))
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, 500f, pos.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Parede 1"))
        {
            rigidbody.velocity = new Vector3(-fLastVel.x, 0, 0);
            
            float X = fLastPos.x;
            float Y = gameObject.transform.position.y;
            float Z = gameObject.transform.position.z;

            gameObject.transform.position = new Vector3(X, Y, Z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Verifica se a tecla "P" foi pressionada
        {
            StartExperiment(); // Chama o método StartExperiment() quando a tecla "S" é pressionada
        }
    }

    void FixedUpdate()
    {
        fLastVel = rigidbody.velocity;
        fLastPos = gameObject.transform.position;
    }

    public void StartExperiment()
    {
        transform.position = originalPosition;
        rigidbody.velocity = Vector3.left * initialVelocity;
    }
}
