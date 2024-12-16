using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FreeFallBehaviour : InteractionBehaviourBase
{
    [Header("Objetos")]
    public Rigidbody Bola1;
    public Rigidbody Bola2;
    public LineRendererHUD trajectoryGraph1;
    public LineRendererHUD trajectoryGraph2;

    private Vector3 bola1StartPosition;
    private Vector3 bola2StartPosition;

    private float startTime;
    private List<Vector3> trajectoryPoints1 = new List<Vector3>();
    private List<Vector3> trajectoryPoints2 = new List<Vector3>();

    [Header("Parâmetros Gerais")]
    public bool applyAirResistance = false;
    private float groundHeight = 1.25f;
    private float maxFallTime;
    private float startingHeight;
    private List<float> totalFlightTime = new List<float>{0f, 0f};
    private bool isRunning = false;

    [Header("Legendas do Gráfico")]
    public List<TextMeshProUGUI> X;
    public List<TextMeshProUGUI> Y;
    public List<TextMeshProUGUI> simulated_mass;
    public List<TextMeshProUGUI> flight_duration;
    public List<TextMeshProUGUI> velocity;

    void Start()
    {
        InitializeBalls();
        CalculateSimulationParameters();
    }

    private void InitializeBalls()
    {
        // Initialize ball 1
        Bola1.mass = GetClampedMass("object_1_mass", 10f);
        bola1StartPosition = Bola1.transform.position;
        SetupAirResistance(Bola1);

        // Initialize ball 2
        Bola2.mass = GetClampedMass("object_2_mass", 50f);
        bola2StartPosition = Bola2.transform.position;
        SetupAirResistance(Bola2);
    }

    private void SetTexts(List<TextMeshProUGUI> textList, List<float> values, string uom)
    {
        textList[0].text = $"{values[0]:F2} {uom}";
        textList[1].text = $"{values[1]:F2} {uom}";
    }

    private float GetClampedMass(string prefKey, float defaultValue)
    {
        float mass = PlayerPrefs.HasKey(prefKey) 
            ? float.Parse(PlayerPrefs.GetString(prefKey)) 
            : defaultValue;
        return Mathf.Clamp(mass, 1f, 100f) / 1000f;
    }

    private void SetupAirResistance(Rigidbody bola)
    {
        var resistance = bola.gameObject.AddComponent<AirResistance>();
        resistance.enabled = applyAirResistance;
    }

    private void CalculateSimulationParameters()
    {
        float timeToFallBall1 = SimulateFallTime(Bola1);
        float timeToFallBall2 = SimulateFallTime(Bola2);
        
        maxFallTime = Mathf.Max(timeToFallBall1, timeToFallBall2);
        startingHeight = Mathf.Max(bola1StartPosition.y, bola2StartPosition.y) - groundHeight;
    }

    public override void Interagir()
    {
        isRunning = true;
        ResetBalls();
        StartSimulation();
    }

    private void ResetBalls()
    {
        // Reset positions
        Bola1.transform.position = bola1StartPosition;
        Bola2.transform.position = bola2StartPosition;

        // Reset physics
        Bola1.velocity = Bola2.velocity = Vector3.zero;
        Bola1.useGravity = Bola2.useGravity = true;

        // Reset air resistance
        Bola1.GetComponent<AirResistance>().enabled = applyAirResistance;
        Bola2.GetComponent<AirResistance>().enabled = applyAirResistance;
    }

    private void StartSimulation()
    {
        startTime = Time.fixedTime;
        trajectoryPoints1.Clear();
        trajectoryPoints2.Clear();
        
        StopAllCoroutines();
        UpdateGraphicSubtitles();
        StartCoroutine(TrackFallTrajectory());
    }

    private float SimulateFallTime(Rigidbody bola)
    {
        Vector3 position = bola.position;
        Vector3 velocity = Vector3.zero;
        float simulatedTime = 0f;
        
        var airResistance = bola.GetComponent<AirResistance>();
        airResistance.CalculateArea();

        while (position.y > groundHeight)
        {
            Vector3 netForce = CalculateNetForce(bola, velocity, airResistance);
            Vector3 acceleration = netForce / bola.mass;

            velocity += acceleration * Time.fixedDeltaTime;
            position += velocity * Time.fixedDeltaTime;
            simulatedTime += Time.fixedDeltaTime;
        }

        return simulatedTime;
    }

    private Vector3 CalculateNetForce(Rigidbody ball, Vector3 velocity, AirResistance airResistance)
    {
        Vector3 gravityForce = ball.mass * Physics.gravity;
        
        if (!applyAirResistance) return gravityForce;

        float speed = velocity.magnitude;
        Vector3 dragForce = -0.5f * airResistance.airDensity * speed * speed * 
                           airResistance.dragCoefficient * airResistance.objectArea * 
                           velocity.normalized;
        
        return gravityForce + dragForce;
    }

    private void UpdateGraphicSubtitles()
    {
        float lastTime = Mathf.Ceil(maxFallTime);
        float maxHeight = Mathf.Ceil(startingHeight);

        float incrementX = lastTime / X.Count;
        float currentValueX = incrementX;

        float incrementY = maxHeight / Y.Count;
        float currentValueY = incrementY;

        for(int i = 0; i < X.Count; i++)
        {
            X[i].text = Math.Round(currentValueX, 1).ToString("F1");
            currentValueX += incrementX;
        }

        for(int i = 0; i < Y.Count; i++)
        {
            Y[i].text = Math.Round(currentValueY, 1).ToString("F1");
            currentValueY += incrementY;
        }
    }

    private System.Collections.IEnumerator TrackFallTrajectory()
    {
        float elapsedTime;
        float scaledElapsedTime;
        float[] lastYMetric = {float.MaxValue, float.MaxValue};

        while (true) // Continuar até que o pêndulo seja interrompido ou a interação termine
        {
            // Calcular o tempo decorrido
            float lastTime = Mathf.Ceil(maxFallTime);
            float maxHeight = Mathf.Ceil(startingHeight);
            
            float scaleX = 15 / lastTime;
            float scaleY = 7.5f / maxHeight;

            elapsedTime = Time.fixedTime - startTime;
            scaledElapsedTime = (elapsedTime) * scaleX;

            float scaledPositionY1 = (Bola1.transform.position.y - 1.25f) * scaleY;
            float scaledPositionY2 = (Bola2.transform.position.y - 1.25f) * scaleY;
            
            if(scaledElapsedTime < 17)
            {
                if(scaledPositionY1 != lastYMetric[0])
                {
                    totalFlightTime[0] = elapsedTime;
                    trajectoryPoints1.Add(new Vector3(scaledElapsedTime, scaledPositionY1, 0)); // Posição relativa em X
                    trajectoryGraph1.UpdateTrajectory(trajectoryPoints1);
                }

                if(scaledPositionY2 != lastYMetric[1])
                {
                    totalFlightTime[1] = elapsedTime;
                    trajectoryPoints2.Add(new Vector3(scaledElapsedTime, scaledPositionY2, 0)); // Posição relativa em X
                    trajectoryGraph2.UpdateTrajectory(trajectoryPoints2);
                }
            }

            if (scaledPositionY1 == lastYMetric[0] && scaledPositionY1 == lastYMetric[1])
            {
                isRunning = false;
                break;
            }

            lastYMetric[0] = scaledPositionY1;
            lastYMetric[1] = scaledPositionY2;

            yield return new WaitForSeconds(0.01f); // Ajuste a frequência de atualização conforme necessário
        }
    }

    void FixedUpdate()
    {
        if (isRunning)
        {
            SetTexts(simulated_mass, new List<float>{Bola1.mass * 1000, Bola2.mass * 1000}, "g");
            SetTexts(flight_duration, new List<float>{totalFlightTime[0], totalFlightTime[1]}, "s");
            SetTexts(velocity, new List<float>{-Bola1.velocity.y, -Bola2.velocity.y}, "m/s");
        }
    }
}
