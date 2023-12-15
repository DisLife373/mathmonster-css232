using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class StageSelectionHandler : MonoBehaviour
{
    [SerializeField] private List<GameObject> zone_lock;
    [SerializeField] private List<GameObject> stage_lock;
    [SerializeField] private GameObject zone_panel;
    [SerializeField] private GameObject stage_panel;

    private int select_zone = 0;
    private int page_num = 0;
    private int zone, stage;
    private Dictionary<int, int> gameStage;

    void Awake() {
        gameStage = new Dictionary<int, int>(){
            {1, 0},
            {2, 0},
            {3, 0},
            {4, 0}
        };
        StartCoroutine(LoadCurrentStage());
    }

    void Start()
    {
        
        page_num = 1;
        zone_panel.SetActive(true);
        stage_panel.SetActive(false);
        LoadZone();
    }

    public void SelectZone1() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        page_num++;
        select_zone = 1;
        zone_panel.SetActive(false);
        stage_panel.SetActive(true);
        LoadStage();
    }
    public void SelectZone2() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        page_num++;
        select_zone = 2;
        zone_panel.SetActive(false);
        stage_panel.SetActive(true);
        LoadStage();
    }
    public void SelectZone3() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        page_num++;
        select_zone = 3;
        zone_panel.SetActive(false);
        stage_panel.SetActive(true);
        LoadStage();
    }
    public void SelectZone4() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        page_num++;
        select_zone = 4;
        zone_panel.SetActive(false);
        stage_panel.SetActive(true);
        LoadStage();
    }

    public void SelectStage1() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        UserInfo.SetZoneStage(select_zone, 1);
        Main.instance.Load(Main.Scene.Game);
    }
    public void SelectStage2() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        UserInfo.SetZoneStage(select_zone, 2);
        Main.instance.Load(Main.Scene.Game);
    }
    public void SelectStage3() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        UserInfo.SetZoneStage(select_zone, 3);
        Main.instance.Load(Main.Scene.Game);
    }
    public void SelectStage4() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        UserInfo.SetZoneStage(select_zone, 4);
        Main.instance.Load(Main.Scene.Game);
    }

    public void GoBack() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        if (page_num == 1) {
            Main.instance.Load(Main.Scene.Menu);
        } 
        else if (page_num == 2) {
            page_num--;
            zone_panel.SetActive(true);
            stage_panel.SetActive(false);
        }
    }

    private void LoadZone() {
        for (int i = zone; i < 4; i++) {
            zone_lock[i].SetActive(true);
        }
        
    }
    private void LoadStage() {
        if (select_zone == 1) {
            for (int i = 0; i < 4 - gameStage[1]; i++) {
                stage_lock[3-i].SetActive(true);
            }
            for (int i = 0; i < gameStage[1]; i++) {
                stage_lock[i].SetActive(false);
            }
        }
        else if (select_zone == 2) {
            for (int i = 0; i < 4; i++) {
                stage_lock[i].SetActive(true);
            }
        }
    }

    IEnumerator LoadCurrentStage() {
        WWWForm form = new();
        form.AddField("user_inp", UserInfo.username);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/LoadCurrentStage.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            //Show results as text
            //Debug.Log(www.downloadHandler.text);
            if (!www.downloadHandler.text.Contains("Can't find any saves.")) {
                string[] result_str = www.downloadHandler.text.Split(",");

                int lzone = int.Parse(result_str[0]);
                int lstage = int.Parse(result_str[1]);

                if (lstage == 4 && lzone != 0 && lstage != 0) {
                    zone = lzone + 1;
                    stage = 1;
                }
                else if (lzone == 0 && lstage == 0) {
                    zone = 1;
                    stage = 1;
                }
                else {
                    zone = lzone;
                    stage = lstage + 1;
                }

                int i = 1;
                while (i <= zone) {
                    //gameStage[i] = i == zone ? stage : gameStage[i] + 4;
                    if (i == zone) {
                        gameStage[i] = stage;
                    }
                    else {
                        gameStage[i] += 4;
                    }
                    
                    i++;
                }
                
                UserInfo.SetZoneStage(zone, stage);
            }
            else {
                if (UserInfo.last_zone == 1 && UserInfo.last_stage == 1) {
                    zone++;
                    stage++;
                }
                else {
                    zone = 0;
                    stage = 0;
                }
                
            }
        }
    }
}
