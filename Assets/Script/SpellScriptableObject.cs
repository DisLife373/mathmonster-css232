using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells")]
public class SpellScriptableObject : ScriptableObject
{
    public string spellElement = "Natural";
    public float Speed = 15f;
}
