public abstract class E_Orc : EnemyClass
{
    public string enemyRace = "Orc";

    public void SetOrc() {
        base.fire_resist += 10;
        base.ice_resist += 10;
        base.lightning_resist -= 10;
        base.physical_resist += 15;
    }
    public abstract override string GetName();
    public abstract override float MyDMG();
    public abstract override void TakeDMG(float _oppositeDMG, string _spellType);
    public abstract override float GetHP();
    public abstract override float GetMaxHP();
}
