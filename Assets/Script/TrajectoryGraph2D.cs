using System.Collections.Generic;
using UnityEngine;

public class TrajectoryGraph2D : MonoBehaviour
{
    public LineRenderer lineRenderer;  // O LineRenderer para desenhar a trajetória
    public Camera playerCamera;         // A câmera do jogador

    void Start()
    {
        // Configura o LineRenderer para usar o espaço da tela
        lineRenderer.useWorldSpace = false; // Agora usa espaço da tela
    }

    // Método para atualizar a trajetória com uma nova lista de pontos
    public void UpdateTrajectory(List<Vector3> points)
    {
        // Atualiza os pontos no LineRenderer
        lineRenderer.positionCount = points.Count;

        Vector3[] screenPoints = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            // Converte a posição do mundo para a posição da tela
            Vector3 screenPoint = playerCamera.WorldToViewportPoint(points[i]);
            // Verifique se o ponto está dentro dos limites da tela (opcional)
            if (screenPoint.z >= 0)
            {
                screenPoints[i] = new Vector3(screenPoint.x * Screen.width, screenPoint.y * Screen.height, 0);
            }
        }

        lineRenderer.SetPositions(screenPoints);
    }
}
