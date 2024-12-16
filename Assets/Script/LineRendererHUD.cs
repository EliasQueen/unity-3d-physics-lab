using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class LineRendererHUD : Graphic
{
    public Vector2Int gridSize; // Tamanho da grade
    public float thickness; // Espessura da linha
    public Vector2 originOffset; // Offset para o ponto de origem (0,0)

    public List<Vector2> points; // Pontos a serem desenhados

    float width;
    float height;
    float unitWidth;
    float unitHeight;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        unitWidth = width / gridSize.x;
        unitHeight = height / gridSize.y;

        // Ajustar os pontos com o offset do ponto de origem
        List<Vector2> adjustedPoints = new List<Vector2>();
        foreach (var point in points)
        {
            adjustedPoints.Add(new Vector2(point.x + originOffset.x, point.y + originOffset.y));
        }

        if (adjustedPoints.Count < 2) return;

        float angle = 0;
        for (int i = 0; i < adjustedPoints.Count - 1; i++)
        {
            Vector2 point = adjustedPoints[i];
            Vector2 point2 = adjustedPoints[i + 1];

            if (i < adjustedPoints.Count - 1)
            {
                angle = GetAngle(adjustedPoints[i], adjustedPoints[i + 1]) + 90f;
            }

            DrawVerticesForPoint(point, point2, angle, vh);
        }

        for (int i = 0; i < adjustedPoints.Count - 1; i++)
        {
            int index = i * 4;
            vh.AddTriangle(index + 0, index + 1, index + 2);
            vh.AddTriangle(index + 1, index + 2, index + 3);
        }
    }

    public float GetAngle(Vector2 me, Vector2 target)
    {
        // Tamanho do painel em proporção
        return (float)(Mathf.Atan2(9f * (target.y - me.y), 16f * (target.x - me.x)) * Mathf.Rad2Deg);
    }

    void DrawVerticesForPoint(Vector2 point, Vector2 point2, float angle, VertexHelper vh)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        // Calcular posição da linha
        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
        vertex.position += new Vector3(unitWidth * point2.x, unitHeight * point2.y);
        vh.AddVert(vertex);
    }

    public void UpdateTrajectory(List<Vector3> points3D)
    {
        List<Vector2> points2D = new List<Vector2>();

        foreach (var vector3 in points3D)
        {
            points2D.Add(new Vector2(vector3.x, vector3.y)); // Converte para Vector2
        }

        this.points = points2D;

        SetVerticesDirty();
    }
}
