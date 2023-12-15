using UnityEngine;

public class Player_Factory : MonoBehaviour
{
    public static PlayerClass GetPlayer(string _username,string _race, string _class, float _maxhp) {
        if (_race == "Human" && _class == "Elementalist") {
            return new P_Hu_Elementalist(_username, _maxhp);
        }
        else if (_race == "Human" && _class == "Necromancer") {
            return new P_Hu_Necromancer(_username, _maxhp);
        }
        else if (_race == "Elf" && _class == "Elementalist") {
            return new P_Elf_Elementalist(_username, _maxhp);
        }
        else if (_race == "Elf" && _class == "Necromancer") {
            return new P_Elf_Necromancer(_username, _maxhp);
        }
        else {
            return null;
        }
    }
}
