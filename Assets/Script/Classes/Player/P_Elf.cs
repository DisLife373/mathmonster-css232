public abstract class P_Elf : PlayerClass
{
    
    
    protected string playerRace = "Elf";

    public void SetElf(float _add=0f) {
        add_fire_dmg += 5f + _add;
        add_ice_dmg += 5f + _add;
        add_lightning_dmg += 5f + _add;
    }
    public abstract override string GetName();

    public abstract override float MyDMG(string _spellType);

    public abstract override void TakeDMG(float _oppositeDMG, string _spellType);

    public abstract override float GetHP();

    public abstract override float GetMaxHP();

    public abstract override void SoulGathering(float _enemyHP);

}
