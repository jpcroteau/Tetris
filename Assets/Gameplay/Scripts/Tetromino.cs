using UnityEngine;

public class Tetromino : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)]
    private Color color;

    private void OnValidate()
    {
        SetColor(color);
    }

    private void Awake()
    {
        SetColor(color);
    }
    
    private void SetColor(Color color)
    {
        var pb = new MaterialPropertyBlock();
        pb.SetColor("_BaseColor", color);
        
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            r.SetPropertyBlock(pb);
        }
    }
}
