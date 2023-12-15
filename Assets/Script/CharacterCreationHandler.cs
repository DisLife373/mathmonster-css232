using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterCreationHandler : MonoBehaviour
{
    public static CharacterCreationHandler instance;

    //frame
    [SerializeField] private GameObject race_panel;
    [SerializeField] private GameObject class_panel;

    //race panel
    [SerializeField] private TMP_Text race_choosen_txt;
    [SerializeField] private TMP_Text race_detail_txt;

    //class panel
    [SerializeField] private TMP_Text class_choosen_txt;
    [SerializeField] private TMP_Text class_detail_txt;

    //scroll
    [SerializeField] private GameObject scroll_race;
    [SerializeField] private GameObject scroll_human_class;
    [SerializeField] private GameObject scroll_elf_class;

    //button
    [SerializeField] private GameObject choose_race_btn;
    [SerializeField] private GameObject choose_class_btn;

    public string raceCC;
    public string classCC;

    private Dictionary<string, string> player_info;
    private int pageNum = 0;

    void Start()
    {
        instance = this;
        player_info = new Dictionary<string, string>{
            {"Human", "+Additional Earth Resistance"},
            {"HumanElementalist", "+Additional Fire Damage\n+Additional Ice Damage\n+Additional Lightning Damage"},
            {"HumanNecromancer", "Greatly heal yourself when kill a monster"},
            {"Elf", "+Additional Fire Damage\n+Additional Ice Damage\n+Additional Lightning Damage"},
            {"ElfElementalist", "+Additional Fire Damage\n+Additional Ice Damage\n+Additional Lightning Damage"},
            {"ElfNecromancer", "Slightly heal yourself when kill a monster"}
        };
        scroll_race.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowRace() {
        race_choosen_txt.text = raceCC;
        race_detail_txt.text = player_info[raceCC];
    }

    private void ShowClass() {
        class_choosen_txt.text = classCC;
        class_detail_txt.text = player_info[raceCC + classCC];
    }

    private void ClearInfo() {
        race_choosen_txt.text = "Not Select";
        race_detail_txt.text = "";
        class_choosen_txt.text = "Not Select";
        class_detail_txt.text = "";
    }

    public void GetInfo() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        if (classCC == "") {
            ShowRace();
            race_panel.SetActive(true);
            choose_race_btn.SetActive(true);
        }
        else if (raceCC != "" && classCC != "") {
            ShowClass();
            class_panel.SetActive(true);
            choose_race_btn.SetActive(false);
            choose_class_btn.SetActive(true);
        }
    }

    public void InfoHuman() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        raceCC = "Human";
        GetInfo();
    }
    public void InfoElf() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        raceCC = "Elf";
        GetInfo();
    }
    public void InfoElementalist() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        classCC = "Elementalist";
        GetInfo();
    }
    public void InfoNecromancer() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        classCC = "Necromancer";
        GetInfo();
    }

    public void GoBack() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        raceCC = "";
        classCC = "";

        if (pageNum == 1) {
            pageNum--;
            scroll_race.SetActive(true);
            scroll_human_class.SetActive(false);
            scroll_elf_class.SetActive(false);
            class_panel.SetActive(false);
            choose_race_btn.SetActive(true);
            choose_class_btn.SetActive(false);
        }
        else {
            Main.instance.Load(Main.Scene.Menu);
        }
    }

    public void ChooseRace() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        pageNum++;
        scroll_race.SetActive(false);
        if (raceCC == "Human") {
            scroll_human_class.SetActive(true);
        }
        else {
            scroll_elf_class.SetActive(true);
        } 
    }
    public void ChooseClass() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        StartCoroutine(SetPlayer(UserInfo.username, raceCC, classCC));
        Debug.Log("Choose Complete " + UserInfo.last_zone + " " + UserInfo.last_stage);
        Main.instance.Load(Main.Scene.Tutorial);
    }

    IEnumerator SetPlayer(string _username, string _race, string _class) {
        WWWForm form = new();
        form.AddField("username", _username);
        form.AddField("race", _race);
        form.AddField("class", _class);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/SetPlayer.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            string maxhp_str = "";
            if (www.downloadHandler.text.Contains("Delete Saves Complete!")) {
                maxhp_str = www.downloadHandler.text.Replace("Delete Saves Complete!", "");
            }
            float maxhp_conv = float.Parse(maxhp_str);
            UserInfo.SetPlayerInfo(_race, _class, maxhp_conv);
            UserInfo.SetZoneStage(0, 0);
        }
    }
}
