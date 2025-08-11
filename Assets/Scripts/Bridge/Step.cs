using UnityEngine;

public class Step : MonoBehaviour
{
    public Color Color;
    [SerializeField] private MeshRenderer meshRenderer;
   
    public bool IsBuilt => meshRenderer.enabled;

    private void OnEnable()
    {
        meshRenderer.enabled = false;
    }
    public void SetNatural()
    {
        Color = GameManager.Instance.NaturalColor;
        meshRenderer.enabled = false;
        meshRenderer.material.SetColor(GameStatic.BASE_COLOR, Color);
    }
    public void SetColor(Color color)
    {
        Color = color;
        meshRenderer.enabled = true;
        meshRenderer.material.SetColor(GameStatic.BASE_COLOR, color);
    }

}
