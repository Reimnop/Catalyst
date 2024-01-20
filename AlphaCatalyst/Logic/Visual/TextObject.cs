using TMPro;
using UnityEngine;

namespace Catalyst.Logic.Visual;

public class TextObject : VisualObject
{
    private readonly TextMeshPro textMeshPro;
    private readonly float opacity;

    public TextObject(GameObject gameObject, float opacity, string text)
    {
        this.opacity = opacity;
        
        textMeshPro = gameObject.GetComponent<TextMeshPro>();
        textMeshPro.enabled = true;
        textMeshPro.text = text;
    }
    
    public override void SetColor(Color color)
    {
        textMeshPro.color = new Color(color.r, color.g, color.b, opacity);
    }
}