using System;
using System.Threading.Tasks;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public string Enemy_Name;

    private EnemyClass enemy_class;
    public string Enemy_Race { get; private set;}
    public string Enemy_Class { get; private set;}
    private bool isDead = false;
    private float slideSpeed = 10f;
    private Shootable shootable;
    private Vector3 targetPos;
    private Vector3 SpawnPos;
    private Vector3 slideTargetPos;
    private string[] anim_arr;
    private string curr_anim;
    
    private enum State {
        Idle,
        Busy,
        Sliding,
        Dead,
    }
    private State state;
    
    private int attackTime = 0;
    private int deadTime = 0;

    void Awake() {
        
        anim_arr = new string[] {
            "Idle", "Attack", "Dead" ,"Ghost"
        };

        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips) {
            switch (clip.name) {
                case "Attack":
                    attackTime = Mathf.RoundToInt(clip.length * 1000);
                    break;
                case "Dead":
                    deadTime = Mathf.RoundToInt(clip.length * 1000);
                    break;
            }
        }

        SpawnPos = GetPosition();
    }

    void Start()
    {   
        shootable = GetComponent<Shootable>();
        //AttackTest();
        //state = State.Idle;
        //ChangeAnim(anim_arr[0], true);
        state = State.Idle;
    }

    
    async void Update()
    {
        switch (state) {
            case State.Idle:
                var idle = await ChangeAnim(anim_arr[0], false);
                break;
            case State.Sliding:
                transform.position += (slideTargetPos - GetPosition()) * slideSpeed * Time.deltaTime;
                
                float reachedDistance = 1f;
                if (Vector3.Distance(GetPosition(), slideTargetPos) < reachedDistance) {
                    transform.position = slideTargetPos;
                }
                break;
            case State.Dead:
                var ghost = await ChangeAnim(anim_arr[3], false);
                break;
        }
        //Debug.Log(targetPos);
    }

    public void SetUp(string _race, string _class, float _maxhp) {
        enemy_class = Enemy_Factory.GetEnemy(_race, _class, _maxhp);
        Enemy_Name = enemy_class.GetName();
    }

    public void SetUp() {
        enemy_class = new E_Dummy(350);
        Enemy_Name = enemy_class.GetName();
    }

    public float GetMaxHP() {
        return enemy_class.GetMaxHP();
    }

    public float GetHP() {
        return enemy_class.GetHP();

    }

    public string GetDMG_Type() {
        return enemy_class.GetDMG_Type();
    }

    public float MyDMG() {
        return enemy_class.MyDMG();
    }

    public void TakeDMG(float _oppositeDMG, string _spellType) {
        enemy_class.TakeDMG(_oppositeDMG, _spellType);
        
    }
    
    public bool Dead() {
        if (enemy_class.GetHP() <= 0) {
            if (!isDead) {
                FindObjectOfType<AudioManager>().PlaySound("Dead");
                Debug.Log(this.gameObject.name + " Dead");
                var result = ChangeAnim(anim_arr[2], true);
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

    public Vector3 GetPosition() {
        return transform.position;
    }

    public async void Attack(PlayerControl _target, Action onComplete) {
        Vector3 attackDistance = new (1.5f, 0, 0);
        targetPos = _target.GetPosition();
        
        //Slide to Target
        if (this.gameObject.name.Contains("Ranger")) {
            await PlayAttack();
            await FireArrow();
            onComplete();
        }
        else {
            await SlideToPosition(targetPos + attackDistance);
            await PlayAttack();
            onComplete();
            await SlideToPosition(SpawnPos);
        }
        state = State.Idle;
        
    }

    private async Task<bool> SlideToPosition(Vector3  slideTargetPos) {
        this.slideTargetPos = slideTargetPos;
        int waitTime = Mathf.RoundToInt(Vector3.Distance(GetPosition(), slideTargetPos) / slideSpeed * 1000);
        state = State.Sliding;
        await Task.Delay(waitTime);
        return true;
    }
    
    private async Task<bool> PlayAttack() {
        var result = await ChangeAnim(anim_arr[1], true);
        return result;
    }

    private async Task<bool> ChangeAnim(string _newAnim, bool _isOneTime) {
        if (curr_anim == _newAnim) return false;

        anim.Play(_newAnim);
        if (_isOneTime) {
            if (this.gameObject.name.Contains("Warrior")) {
                FindObjectOfType<AudioManager>().PlaySound("Warrior");
            }

            if (this.gameObject.name.Contains("Ranger")) {
                FindObjectOfType<AudioManager>().PlaySound("Bow");
            }
            await Task.Delay(attackTime);
        }

        curr_anim = _newAnim;
        return true;
    }

    public async Task<bool> FireArrow() {
        FindObjectOfType<AudioManager>().PlaySound("Arrow");
        var result = await shootable.Shoot(targetPos);
        return result;
    }

}