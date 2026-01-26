using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] lines; // Each string is a line in the dialogue
}
