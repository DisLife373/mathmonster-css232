using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public string enemy_race { get; private set;}
    public string enemy_class { get; private set;}
    public float enemy_maxhp { get; private set;}

    public void SetEnemyInfo(string _race, string _class, float _maxhp) {
        enemy_race = _race;
        enemy_class = _class;
        enemy_maxhp = _maxhp;
    }
}
