using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Cubo1 : MonoBehaviour
{
    [Header("Propriedades do Cubo")]
    public Vector3 fLastVel;
    public Vector3 fLastPos;
    public Boolean IsOOB = false;

    [Header("Outro Cubo")]
    public GameObject Cubo2;
    private Vector3 originalPosition;
    private new Rigidbody rigidbody;
    private new Transform transform;

    [Header("Contador")]
    public TextMeshProUGUI Contador;
    public TextMeshProUGUI PI;

    [Header("Propriedades")]
    public int Precision;

    public int collisionCounter = 0;

    void Start()
    {
        PropertiesHandler();
    }

    void PropertiesHandler()
    {
        Precision = PlayerPrefs.HasKey("decimal_places") ? int.Parse(PlayerPrefs.GetString("decimal_places")) : 1;

        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        originalPosition = transform.position;

        float escala = 0.1f;

        transform.localScale = new Vector3(escala, escala, escala);
        rigidbody.mass = 1f;

        originalPosition.y = 1 + (escala / 2);
        transform.position = originalPosition;
    }

    // void StartExperiment()
    // {
    //     transform.position = originalPosition;
    //     if (cubeNumber == 2 && outroCubo != null)
    //     {
    //         VelocidadeAtual = velocidadeInicial;
    //         // Vector3 direction = outroCubo.GetComponent<Transform>().position - transform.position; // Calcula o vetor direção entre as posições
    //         // this.rigidbody.AddForce(direction.normalized * velocidadeInicial, ForceMode.VelocityChange); // Aplica uma força na direção calculada
    //     }
    //     else
    //     {
    //         // Debug.LogError("Cubo1 não atribuído no Inspector!"); // Se Cubo1 não estiver atribuído, mostra um erro no console
    //     }
    // }

    private static Vector3 CalcularNovaVelocidade(float mAtual, Vector3 vAtual, float mOutro, Vector3 vOutro)
    {
        float somaMassas = mAtual + mOutro;
        float novaVelocidade = ((mAtual - mOutro) / somaMassas * vAtual.x) + 2 * mOutro / somaMassas * vOutro.x;

        return new Vector3(novaVelocidade, 0, 0);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Parede 2"))
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, 500f, pos.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Debug.Log(collision.GetContact(0).normal);
        if (collision.gameObject == Cubo2)
        {
            Rigidbody Atual = rigidbody;
            Rigidbody Outro = collision.gameObject.GetComponent<Rigidbody>();

            rigidbody.velocity = CalcularNovaVelocidade(Atual.mass, fLastVel, Outro.mass, Outro.GetComponent<Cubo2>().fLastVel);
            Outro.velocity = CalcularNovaVelocidade(Outro.mass, Outro.GetComponent<Cubo2>().fLastVel, Atual.mass, fLastVel);

            gameObject.transform.position = fLastPos;
            Cubo2.transform.position = Cubo2.GetComponent<Cubo2>().fLastPos;
        }

        if (collision.gameObject.CompareTag("Parede 1"))
        {
            rigidbody.velocity = new Vector3(-fLastVel.x, 0, 0);
            
            float X = fLastPos.x;
            float Y = gameObject.transform.position.y;
            float Z = gameObject.transform.position.z;

            gameObject.transform.position = new Vector3(X, Y, Z);
        }

        collisionCounter++;
    }

    public void StartExperiment()
    {
        transform.position = originalPosition;
        rigidbody.velocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        fLastVel = rigidbody.velocity;
        fLastPos = gameObject.transform.position;
        Contador.text = collisionCounter.ToString();
        PI.text = collisionCounter == 0 ? "0" : (collisionCounter / Math.Pow(10, (Precision - 1))).ToString();
    }
}
