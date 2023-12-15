public class E_Dummy : EnemyClass
{
    public E_Dummy(float _maxhp) {
        base.maxHP = _maxhp;
        base.enemyHP = _maxhp;
    }

    public override string GetName() {
        return "Dummy, Please Hit Me Harder~!";
    }

    public override float MyDMG() { 
        return 5;
    }

    public override string GetDMG_Type()
    {
        return "Physical";
    }

    public override void TakeDMG(float _oppositeDMG, string _spellType) {
        switch (_spellType) {
            case "Fire": 
                base.enemyHP -= _oppositeDMG - base.add_fire_dmg;
                break;
            case "Ice": 
                base.enemyHP -= _oppositeDMG - base.add_ice_dmg;
                break;
            case "Lightning": 
                base.enemyHP -= _oppositeDMG - base.add_lightning_dmg;
                break;
            case "Physical": 
                base.enemyHP -= _oppositeDMG - base.add_physical_dmg;
                break;
        }
    }

    public override float GetHP() {
        return enemyHP;
    }

    public override float GetMaxHP() {
        return maxHP;
    }

    
}
