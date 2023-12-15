using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] private List<Spell> listSpell;
    [SerializeField] private Transform castPoint;
    [SerializeField] private Animator anim;
    
    public string Player_Name {get; private set;}
    public string spellType;

    private PlayerClass player_class;
    private Spell spellToCast;
    private Castable castable;
    private Vector3 enemyPos;
    private string[] anim_arr;
    private string curr_anim;
    private bool isDead = false;
    private State state;
    private enum State {
        Idle,
        Casting,
        Attack,
        Hit,
        Dead,
    }
    private Dictionary<string, int> oneTime_anim;

    void Awake() {
        castable = GetComponent<Castable>();

        oneTime_anim = new Dictionary<string, int>() {
            {"PreCast", 0},
            {"Attack", 0},
            {"Dead", 0}
        };

        anim_arr = new string[] {
            "Idle", "PreCast", "Cast", "Attack" ,"Dead" ,"Ghost"
        };
        
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "PreCast": 
                    oneTime_anim["PreCast"] = Mathf.RoundToInt(clip.length * 1000);
                    break;
                case "Attack": 
                    oneTime_anim["Attack"] = Mathf.RoundToInt(clip.length * 1000);
                    break;
                case "Dead": 
                    oneTime_anim["Dead"] = Mathf.RoundToInt(clip.length * 1000);
                    break;
            }
        }
    }

    void Start()
    {   
        transform.gameObject.SetActive(true);
        spellType = "";

        state = State.Idle;
    }

    
    void Update()
    {
        switch (state) {
            case State.Idle:
                var result = ChangeAnim(anim_arr[0], false);
                break;
            case State.Casting:
                var cast = ChangeAnim(anim_arr[2], false);
                break;
            case State.Attack:
                break;
            case State.Hit:
                break;
            case State.Dead:
                var ghost = ChangeAnim(anim_arr[5], false);
                break;
        }
    }
    
    public void SetUp() {
        player_class = Player_Factory.GetPlayer(UserInfo.username, UserInfo.player_race, UserInfo.player_class, UserInfo.player_maxhp);
        Player_Name = player_class.GetName();
    }

    public float GetMaxHP() {
        return player_class.GetMaxHP();
    }

    public float GetHP() {
        return player_class.GetHP();
    }

    public string GetDMG_Type() {
        return "";
    }

    public float MyDMG(string _spellType) {
        return player_class.MyDMG(_spellType);
    }

    public void TakeDMG(float _oppositeDMG, string _spellType) {
        player_class.TakeDMG(_oppositeDMG, _spellType);
    }

    public void ActiveSkill(float _enemyHP) {
        if (this.gameObject.name.Contains("Necromancer")) {
            player_class.SoulGathering(_enemyHP);
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void SpellCastAnim() {
        state = State.Casting;
    }

    public async void SpellPreCastAnim() {
        var result = await ChangeAnim(anim_arr[1], false);
        state = State.Casting;
    }

    public async void Attack(EnemyControl _target, Action onComplete) {
        enemyPos = _target.GetPosition();

        await PlayAnimAttack();
        await CastSpell(enemyPos);

        state = State.Idle;
        onComplete();
    }

    public void ResetAnim() {
        state = State.Idle;
    }

    public bool Dead() {
        if (player_class.GetHP() <= 0) {
            if (!isDead) {
                FindObjectOfType<AudioManager>().PlaySound("Dead");
                var result = ChangeAnim(anim_arr[4], true);
                FindObjectOfType<AudioManager>().PlaySound("Ghost");
                isDead = true;
                state = State.Dead;
            }
            return true;
        }
        else {
            return false;
        }
    }

    private async Task<bool> PlayAnimAttack() {
        var result = await ChangeAnim(anim_arr[3], true);
        
        //await Task.Delay(Mathf.CeilToInt(anim.GetCurrentAnimatorStateInfo(0).length * 1000));
        state = State.Attack;
        return result;
    }

    private async Task<bool> ChangeAnim(string _newAnim, bool _isOneTime) {
        if (curr_anim == _newAnim) return false;
        
        anim.Play(_newAnim);
        if (_isOneTime) {
            await Task.Delay(oneTime_anim[_newAnim]);
            
        }
        curr_anim = _newAnim;
        return true;
    }

    public async Task<bool> CastSpell(Vector3 _enemyPos) {
        spellToCast = spellType switch
        {
            "Fire" => listSpell[0],
            "Ice" => listSpell[1],
            "Lightning" => listSpell[2],
            "Physical" => listSpell[3],
            _ => listSpell[3],
        };
        var result = await castable.Cast(_enemyPos, spellToCast, castPoint.position, castPoint.rotation);
        return result;
    }

    
}

/*
P_Elf_Elementalist player = new P_Elf_Elementalist("Elena");
Debug.Log(player.getName());
Debug.Log(player.myDMG());
*/