using UnityEngine;

public class Enemy_Factory : MonoBehaviour
{
    public static EnemyClass GetEnemy(string _race, string _class, float _maxhp) {
        if (_race == "Goblin" && _class == "Warrior") {
            return new E_Gob_Warrior(_maxhp);
        }
        else if (_race == "Goblin" && _class == "Ranger") {
            return new E_Gob_Ranger(_maxhp);
        }
        else if (_race == "Orc" && _class == "Warrior") {
            return new E_Orc_Warrior(_maxhp);
        }
        else {
            return null;
        }
    }
}
