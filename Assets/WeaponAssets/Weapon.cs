using UnityEngine;

[CreateAssetMenu(menuName = "CSSim/Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName = "AK";
    public float damage = 35f;
    public float baseHitChance = 0.6f; // 0..1
    public float maxEffectiveRange = 20f;
    public int diceCount = 2; // number of d6 rolled
}
