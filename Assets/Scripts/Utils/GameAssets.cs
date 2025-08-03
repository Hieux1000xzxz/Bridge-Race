using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public static GameAssets Instance;
    public Brick Brick;
    public ContestantAI Contestant;

    private void Awake()
    {
        Instance = this;
    }
}
