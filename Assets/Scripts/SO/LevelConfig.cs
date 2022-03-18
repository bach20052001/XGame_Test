using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelScriptableObject", order = 1)]
public class LevelConfig : ScriptableObject
{
    [Range(1,3)]
    public int Level;

    public float[] listAngle = new float[5];

    public float[] time = new float[5];
}