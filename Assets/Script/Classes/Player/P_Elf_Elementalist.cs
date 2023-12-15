using UnityEngine;

public class P_Elf_Elementalist : P_Elf
{
    private string playerClass = "Elementalist";

    public P_Elf_Elementalist(string _name, float _maxhp) {
        base.playerName = _name;
        base.maxHP = _maxhp;
        base.playerHP = _maxhp;
        base.SetElf(8f);
    }

    public override string GetName() {
        return base.playerName + ", \n" + playerClass + " of The " + base.playerRace;
    }

    public override float MyDMG(string _spellType) {
        return _spellType switch
        {
            "Fire" => base.base_dmg + base.add_fire_dmg,
            "Ice" => base.base_dmg + base.add_ice_dmg,
            "Lightning" => base.base_dmg + base.add_lightning_dmg,
            "Physical" => base.base_dmg + base.add_physical_dmg,
            _ => 0,
        };
    }

    public override void TakeDMG(float _oppositeDMG, string _spellType) {
        switch (_spellType) {
            case "Fire": 
                base.playerHP -= _oppositeDMG - base.add_fire_dmg;
                break;
            case "Ice": 
                base.playerHP -= _oppositeDMG - base.add_ice_dmg;
                break;
            case "Lightning": 
                base.playerHP -= _oppositeDMG - base.add_lightning_dmg;
                break;
            case "Physical": 
                base.playerHP -= _oppositeDMG - base.add_physical_dmg;
                break;
        }
    }

    public override float GetHP() {
        return playerHP;
    }
    
    public override float GetMaxHP() {
        return maxHP;
    }
    
    public override void SoulGathering(float _enemyHP) {
        Debug.Log("No Active Skill");
    }
}   
