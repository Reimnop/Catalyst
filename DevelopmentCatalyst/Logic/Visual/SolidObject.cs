using UnityEngine;

namespace Catalyst.Logic.Visual;

public class SolidObject : VisualObject
{
    private readonly GameObject gameObject;
    
    private readonly Material material;
    private readonly float opacity;

    public SolidObject(GameObject gameObject, float opacity, bool hasCollider)
    {
        this.gameObject = gameObject;
        this.opacity = opacity;
        
        Renderer renderer = gameObject.GetComponent<Renderer>();
        renderer.enabled = true;
        material = renderer.material;

        if (hasCollider)
        {
            Collider2D collider2D = gameObject.GetComponent<Collider2D>();

            if (collider2D != null)
            {
                Object.Destroy(collider2D);
            }
        }
    }
    
    public override void SetColor(Color color)
    {
        material.color = new Color(color.r, color.g, color.b, opacity);
    }
}