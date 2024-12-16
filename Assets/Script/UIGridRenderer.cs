using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UIGridRenderer : MaskableGraphic
{
    public Vector2Int gridSize = new Vector2Int(1, 1);
    public float thickness = 10f;
    public Vector2 originOffset; // Changed to Vector2 to match LineRendererHUD

    private float width;
    private float height;
    private float cellWidth;
    private float cellHeight;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        cellWidth = width / gridSize.x;
        cellHeight = height / gridSize.y;

        DrawBackground(vh);

        // Draw grid lines
        for (int y = 0; y <= gridSize.y; y++)
        {
            DrawHorizontalLine(y, vh);
        }
        for (int x = 0; x <= gridSize.x; x++)
        {
            DrawVerticalLine(x, vh);
        }
        
        DrawAxes(vh);
    }

    private void DrawBackground(VertexHelper vh)
    {
        // Adiciona 4 vértices para o fundo
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = Color.white; // Cor branca para o fundo

        vertex.position = new Vector3(0, 0);
        vh.AddVert(vertex);

        vertex.position = new Vector3(0, height);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width, height);
        vh.AddVert(vertex);

        vertex.position = new Vector3(width, 0);
        vh.AddVert(vertex);

        // Adicionar os triângulos para preencher o fundo
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    private void DrawAxes(VertexHelper vh)
    {
        // Calculate the position of the axes based on originOffset
        float xAxisY = originOffset.y * cellHeight;
        float yAxisX = originOffset.x * cellWidth;

        // Draw X axis
        Vector3 startX = new Vector3(0, xAxisY);
        Vector3 endX = new Vector3(width, xAxisY);
        DrawLine(startX, endX, vh, Color.black);

        // Draw Y axis
        Vector3 startY = new Vector3(yAxisX, 0);
        Vector3 endY = new Vector3(yAxisX, height);
        DrawLine(startY, endY, vh, Color.black);
    }

    // Desenha uma linha horizontal na posição Y fornecida
    private void DrawHorizontalLine(int y, VertexHelper vh)
    {
        float yPos = y * cellHeight;

        Vector3 start = new Vector3(0, yPos);
        Vector3 end = new Vector3(width, yPos);

        DrawLine(start, end, vh);
    }

    // Desenha uma linha vertical na posição X fornecida
    private void DrawVerticalLine(int x, VertexHelper vh)
    {
        float xPos = x * cellWidth;

        Vector3 start = new Vector3(xPos, 0);
        Vector3 end = new Vector3(xPos, height);

        DrawLine(start, end, vh);
    }

    // Método que desenha uma linha usando a espessura especificada
    private void DrawLine(Vector3 start, Vector3 end, VertexHelper vh, Color? lineColor = null)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 normal = new Vector3(-direction.y, direction.x) * thickness * 0.5f; // Perpendicular para criar a espessura

        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = lineColor ?? color;

        // Vértices para criar a linha
        vertex.position = start - normal;
        vh.AddVert(vertex);

        vertex.position = start + normal;
        vh.AddVert(vertex);

        vertex.position = end + normal;
        vh.AddVert(vertex);

        vertex.position = end - normal;
        vh.AddVert(vertex);

        // Criar os triângulos para desenhar a linha
        int offset = vh.currentVertCount - 4;
        vh.AddTriangle(offset + 0, offset + 1, offset + 2);
        vh.AddTriangle(offset + 2, offset + 3, offset + 0);
    }
}
