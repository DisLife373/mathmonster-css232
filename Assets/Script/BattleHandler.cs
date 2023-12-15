using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using KBMath;
using System.Linq;
using System;
using Unity.VisualScripting;


public class BattleHandler : MonoBehaviour
{
    private static BattleHandler instance;
    public static BattleHandler GetInstance() {
        return instance;
    }

    [SerializeField] private TMP_Text time_txt;
    [SerializeField] private TMP_Text game_log;
    [SerializeField] private GameObject spellcastUI;
    [SerializeField] private TMP_InputField inpFomular;
    [SerializeField] private TMP_Text goal_txt;
    [SerializeField] private TMP_Text pname_txt;
    [SerializeField] private TMP_Text ename_txt;
    [SerializeField] private HealthBar phealth;
    [SerializeField] private HealthBar ehealth;
    [SerializeField] private GameObject endgame_panel;
    [SerializeField] private List<Image> stars;
    [SerializeField] private Sprite star;
    [SerializeField] private Sprite star_blank;
    [SerializeField] private TMP_Text endglog_txt;
    [SerializeField] private GameObject continue_btn;
    [SerializeField] private GameObject pause_panel;
    [SerializeField] private List<TMP_Text> materials_text;
    [SerializeField] private Transform pheCharacter;
    [SerializeField] private Transform phnCharacter;
    [SerializeField] private Transform peeCharacter;
    [SerializeField] private Transform penCharacter;
    [SerializeField] private List<Transform> enemyCharacter;
    [SerializeField] private Transform playerParent;
    [SerializeField] private Transform enemyParent;
    
    private List<EnemyControl> enemies;
    private List<Transform> listCharacter;
    private int enemy_num;
    private PlayerControl player;
    private GameObject targetEnemy;
    private EnemyControl targetEControl;
    private EnemyControl saveTargetControl;
    private int total_star;
    private double playTime = 0;
    private bool isEnd = false;
    private bool isPause = false;
    private bool isTimesUp = false;
    private bool isOneTime = true;
    private State state;

    private enum State {
        playerTurn,
        spellCasting,
        playerAttack,
        enemyTurn,
    }

    private List<List<int>> problems;
    private int enemyGoal;
    private int[] enemyMaterial;
    private int randNum;

    //Input
    private Camera _mainCamera;

    //Spell Cast
    private double currentTime = 0;
    private bool isEnemyAttack = false;
    private bool isPAttack = false;
    private bool isPCast = false;
    private Dictionary<char, int> signDict;

    void Awake() {
        _mainCamera = Camera.main;
        problems = new List<List<int>>();
        Debug.Log("Before Load " + UserInfo.last_zone + " " + UserInfo.last_stage);
        endgame_panel.SetActive(false);
        StartCoroutine(LoadGamePage());
        Debug.Log("After Load " + UserInfo.last_zone + " " + UserInfo.last_stage);
        currentTime = 180;
        state = State.playerTurn;
    }
    void Start()
    {
        FindObjectOfType<AudioManager>().PlaySound("GameBG");

        player = SpawnPlayer();
        
        pname_txt.text = player.Player_Name;
        phealth.SetMaxHealth(player.GetMaxHP());
        phealth.SetHealth(player.GetHP());


        signDict = new Dictionary<char, int>
        {
            { '+', 0 },
            { '-', 0 },
            { '*', 0 },
            { '/', 0 }
        };

        
        enemyMaterial = new int[4];
        RandProblem();

        
        isEnd = false;

        
        spellcastUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isEnd && !isPause) {
            playTime += 1 * Time.deltaTime;
            RunTurn();
            
        }
        
    }

    public void Calculate() {
        MathParser mathParser = new('.');
        string input = inpFomular.text;

        if (CountSign(input)) {
            double actual = mathParser.Parse(input);

            if (actual.ToString() != "") {
                FindObjectOfType<AudioManager>().PlaySound("Button");
                if (actual.ToString() == goal_txt.text) {
                    game_log.text = "Player Turn!";
                    isPAttack = true;
                    isPCast = true;
                    player.spellType = CheckSign();
                    state = State.playerAttack;
                }
                else {
                    isEnemyAttack = true;
                    state = State.enemyTurn;
                }
                RandProblem();
                currentTime = 180;
            }
        }
        else {
            FindObjectOfType<AudioManager>().PlaySound("Error");
            game_log.text = "Enter Again!";
        }
    }

    public void GoSelect() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        Main.instance.Load(Main.Scene.Zone_Stage);
        FindObjectOfType<AudioManager>().PlaySound("MenuBG");
    }

    public void GoNextStage() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        if (UserInfo.last_zone == 1 && UserInfo.last_stage == 4) {
            Main.instance.Load(Main.Scene.Zone_Stage);
            FindObjectOfType<AudioManager>().PlaySound("MenuBG");
        }
        else {
            Main.instance.Load(Main.Scene.Game);
        }
        
    }

    public void Pause() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        isPause = true;
        pause_panel.SetActive(true);
    }

    public void Resume() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        isPause = false;
        pause_panel.SetActive(false);
    }

    public void Back2Menu() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        Main.instance.Load(Main.Scene.Menu);
        pause_panel.SetActive(false);
        FindObjectOfType<AudioManager>().PlaySound("MenuBG");
    }
    

    private void RunTurn() {
        if (state == State.playerTurn || (state == State.spellCasting && !isPCast)) {
            if (saveTargetControl == targetEControl && saveTargetControl != null) {
                saveTargetControl = null;
                player.SpellCastAnim();
                isPCast = true;
            }

            if (isTimesUp && isOneTime) {
                isOneTime = false;
                EnemyAttack();
            }
            
            else if (!isTimesUp) {
                if (game_log.text != "Enter Again!") game_log.text = "Player Turn!";
                time_txt.text = currentTime.ToString("F0");
                currentTime -= 1 * Time.deltaTime;
                if (currentTime < 0) {
                    currentTime = 180;
                    isEnemyAttack = true;
                    state = State.enemyTurn;
                }
                else {
                    PlayerInteract();
                }
            }
            
            
        }
        else if (state == State.spellCasting && isPCast) {
            player.SpellPreCastAnim();
            isPCast = false;
        }
        else if (state == State.playerAttack && isPAttack) {
            spellcastUI.SetActive(false);

            isPAttack = false;
            player.Attack(targetEControl, () => {
                inpFomular.text = "";
                currentTime = 180;
                targetEControl.TakeDMG(player.MyDMG(player.spellType), player.spellType);
                ehealth.SetHealth(targetEControl.GetHP());
                if (targetEControl.Dead() && player.gameObject.name.Contains("Necromancer")) {
                    player.ActiveSkill(targetEControl.GetMaxHP());
                    phealth.SetHealth(player.GetHP());
                }
                ResetSign();
                IsEndGame();
                state = State.playerTurn;
            });
            
            
        }
        
        else if (state == State.enemyTurn && isEnemyAttack) {
            game_log.text = "Monster Turn!";
            spellcastUI.SetActive(false);
            player.ResetAnim();

            isEnemyAttack = false;
            
            //EnemiesAttack(enemy_num - 1);
            EnemyAttack();
            

        }
    }

    private void EnemyAttack() {
        targetEControl.Attack(player, () => {
            player.TakeDMG(targetEControl.MyDMG(), targetEControl.GetDMG_Type());
            phealth.SetHealth(player.GetHP());
            IsEndGame();
            state = State.playerTurn;
            RandProblem();
        });
        if (!targetEControl) {
            foreach (EnemyControl e in enemies) {
                if (!e.Dead()) {
                    e.Attack(player, () => {
                        inpFomular.text = "";
                        player.TakeDMG(e.MyDMG(), e.GetDMG_Type());
                        phealth.SetHealth(player.GetHP());
                        IsEndGame();
                        state = State.playerTurn;
                        RandProblem();
                    });
                    break;
                }
            }
        }
    }

    // private void EnemiesAttack(int _index) {
    //     enemies[_index].Attack(player, () => {
    //         player.TakeDMG(enemies[_index].MyDMG(), enemies[_index].GetDMG_Type());
    //         phealth.SetHealth(player.GetHP());
    //         IsEndGame();
    //         if (_index == 0) {
    //             state = State.playerTurn;
    //             RandProblem();
    //         }
    //         else {
    //             if (player.Dead()) {
    //                 inpFomular.text = "";
    //                 state = State.playerTurn;
    //                 RandProblem();
    //             }
    //             else {
                    
    //                 EnemiesAttack(_index - 1);
    //             }
    //         }
    //     });
    // }

    private void PlayerInteract() {
        if (Mouse.current.leftButton.isPressed) {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if (hit.collider != null) {
                state = State.spellCasting;
                isPCast = true;

                GameObject oldGameObject = GameObject.FindGameObjectWithTag("Selected");
                if (oldGameObject != null) oldGameObject.tag = "Untagged";
                targetEnemy = hit.collider.gameObject;
                targetEnemy.tag = "Selected";
                targetEControl = targetEnemy.GetComponent<EnemyControl>();
                if (targetEControl) {
                    ename_txt.text = targetEControl.Enemy_Name;
                    ehealth.SetMaxHealth(targetEControl.GetMaxHP());
                    ehealth.SetHealth(targetEControl.GetHP());
                }
                else {
                    ename_txt.text = "Enemy Name";
                }
                spellcastUI.SetActive(true);
                goal_txt.text = enemyGoal.ToString();
                for (int i = 0; i < 4; i++) {
                    materials_text[i].text = enemyMaterial[i].ToString();
                }

            }
        }
    }

    private PlayerControl SpawnPlayer() {
        string prace = UserInfo.player_race;
        string pclass = UserInfo.player_class;
        Transform playerTransform = GetComponent<Transform>();
        if (prace == "Human" && pclass == "Elementalist") {
            playerTransform = Instantiate(pheCharacter, new Vector3(-6f, 0.5f), Quaternion.identity, playerParent);
        }
        else if (prace == "Human" && pclass == "Necromancer") {
            playerTransform = Instantiate(phnCharacter, new Vector3(-6f, 0.5f), Quaternion.identity, playerParent);
        }
        else if (prace == "Elf" && pclass == "Elementalist") {
            playerTransform = Instantiate(peeCharacter, new Vector3(-6f, 0.5f), Quaternion.identity, playerParent);
        }
        else if (prace == "Elf" && pclass == "Necromancer") {
            playerTransform = Instantiate(penCharacter, new Vector3(-6f, 0.5f), Quaternion.identity, playerParent);
        }
        PlayerControl playerControl = playerTransform.GetComponent<PlayerControl>();
        playerControl.SetUp();
        return playerControl;
    }

    private void RandProblem() {
        randNum = UnityEngine.Random.Range(0, problems.Count - 1);
        enemyGoal = problems[randNum][0];
        for (int i = 0; i < 4; i++) {
            enemyMaterial[i] = problems[randNum][i + 1];
        }
    }

    private bool CountSign(string _input_text) {
        bool check = true;
        
        foreach (char c in _input_text) {
            if (Char.IsNumber(c) && !enemyMaterial.Contains((int)char.GetNumericValue(c))) {
                check = false;
                
            }
            if (signDict.Keys.Contains(c)) {
                signDict[c] += 1;
            }
        }
        return check;
    }

    private string CheckSign() {
        foreach (char c in signDict.Keys) {
            if (signDict[c] >= 2) {
                return c switch
                {
                    '+' => "Fire",
                    '-' => "Ice",
                    '*' => "Lightning",
                    '/' => "Physical",
                    _ => "Physical",
                };
            }
        }
        return "Physical";
    }

    private void PrintSign() {
        foreach (var x in signDict) {
            Debug.Log(x.Key + " " + x.Value);
        }
    }

    private void ResetSign() {
        signDict['+'] = 0;
        signDict['-'] = 0;
        signDict['*'] = 0;
        signDict['/'] = 0;
    }

    private bool CheckEnemyDead() {
        foreach (EnemyControl e in enemies) {
            if (!e.Dead()) {
                return false;
            }
        }
        return true;
    }

    private void IsEndGame() {
        if (CheckEnemyDead() && !player.Dead()) {
            FindObjectOfType<AudioManager>().PlaySound("LevelPass");
            isEnd = true;
            endgame_panel.SetActive(true);
            continue_btn.SetActive(true);
            endglog_txt.text = "You Win !";
            total_star = GetStars();
            for (int i = 0; i < total_star; i++) {
                stars[i].sprite = star;
            }
            Save();
            
        }
        else if (!CheckEnemyDead() && player.Dead()) {
            FindObjectOfType<AudioManager>().PlaySound("NotPass");
            isEnd = true;
            endgame_panel.SetActive(true);
            continue_btn.SetActive(false);
            endglog_txt.text = "You Lose !";
            for (int i = 0; i < 3; i++) {
                stars[i].sprite = star_blank;
            }
        }
    }

    private int GetStars() {
        if (player.GetHP() >= player.GetMaxHP()) {
            return 3;
        }
        else if (player.GetHP() >= player.GetMaxHP() * 0.4f) {
            return 2;
        }
        else {
            return 1;
        }
    }

    private void Save() {
        StartCoroutine(SaveGame());
        StartCoroutine(SaveStat());
        if (UserInfo.last_stage != 4) {
            UserInfo.SetZoneStage(UserInfo.last_zone, UserInfo.last_stage + 1);
        }
    }

    IEnumerator LoadGamePage() {
        WWWForm form = new();
        form.AddField("sendUser", UserInfo.username);
        form.AddField("sendZone", UserInfo.last_zone);
        form.AddField("sendStage", UserInfo.last_stage);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/LoadGame.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            //Show results as text
            if (!www.downloadHandler.text.Contains("Can't find problems") ||
                !www.downloadHandler.text.Contains("Can't find enemies")
            ) {

                string[] php_str = www.downloadHandler.text.Split("+");
                string[] problems_str = php_str[0].Split("*");
                string[] enemies_str = php_str[1].Split("*");

                
                int prob_material = 0;
                for (int i = 0; i < problems_str.Length; i++) {
                    List<int> prob = new();
                    foreach (string s in problems_str[i].Split(",")) {
                        prob_material = int.Parse(s);
                        prob.Add(prob_material);
                    }
                    problems.Add(prob);
                }

                enemy_num = enemies_str.Length;
                enemies = new List<EnemyControl>(enemy_num);

                List<Vector3> enemyPos = new(enemy_num);
                switch (enemy_num) {
                    case 1:
                        enemyPos = new List<Vector3>{
                            new(5f, 0)
                        };
                        break;
                    case 2:
                        enemyPos = new List<Vector3>{
                            new(3f, -0.5f),
                            new(6f, 0.5f)
                        };
                        break;
                    case 3:
                        enemyPos = new List<Vector3>{
                            new(3f, -0.3f),
                            new(6f, 0.7f),
                            new(5.7f, -1.8f)
                        };
                        break;
                    case 4:
                        enemyPos = new List<Vector3>{
                            new(3f, 0.7f),
                            new(3f, -1.5f),
                            new(6f, 0.7f),
                            new(5.7f, -1.8f)
                        };
                        break;
                }
                
                
                listCharacter = new List<Transform>(enemy_num);
                for (int i = 0; i < enemies_str.Length; i++) {
                    string[] enemy_info  = enemies_str[i].Split(",");
                    string charName = "Enemy_" + enemy_info[0] + "_" + enemy_info[1];
                    float maxhp_conv = float.Parse(enemy_info[2]);
                    
                    for (int j = 0; j < enemyCharacter.Count; j++) {
                        if (enemyCharacter[j].name == charName) {
                            listCharacter.Add(enemyCharacter[j]);
                            Transform enemyTransform = Instantiate(listCharacter[i], enemyPos[i], Quaternion.identity, enemyParent);
                            EnemyControl enemyControl = enemyTransform.GetComponent<EnemyControl>();
                            enemyControl.SetUp(enemy_info[0], enemy_info[1], maxhp_conv);
                            enemies.Add(enemyControl);
                        }
                    }
                }

                
                
            }
            else {
                Debug.Log("Error");
            }
        }
    }

    IEnumerator SaveGame() {
        WWWForm form = new();
        form.AddField("sendUser", UserInfo.username);
        form.AddField("sendZone", UserInfo.last_zone);
        form.AddField("sendStage", UserInfo.last_stage);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/SaveGame.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            //Show results as text
            Debug.Log(www.downloadHandler.text);
            UserInfo.SetZoneStage(UserInfo.last_zone, UserInfo.last_stage);
        }
    }

    IEnumerator SaveStat() {
        WWWForm form = new();
        form.AddField("sendUser", UserInfo.username);
        form.AddField("sendZone", UserInfo.last_zone);
        form.AddField("sendStage", UserInfo.last_stage);
        form.AddField("sendStars", total_star);
        form.AddField("sendLastHP", player.GetHP().ToString());
        form.AddField("sendPlayTime", playTime.ToString());

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/SaveStat.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            playTime = 0;
            Debug.Log(www.downloadHandler.text);
            
        }
    }
}
