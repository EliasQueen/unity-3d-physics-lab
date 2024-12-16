using System.Collections.Generic;
using UnityEngine;

public class TrajectoryGraph3D : MonoBehaviour
{
    public LineRenderer lineRenderer;  // O LineRenderer para desenhar a trajetória

    void Start()
    {
        // Certifique-se de que o LineRenderer usa o espaço mundial
        lineRenderer.useWorldSpace = true;
    }

    // Método para atualizar a trajetória com uma nova lista de pontos
    public void UpdateTrajectory(List<Vector3> points)
    {
        // Atualiza os pontos no LineRenderer
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
