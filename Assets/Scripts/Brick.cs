using DG.Tweening;
using Lean.Pool;
using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider col;
    [SerializeField] private Rigidbody rb;

    public int FloorLevel { get; private set; }
    public bool IsHidden => !gameObject.activeSelf;
    private const float JUMP_DURATION = .5f;
    private const float JUMP_POWER = .001f;
    private const float NO_PICKUP_TIME = 0.5f;
    public bool IsTaken => brickData != null && brickData.IsCollected;
    private bool canBePicked = true;
    public Color Color;
    private BrickGridData brickData;
    private Coroutine pickupDelayCoroutine;

    public void SetBrickData(BrickGridData data, int floorLevel = 0)
    {
        this.FloorLevel = floorLevel;
        brickData = data;
        Color = data.Color;
        meshRenderer.material.SetColor(GameStatic.BASE_COLOR, data.Color);
        col.enabled = true;
    }

    public void SetColor(Color newColor)
    {
        Color = newColor;
        meshRenderer.material.color = newColor;
    }

    public void Take(Character character)
    {
        if (!canBePicked || character == null || character.StackPoint == null)
            return;
        col.enabled = false;
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.SetParent(character.StackPoint);
        SetColor(character.Color);
        int index = character.GetBrickCount();
        Vector3 localPos = new Vector3(0f, index * 0.4f, 0f);
        transform.DOLocalJump(localPos, JUMP_POWER, 1, JUMP_DURATION);
        transform.DOLocalRotate(Vector3.zero, JUMP_DURATION);

        if (brickData != null)
        {
            brickData.IsCollected = true;
        }
        character.AddBrick(this);
    }

    public void Drop()
    {
        transform.SetParent(null);
        SetColor(GameManager.Instance.NaturalColor);
        col.enabled = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        canBePicked = false;
        rb.AddForce(
            new Vector3(
                Random.Range(-2f, 2f),
                Random.Range(3f, 5f),
                Random.Range(-2f, 2f)
            ),
            ForceMode.Impulse
        );
        pickupDelayCoroutine = StartCoroutine(EnablePickupAfterDelay());
    }

    private IEnumerator EnablePickupAfterDelay()
    {
        yield return new WaitForSeconds(NO_PICKUP_TIME);
        canBePicked = true;
        pickupDelayCoroutine = null;
    }

    public void Despawn()
    {
        LeanPool.Despawn(this);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
    public void SetFloorLevel(int level)
    {
        FloorLevel = level;
    }

}