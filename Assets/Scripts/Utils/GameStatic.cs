using UnityEngine;

public static class GameStatic
{
    //SETTINGS
    public static readonly Vector3 BRICK_HEIGHT = new Vector3(0f, .21f, 0f);

    //HASHES
    public static readonly int RUN = Animator.StringToHash("Run");
    public static readonly int ATTACK = Animator.StringToHash("Attack");
    public static readonly int CHEER = Animator.StringToHash("Cheer");
    public static readonly int FALL = Animator.StringToHash("Fall");

    public static readonly int BASE_COLOR = Shader.PropertyToID("_BaseColor");

}