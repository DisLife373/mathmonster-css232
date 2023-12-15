public abstract class P_Human : PlayerClass
{
    protected string playerRace = "Human";

    public void SetHuman() {
        physical_resist += 10;
    }
    public abstract override string GetName();

    public abstract override float MyDMG(string _spellType);

    public abstract override void TakeDMG(float _oppositeDMG, string _spellType);

    public abstract override float GetHP();

    public abstract override float GetMaxHP();

    public abstract override void SoulGathering(float _enemyHP);
}
