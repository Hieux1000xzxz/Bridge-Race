using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider col;
    [SerializeField] private Rigidbody rb;

    public int FloorLevel { get; private set; }
    public bool IsHidden => !gameObject.activeSelf;

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
        SetColor(character.Color);

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

        if (pickupDelayCoroutine != null)
        {
            StopCoroutine(pickupDelayCoroutine);
        }
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
        if (pickupDelayCoroutine != null)
        {
            StopCoroutine(pickupDelayCoroutine);
            pickupDelayCoroutine = null;
        }

        transform.DOKill();

        ResetBrickState();

        ObjectPool.instance.Despawn(this.gameObject);
    }

    private void ResetBrickState()
    {
        transform.SetParent(null);

        rb.isKinematic = true;
        rb.useGravity = false;

        col.enabled = true;

        canBePicked = true;

        brickData = null;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void SetFloorLevel(int level)
    {
        FloorLevel = level;
    }

    private void OnEnable()
    {
        canBePicked = true;
    }
}