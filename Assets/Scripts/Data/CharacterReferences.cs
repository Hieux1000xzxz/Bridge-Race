using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "CharacterReferences", menuName = "Scriptable Objects/CharacterReferences")]
[System.Serializable]
public class CharacterReferences
{
    public Animator animator;
    public NavMeshAgent agent;
    public Transform stackPoint;
    public Transform rayPoint;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public Rigidbody rb;
    public BrickStack brickStack;
}
