using UnityEngine;

public class BridgeController : MonoBehaviour
{
    [SerializeField] Step[] Steps;
    [SerializeField] public int FloorLevel = 1;
    public Vector3 TopPosition => topPoint.position;

    [SerializeField] private Transform topPoint;
    int stepCount;
    private void Awake()
    {
        stepCount = Steps.Length;
    }

    public void SetAllStepsToNatural()
    {
        foreach (Step step in Steps)
        {
            step.SetNatural();
        }
    }
    public int GetColorCount(Color color)
    {
        int counter = 0;
        for (int i = 0; i < stepCount; i++)
        {
            if (Steps[i].Color.Equals(color))
                counter++;
        }
        return counter;
    }
    public bool IsCompleted
    {
        get
        {
            foreach (var step in Steps)
            {
                if (!step.IsBuilt) return false;
            }
            return true;
        }
    }
    public bool HasOtherColorBricks(Color myColor)
    {
        foreach (Step step in Steps)
        {
            if (step.Color != GameManager.Instance.NaturalColor && step.Color != myColor)
            {
                return true;
            }
        }
        return false;
    }




}
