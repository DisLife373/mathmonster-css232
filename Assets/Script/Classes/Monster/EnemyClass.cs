using UnityEngine;


public abstract class EnemyClass : ScriptableObject {
    protected float enemyHP;
    protected float maxHP;
    protected float base_dmg = 25f;
    protected float add_physical_dmg = 0f;
    protected float add_fire_dmg = 0f;
    protected float add_ice_dmg = 0f;
    protected float add_lightning_dmg = 0f;
    protected float fire_resist = 0f;
    protected float ice_resist = 0f;
    protected float lightning_resist = 0f;
    protected float physical_resist = 0f;

    public abstract string GetName();
    public abstract string GetDMG_Type();
    public abstract float MyDMG();
    public abstract void TakeDMG(float _oppositeDMG, string _spellType);
    public abstract float GetHP();
    public abstract float GetMaxHP();
}
