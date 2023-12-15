public abstract class PlayerClass {
    protected string playerName;
    protected float playerHP;
    protected float maxHP;
    protected float base_dmg = 35f;
    protected float add_fire_dmg = 0f;
    protected float add_ice_dmg = 0f;
    protected float add_lightning_dmg = 0f;
    protected float add_physical_dmg = 0f;
    protected float fire_resist = 0f;
    protected float ice_resist = 0f;
    protected float lightning_resist = 0f;
    protected float physical_resist = 0f;
    
    public abstract string GetName();

    public abstract float MyDMG(string _spellType);

    public abstract void TakeDMG(float _oppositeDMG, string _spellType);

    public abstract float GetHP();
    
    public abstract float GetMaxHP();
    
    public abstract void SoulGathering(float _enemyHP);
}
