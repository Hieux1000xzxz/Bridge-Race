using UnityEngine;

public class Step : MonoBehaviour
{
    public Color Color;
    private MeshRenderer meshRenderer;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

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
