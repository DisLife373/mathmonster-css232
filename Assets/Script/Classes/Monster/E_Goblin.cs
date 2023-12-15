public abstract class E_Goblin : EnemyClass
{
    public string enemyRace = "Goblin";

    public void SetGoblin(float _fireR_add=0f, float _iceR_add=0f, float _lightnigR_add=0f, float _physicalR_add=0f) {
        base.fire_resist += _fireR_add;
        base.ice_resist += _iceR_add;
        base.lightning_resist += _lightnigR_add;
        base.physical_resist += _physicalR_add;
    }
    
    public abstract override string GetName();
    public abstract override float MyDMG();
    public abstract override void TakeDMG(float _oppositeDMG, string _spellType);
    public abstract override float GetHP();
    public abstract override float GetMaxHP();
}
