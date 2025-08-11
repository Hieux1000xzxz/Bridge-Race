using UnityEngine;

public static class VictoryManager
{
    public static Color WinnerColor { get; private set; }
    public static string WinnerName { get; private set; }
    public static bool IsPlayerWinner { get; private set; }
    public static int CurrentLevel { get; private set; }

    public static void SetWinner(Character winner, int currentLevel)
    {
        WinnerColor = winner.Color;
        WinnerName = winner.name;
        IsPlayerWinner = winner.CompareTag("Player");
        CurrentLevel = currentLevel;
    }
}