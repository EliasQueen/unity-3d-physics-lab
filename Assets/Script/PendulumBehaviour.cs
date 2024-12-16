using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PendulumBehaviour : InteractionBehaviourBase
{
    [Header("Objetos")]
    public Rigidbody pendulum;
    public HingeJoint pendulumHingeJoint;
    public LineRendererHUD trajectoryGraph2D;
    private List<Vector3> trajectoryPoints = new List<Vector3>();

    [Header("Parâmetros Gerais")]
    public float speed = 1.0f;
    public bool applyAirResistance = false;
    private float startTime;

    [Header("Medições")]
    public TextMeshProUGUI Amplitude;
    public TextMeshProUGUI Period;
    public TextMeshProUGUI Frequency;
    // public TextMeshProUGUI Angle;

    private float maxHeight = 0f;
    private float averagePeriod = 0f;
    private float frequency = 0f;
    private float lastCrossingTime = 0f;
    private float lastPosition = 0f;
    private float maxAngle = 0f;
    private Queue<float> recentPeriods = new Queue<float>();
    private const int MAX_PERIOD_SAMPLES = 5;
    private float pendulumLength;
    
    void Start()
    {
        this.speed = Math.Clamp(PlayerPrefs.HasKey("speed") ? int.Parse(PlayerPrefs.GetString("speed")) : 10, 1, 15);
        this.applyAirResistance = PlayerPrefs.HasKey("air_resistance") ? bool.Parse(PlayerPrefs.GetString("air_resistance")) : false;

        pendulumLength = Vector3.Distance(pendulumHingeJoint.transform.position, pendulum.transform.position);

        PropertiesHandler();
    }

    public override void Interagir()
    {
        pendulum.angularVelocity = new Vector3(0, 0, speed);
        startTime = Time.fixedTime;
        
        // Reset measurements
        maxHeight = 0f;
        maxAngle = 0f;
        recentPeriods.Clear();
        trajectoryPoints.Clear();
        lastCrossingTime = Time.fixedTime;
        lastPosition = 0f;
        averagePeriod = 0f;
        frequency = 0f;
        
        StopAllCoroutines();
        StartCoroutine(TrackPendulumTrajectory());
    }

    void PropertiesHandler()
    {
        this.pendulumHingeJoint.useSpring = this.applyAirResistance;
    }

    float CalculateAngle()
    {
        // Get the current joint angle from the HingeJoint
        float angle = pendulumHingeJoint.angle;
        
        // Return absolute value since we just want the magnitude of the angle
        return Mathf.Abs(angle);
    }

    void UpdatePeriodAndFrequency()
    {
        float currentPosition = pendulum.transform.position.x;
        
        // Update maximum angle using the hinge joint angle
        float currentAngle = CalculateAngle();
        if(currentAngle > maxAngle)
        {
            maxAngle = currentAngle;
        }
        
        // Detecta quando o pêndulo cruza o ponto zero com velocidade positiva
        if (lastPosition <= 0 && currentPosition > 0)
        {
            if (lastCrossingTime > 0)
            {
                float currentPeriod = Time.fixedTime - lastCrossingTime;
                
                if (currentPeriod > 0.1f && currentPeriod < 10f)
                {
                    recentPeriods.Enqueue(currentPeriod);
                    if (recentPeriods.Count > MAX_PERIOD_SAMPLES)
                    {
                        recentPeriods.Dequeue();
                    }

                    float sum = 0f;
                    foreach (float period in recentPeriods)
                    {
                        sum += period;
                    }
                    averagePeriod = sum / recentPeriods.Count;
                    frequency = 1f / averagePeriod;
                }
            }
            
            lastCrossingTime = Time.fixedTime;
        }
        
        lastPosition = currentPosition;
    }

    void FixedUpdate()
    {
        UpdatePeriodAndFrequency();
        
        // Update UI with measurements
        Amplitude.text = $"{Math.Round(maxHeight * 100, 1):F1} cm";
        Period.text = $"{averagePeriod:F2} s";
        Frequency.text = $"{frequency:F2} Hz";
        // Angle.text = $"Ângulo Máximo: {maxAngle:F1}°";
    }


    private System.Collections.IEnumerator TrackPendulumTrajectory()
    {
        float elapsedTime;
        
        while (true)
        {
            elapsedTime = Time.fixedTime - startTime;
            
            if(Mathf.Abs(pendulum.transform.position.x) > maxHeight)
            {
                maxHeight = Mathf.Abs(pendulum.transform.position.x);
            }
            
            float scaledPositionX = pendulum.transform.position.x * 12;
            
            if(elapsedTime < 17)
            {
                trajectoryPoints.Add(new Vector3(elapsedTime, scaledPositionX, 0));
                trajectoryGraph2D.UpdateTrajectory(trajectoryPoints);
            }
            
            yield return new WaitForSeconds(0.01f);
        }
    }
}
