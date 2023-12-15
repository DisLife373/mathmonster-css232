using UnityEngine;

public class UserInfo : MonoBehaviour
{
    public static string username {get; private set;}
    public static string player_race {get; private set;}
    public static string player_class {get; private set;}
    public static float player_maxhp {get; private set;}
    public static int last_zone {get; private set;}
    public static int last_stage {get; private set;}

    public static void SetUserInfo(string _username) {
        username = _username;
    }
    public static void SetPlayerInfo(string _race, string _class, float _maxhp) {
        player_race = _race;
        player_class = _class;
        player_maxhp = _maxhp;
    }
    public static void SetZoneStage(int _zone, int _stage) {
        last_zone = _zone;
        last_stage = _stage;
    }
    
}
