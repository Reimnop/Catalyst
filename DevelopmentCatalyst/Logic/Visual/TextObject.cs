using TMPro;
using UnityEngine;

namespace Catalyst.Logic.Visual;

public class TextObject : VisualObject
{
    private readonly GameObject gameObject;
    private readonly TextMeshPro textMeshPro;
    private readonly float opacity;

    private Color currentColor;

    public TextObject(GameObject gameObject, float opacity, string text)
    {
        this.gameObject = gameObject;
        this.opacity = opacity;

        currentColor = new Color(1.0f, 1.0f, 1.0f, opacity);
        
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
        textMeshPro.enabled = true;
        textMeshPro.color = currentColor;
        textMeshPro.text = text;
    }
    
    public override void SetColor(Color color)
    {
        if (currentColor == color)
        {
            return;
        }
        
        currentColor = color;
        
        // textMeshPro.color = new Color(color.r, color.g, color.b, opacity);
        // Manipulate text mesh directly for performance
        TMP_MeshInfo[] meshInfo = textMeshPro.textInfo.meshInfo;
        for (int i = 0; i < meshInfo.Length; i++)
        {
            Color32[] vertexColors = meshInfo[i].colors32;
            
            if (vertexColors == null)
            {
                continue;
            }
            
            for (int j = 0; j < vertexColors.Length; j++)
            {
                vertexColors[j] = new Color32((byte) (color.r * 255.0f), (byte) (color.g * 255.0f), (byte) (color.b * 255.0f), vertexColors[j].a);
            }
        }
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}