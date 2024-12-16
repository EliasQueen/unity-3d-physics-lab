using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CannonBehaviour : InteractionBehaviourBase
{
    [Header("Objetos")]
    public GameObject Projectile;
    public TrajectoryGraph3D trajectoryGraph3D;  // Para a linha 3D
    public LineRendererHUD trajectoryGraph2D;
    public Transform cannonBase;
    
    [Header("Propriedades")]
    public float ProjectileSpeed;
    public float ShootAngle;
    public float Height;

    [Header("Height Tracking")]
    public TextMeshProUGUI alturaMaxima;
    public TextMeshProUGUI distanciaNoAr;
    public TextMeshProUGUI tempoDeVoo;
    // private float theoreticalMaxHeight;
    private float actualMaxHeight;
    private float heightAtGround = 0.23f;
    private float distanceFlown;
    private float timeFlown;
    private float startTime;
    private GameObject lastCannonball;

    [Header("Graph")]
    public UIGridRenderer grafico;
    public LineRendererHUD trajetoria;
    private List<Vector3> trajectoryPoints2D = new List<Vector3>();
    private List<Vector3> trajectoryPoints3D = new List<Vector3>();

    void Start()
    {
        PropertiesHandler();
    }

    void PropertiesHandler()
    {
        ProjectileSpeed = Mathf.Clamp(PlayerPrefs.HasKey("projectile_speed") ? float.Parse(PlayerPrefs.GetString("projectile_speed")) : 10.0f, 1.0f, 10.0f);
        ShootAngle = PlayerPrefs.HasKey("shoot_angle") ? float.Parse(PlayerPrefs.GetString("shoot_angle")) : 45.0f;
        Height = Mathf.Clamp(PlayerPrefs.HasKey("initial_height") ? float.Parse(PlayerPrefs.GetString("initial_height")) : 1.0f, 1.0f, 5.0f);

        ShootAngle = Mathf.Clamp(ShootAngle % 360, 25, 155);

        transform.rotation = Quaternion.Euler(0, 0, -90 + ShootAngle);
        transform.position = new Vector3(0, Height, 0);

        Vector3 basePos = cannonBase.transform.position;
        Vector3 baseSca = cannonBase.transform.localScale;

        cannonBase.transform.position = new Vector3(basePos.x, (Height - 0.5f) / 2, basePos.z);
        cannonBase.transform.localScale = new Vector3(baseSca.x, Height - 0.5f, baseSca.z);
    }

    public override void Interagir()
    {
        startTime = Time.fixedTime;

        // Destruir a última bala de canhão antes de atirar uma nova
        if (lastCannonball != null)
        {
            Destroy(lastCannonball);
            trajectoryPoints2D.Clear();
            trajectoryPoints3D.Clear();
            StopAllCoroutines();
        }

        float radAngle = Mathf.Deg2Rad * ShootAngle; // Convertendo o ângulo de graus para radianos
        
        // Calculate velocity components directly without normalization
        float vx = ProjectileSpeed * Mathf.Cos(radAngle);
        float vy = ProjectileSpeed * Mathf.Sin(radAngle);

        // theoreticalMaxHeight = Height + vy * vy / (2 * Physics.gravity.magnitude);
        actualMaxHeight = Height;
        
        GameObject cannonball = Instantiate(Projectile, transform.position, Quaternion.identity);
        Rigidbody cannonballRB = cannonball.GetComponent<Rigidbody>();
        cannonballRB.velocity = new Vector2(vx, vy);

        // Debug.Log($"Theoretical max height: {theoreticalMaxHeight:F3} m");

        lastCannonball = cannonball;
        StartCoroutine(TrackProjectileTrajectory(cannonball));

        UpdateDisplay();
    }

    private System.Collections.IEnumerator TrackProjectileTrajectory(GameObject projectile)
    {
        while (projectile != null)
        {
            float currentDistance = Mathf.Abs(projectile.transform.position.x);
            float currentHeight = projectile.transform.position.y;

            if (currentHeight > actualMaxHeight)
            {
                actualMaxHeight = currentHeight;
            }

            if (currentHeight > heightAtGround)
            {
                distanceFlown = currentDistance;
                timeFlown = Time.fixedTime - startTime;
            }

            trajectoryPoints3D.Add(projectile.transform.position);
            trajectoryGraph3D.UpdateTrajectory(trajectoryPoints3D);  // Atualiza o gráfico 3D

            Vector3 currentPos2D = new Vector3(currentDistance, currentHeight, 0);

            if(-1 < currentPos2D.x && currentPos2D.x < 17)
            {
                trajectoryPoints2D.Add(currentPos2D);
                trajectoryGraph2D.UpdateTrajectory(trajectoryPoints2D);

                UpdateDisplay();
            }
            yield return new WaitForSeconds(0.01f);  // Ajuste conforme necessário
        }
    }

    private void UpdateDisplay()
    {
        if (alturaMaxima != null)
        {
            string maxHeight = $"{actualMaxHeight:F3} m";
            string airDist = $"{distanceFlown:F3} m";
            string airTime = $"{timeFlown:F3} s";
            
            alturaMaxima.text = maxHeight;
            distanciaNoAr.text = airDist;
            tempoDeVoo.text = airTime;
        }
    }
}
