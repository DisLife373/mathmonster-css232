public class E_Gob_Warrior : E_Goblin
{
    public string enemyClass = "Warrior";

    private string DMG_Type = "Physical";

    public E_Gob_Warrior(float _maxhp) {
        base.maxHP = _maxhp;
        base.enemyHP = _maxhp;
        base.add_physical_dmg += 5;
        base.SetGoblin(-5f, 5f, -3f, 8f);
    }

    public override string GetName() {
        return enemyClass + " of The " + enemyRace;
    }

    public override string GetDMG_Type() {
        return this.DMG_Type;
    }

    public override float MyDMG() {
        
        return base.base_dmg + base.add_physical_dmg;
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
