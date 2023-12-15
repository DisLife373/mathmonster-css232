using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using KBMath;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.Networking;
using Unity.VisualScripting;



public class TutorialHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text time_txt;
    [SerializeField] private TMP_Text game_log;
    [SerializeField] private GameObject spellcastUI;
    [SerializeField] private GameObject block_screen;
    [SerializeField] private TMP_InputField inpFomular;
    [SerializeField] private TMP_Text goal_txt;
    [SerializeField] private TMP_Text pname_txt;
    [SerializeField] private TMP_Text ename_txt;
    [SerializeField] private HealthBar phealth;
    [SerializeField] private HealthBar ehealth;
    [SerializeField] private GameObject endgame_panel;
    [SerializeField] private GameObject dummy_txt;
    [SerializeField] private List<Image> stars;
    [SerializeField] private Sprite star;
    [SerializeField] private Sprite star_blank;
    [SerializeField] private TMP_Text endglog_txt;
    [SerializeField] private GameObject continue_btn;
    [SerializeField] private List<TMP_Text> materials_text;
    [SerializeField] private Transform pheCharacter;
    [SerializeField] private Transform phnCharacter;
    [SerializeField] private Transform peeCharacter;
    [SerializeField] private Transform penCharacter;
    [SerializeField] private Transform playerParent;
    [SerializeField] private EnemyControl enemy;
    [SerializeField] private GameObject dialogue_panel;
    [SerializeField] private TMP_Text npc_txt;
    [SerializeField] private GameObject warn_panel;
    [SerializeField] private TMP_Text npcWarn_txt;
    
    private PlayerControl player;
    private GameObject targetEnemy;
    private EnemyControl targetEControl;
    private EnemyControl saveTargetControl;
    private State state;

    private enum State {
        playerTurn,
        spellCasting,
        playerAttack,
        enemyTurn,
    }

    private List<List<int>> problems;
    private int runNum = -1;

    private string[] npc_dialogue;
    private string[] warn_dialogue;
    private int dialogue = 0;
    private int wdialogue = 0;

    //Input
    private Camera _mainCamera;

    //Spell Cast
    private double currentTime = 0;
    private bool isTimesUp = false;
    private bool isOneTime = false;
    private bool isEnemyAttack = false;
    private bool isPAttack = false;
    private bool isPCast = false;
    private Dictionary<char, int> signDict;
    private string save_txt = "";

    void Awake() {
        _mainCamera = Camera.main;
        endgame_panel.SetActive(false);
        
    }
    void Start()
    {
        FindObjectOfType<AudioManager>().PlaySound("GameBG");
        problems = new List<List<int>>{
            new() {20, 2, 6, 1, 7},
            new() {24, 7, 8, 4, 6},
            new() {10, 4, 6, 3, 3},
            new() {41, 8, 5, 7, 7}
        };

        npc_dialogue = new string[] {
            "สวัสดี นักผจญภัย ข้างหน้านี้มีศัตรูตัวฉกาดขวางอยู่ !",
            "เรามาดูกันว่าเราจะทำไรได้บ้าง.",
            "จะเห็นว่าแถบด้านล่างของจอเธอจะมี \"ตัวเลขตรงกลาง\", \"กล่องสี่เหลียมที่มีตัวเลข\" และ \"ช่องใส่ข้อความ\"",

            "เริ่มจาก \"ตัวเลขตรงกลาง\" มันคือตัวเลขที่เธอต้องคำนวณออกมาให้ได้",
            "โดยใช้ตัวเลขใน \"กล่องสี่เหลียมที่มีตัวเลข\" ทั้ง 4 ตัวในการคำนวณ",
            "เธอต้องพิมพ์ลงใน \"ช่องใส่ข้อความ\"",
            "ส่วนเครื่องหมายจะใช้ \n + แทนการบวก, - แทนการลบ\n * แทนการคูณ, / แทนการหาร",
            "เธอสามารถใช้วงเล็บ ( ) ได้ด้วยนะ !",
            "และเธอมีเวลาคำนวณต่อโจทย์ 180 วินาที",
            "เอาล่ะ มาเริ่มกันเลย ~ !",
            "ให้เธอพิมพ์ตามชั้นก่อนนะ !",

            "อ้าว สงสัยชั้นคิดผิด แต่ก็ดีเลย ! ถ้าเธอคิดเลขผิดเธอก็จะโดนมอนส์เตอร์โจมตี.",
            "เอาล่ะลองอีกที",

            "เก่งมาก !",
            "จะเห็นได้ว่าเมื่อกี้เธอปล่อยเวทย์ไฟออกมา เพราะว่าเธอใช้เครื่องหมาย + ในสูตร 2 ตัวไงล่ะ !",
            "การปล่อยเวทย์แต่ละธาตุจะขึ้นอยู่กับเครื่องหมายที่เธอใช้ ซึ่งมอนส์เตอร์แต่ละตัวก็จะแพ้ธาตุต่างกัน",
            "เวทย์ไฟ จะใช้เครื่องหมายบวก( + ) 2 ตัวขึ้นไป.",
            "ทีนี้ลองเวทย์น้ำแข็งกัน !",
            "เวทย์น้ำแข็ง จะใช้เครื่องหมายลบ( - ) 2 ตัวขึ้นไป.",

            "ต่อมาลองเวทย์ไฟฟ้าจากโจทย์เดิมกัน !",
            "เวทย์ไฟฟ้า จะใช้เครื่องหมายคูณ( * ) 2 ตัวขึ้นไป.",

            "ต่อมาลองเวทย์ดินกัน !",
            "เวทย์ดิน จะใช้เครื่องหมายหาร( / ) 2 ตัวขึ้นไป หรือ ใช้เครื่องหมายแต่ละตัวไม่ถึง 2 ซักตัว.",

            "สุดท้ายมาลองเวทย์ดินแบบมีเครื่องหมายซ้ำกันไม่ถึง 2 กัน !",
            
            "เก่งมาก ~ !",
            "แล้วก็... เมื่อมอนส์เตอร์ตายก็จะจบเกมและจะได้ดาว",
            "3 ดาว ถ้า HP เธอเต็มหลอด, \n2 ดาว ถ้า HP มากกว่า 40%, \n1 ดาว ถ้า HP น้อยกว่า 40%",
            "และก็จะมีการบันทึกปริมาณ HP ของเธอและเวลาการตลอดเล่นตอนจบเกม",
            "เอาล่ะ ! ชั้นจะช่วยเธออีกแรงนะ.",
            "ย้ากกกก ~ !"
        };

        warn_dialogue = new string[] {
            "เอ๋า~! อย่าปล่อยให้เวลาหมดสิ",
            "ถ้าเวลาหมดเธอจะโดนมอนส์เตอร์โจมตีนะ",
            "ระวังตัวด้วย~!"
        };

        dialogue_panel.SetActive(true);
        npc_txt.text = npc_dialogue[dialogue];
        
        player = SpawnPlayer();
        enemy.SetUp();

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

        currentTime = 180;

        isOneTime = true;
        
        RunProblem();

        state = State.playerTurn;
        spellcastUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.playerTurn || (state == State.spellCasting && !isPCast)) {
            if (saveTargetControl == targetEControl && saveTargetControl != null) {
                saveTargetControl = null;
                player.SpellCastAnim();
                isPCast = true;
            }

            time_txt.text = currentTime.ToString("F0");

            if (currentTime < 0) {
                isTimesUp = true;
            }
            

            if (isTimesUp && isOneTime) {
                isOneTime = false;
                dialogue_panel.SetActive(false);
                wdialogue = 0;
                enemy.Attack(player, () => {
                    inpFomular.text = "";
                    player.TakeDMG(enemy.MyDMG(), enemy.GetDMG_Type());
                    phealth.SetHealth(player.GetHP());
                    
                    warn_panel.SetActive(true);
                    npcWarn_txt.text = warn_dialogue[wdialogue];
                });
            }

            else if (!isTimesUp) {
                currentTime -= 1 * Time.deltaTime;
                if (Mouse.current.leftButton.isPressed) {
                    Vector2 mousePosition = Mouse.current.position.ReadValue();
                    Ray ray = _mainCamera.ScreenPointToRay(mousePosition);
                    RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
                    if (hit.collider != null && hit.collider.tag != "Player") {
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
                        
                    }
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
                targetEControl.TakeDMG(player.MyDMG(player.spellType), player.spellType);
                ehealth.SetHealth(targetEControl.GetHP());
                ResetSign();
                dialogue_panel.SetActive(true);
                currentTime = 180;
                saveTargetControl = targetEControl;
                state = State.playerTurn;
            });
        }
        
        else if (state == State.enemyTurn && isEnemyAttack) {
            spellcastUI.SetActive(false);
            player.ResetAnim();

            isEnemyAttack = false;
            
            EnemyAttack();
        }
    }

    public void Calculate() {
        MathParser mathParser = new('.');
        string input = inpFomular.text;

        if (CountSign(input)) {
            double actual = mathParser.Parse(input);

            if (actual.ToString() != "") {
                if (actual.ToString() == goal_txt.text) {
                    isPAttack = true;
                    isPCast = true;
                    player.spellType = CheckSign();
                    game_log.text = "";
                    state = State.playerAttack;
                }
                else {
                    isEnemyAttack = true;
                    state = State.enemyTurn;
                }

                currentTime = 180;
            }
        }
        else {
            save_txt = game_log.text;
            game_log.text = "Enter Again!";
            Invoke("ReturnSaveTXT", 1);
        }
    }

    public void GoSelect() {
        Main.instance.Load(Main.Scene.Zone_Stage);
        FindObjectOfType<AudioManager>().PlaySound("MenuBG");
    }

    public void GoNextStage() {
        StartCoroutine(SaveGame());
        Main.instance.Load(Main.Scene.Game);
        
    }

    public void RunDialogue() {
        if (dialogue != 29) {
            dialogue++;
            npc_txt.text = npc_dialogue[dialogue];
        }
        else {
            dialogue_panel.SetActive(false);
            targetEControl.TakeDMG(999, "Fire");
            ehealth.SetHealth(targetEControl.GetHP());
            EndGame();
        }

        switch (dialogue) {
            case 3:
                dialogue_panel.SetActive(false);
                spellcastUI.SetActive(true);
                block_screen.SetActive(true);
                Invoke("WaitForLooking", 4);
                
                break;
            case 11:
                dialogue_panel.SetActive(false);
                game_log.text = "(2+6)-7*1";
                dummy_txt.SetActive(true);
                break;
            case 12:
                game_log.text = "";
                dummy_txt.SetActive(false);
                break;
            case 13:
                dialogue_panel.SetActive(false);
                game_log.text = "(2*6)+7+1";
                dummy_txt.SetActive(true);
                break;
            case 14:
                game_log.text = "";
                dummy_txt.SetActive(false);
                RunProblem();
                break;
            case 19:
                dialogue_panel.SetActive(false);
                game_log.text = "(6-(7-4))*8";
                dummy_txt.SetActive(true);
                break;
            case 20:
                game_log.text = "";
                dummy_txt.SetActive(false);
                break;
            case 21:
                dialogue_panel.SetActive(false);
                game_log.text = "(8-7)*6*4";
                dummy_txt.SetActive(true);
                break;
            case 22:
                game_log.text = "";
                dummy_txt.SetActive(false);
                RunProblem();
                break;
            case 23:
                dialogue_panel.SetActive(false);
                game_log.text = "(6+4)/(3/3)";
                dummy_txt.SetActive(true);
                break;
            case 24:
                dialogue_panel.SetActive(false);
                game_log.text = "8*5+7/7";
                RunProblem();
                break;
            case 25:
                game_log.text = "";
                dummy_txt.SetActive(false);
                break;
        }
        
        
    }

    public void RunWarningDialogue() {
        if (wdialogue == warn_dialogue.Length - 1) {
            wdialogue = 0;
            currentTime = 180;
            warn_panel.SetActive(false);
            isTimesUp = false;
            int[] backlist = new int[] {
                3, 11, 13, 19, 21, 23, 24
            };
            if (!backlist.Contains(dialogue)) {
                dialogue_panel.SetActive(true);
                npc_txt.text = npc_dialogue[dialogue];
            }
            state = State.playerTurn;
        }
        else {
            wdialogue++;
            npcWarn_txt.text = warn_dialogue[wdialogue];
        }
        
    }

    private void EnemyAttack() {
        targetEControl.Attack(player, () => {
            player.TakeDMG(targetEControl.MyDMG(), targetEControl.GetDMG_Type());
            phealth.SetHealth(player.GetHP());
            dialogue_panel.SetActive(true);
            state = State.playerTurn;
        });
        if (!targetEControl) {
            enemy.Attack(player, () => {
                inpFomular.text = "";
                player.TakeDMG(enemy.MyDMG(), enemy.GetDMG_Type());
                phealth.SetHealth(player.GetHP());
                dialogue_panel.SetActive(true);
                state = State.playerTurn;
            });
                
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

    private void ReturnSaveTXT() {
        game_log.text = save_txt;
    }

    private void RunProblem() {
        runNum++;
        goal_txt.text = problems[runNum][0].ToString();
        for (int i = 0; i < 4; i++) {
            materials_text[i].text = problems[runNum][i + 1].ToString();
        }
    }

    private bool CountSign(string _input_text) {
        bool check = true;
        int[] materials = new int[4];
        
        foreach (char c in _input_text) {
            if (Char.IsNumber(c) && !problems[runNum].Contains((int)char.GetNumericValue(c))) {
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

    private void EndGame() {
        endgame_panel.SetActive(true);
        continue_btn.SetActive(true);
        endglog_txt.text = "You Win !";
        for (int i = 0; i < 2; i++) {
            stars[i].sprite = star;
        }

    }

    private void WaitForLooking() {
        dialogue_panel.SetActive(true);
        spellcastUI.SetActive(false);
        block_screen.SetActive(false);
    }

    IEnumerator SaveGame() {
        WWWForm form = new();
        form.AddField("sendUser", UserInfo.username);
        form.AddField("sendZone", 0);
        form.AddField("sendStage", 0);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/SaveGame.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            //Show results as text
            Debug.Log(www.downloadHandler.text);
            UserInfo.SetZoneStage(1, 1);
        }
    }
}
