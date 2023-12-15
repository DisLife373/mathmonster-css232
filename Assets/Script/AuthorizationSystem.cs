using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections;

public class AuthorizationSystem : MonoBehaviour
{
    //Panel
    [SerializeField] private Image loginPanel;
    [SerializeField] private Image regisPanel;
    //Login
    [SerializeField] private TMP_InputField loginUser_inp;
    [SerializeField] private TMP_InputField loginPass_inp;
    [SerializeField] private TMP_Text loginLog_txt;

    //Register
    [SerializeField] private TMP_InputField regisUser_inp;
    [SerializeField] private TMP_InputField regisPass_inp;
    [SerializeField] private TMP_InputField regisConfirm_inp;
    [SerializeField] private TMP_Text regisLog_txt;

    //GetPlayer
    private string prace;
    private string pclass;
    private float pmaxhp;

    void Start() {
        FindObjectOfType<AudioManager>().PlaySound("MenuBG");
        
    }

    private void LoadMenu() {
        Debug.Log("Login Complete " + UserInfo.last_zone + " " + UserInfo.last_stage);
        Main.instance.Load(Main.Scene.Menu);
    }

    public void GameLogin() {
        FindObjectOfType<AudioManager>().PlaySound("Commit");
        StartCoroutine(Login(loginUser_inp.text, loginPass_inp.text));
    }
    public void GameRegister() {
        
        if (regisPass_inp.text == "" || regisConfirm_inp.text == "" ||
            regisPass_inp.text == null || regisConfirm_inp.text == null
        ) {
            FindObjectOfType<AudioManager>().PlaySound("Error");
            regisLog_txt.text = "Your information should not be null.";
        }
        else if (regisPass_inp.text.Length < 8 || 
            regisPass_inp.text.Length > 20 || 
            regisConfirm_inp.text.Length < 8 || 
            regisConfirm_inp.text.Length > 20 ||
            regisUser_inp.text.Length < 8 || 
            regisUser_inp.text.Length > 20
            
        ) {
            FindObjectOfType<AudioManager>().PlaySound("Error");
            regisLog_txt.text = "Please enter 8-20 characters";
        }
        else if (regisPass_inp.text == regisConfirm_inp.text) {
            FindObjectOfType<AudioManager>().PlaySound("Commit");
            StartCoroutine(Register(regisUser_inp.text, regisPass_inp.text));
        }
        else {
            FindObjectOfType<AudioManager>().PlaySound("Error");
            Debug.Log("Password does not match, try again.");
        }
    }

    public void GoRegister() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        loginPanel.gameObject.SetActive(false);
        regisPanel.gameObject.SetActive(true);
    }

    public void GoBack() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        loginPanel.gameObject.SetActive(true);
        regisPanel.gameObject.SetActive(false);
    }

    IEnumerator Login(string _loginUser, string _loginPass) {
        WWWForm form = new();
        form.AddField("user_inp", _loginUser);
        form.AddField("pass_inp", Hekshuuu.HashString(_loginPass));

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/Login.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            //Show results as text
            if (!www.downloadHandler.text.Contains("Incorrect Password!") && 
                !www.downloadHandler.text.Contains("Username does not exits.")
            ) {
                string[] result_str = www.downloadHandler.text.Split("*");
                
                UserInfo.SetUserInfo(_loginUser);

                int zone = int.Parse(result_str[0]);
                int stage = int.Parse(result_str[1]);

                if (zone != 0 && stage != 0 && stage != 4) {
                    UserInfo.SetZoneStage(zone, stage + 1);
                }
                else if (zone != 0 && stage == 4) {
                    UserInfo.SetZoneStage(zone + 1, 1);
                }
                else {
                    UserInfo.SetZoneStage(zone, stage);
                }

                if (result_str[2] != "") {
                    StartCoroutine(GetPlayer(result_str[2], () => {
                        UserInfo.SetPlayerInfo(prace, pclass, pmaxhp);
                    }));
                    
                    
                }
                else {
                    UserInfo.SetPlayerInfo("", "", 0);
                    
                }
                Invoke("LoadMenu", 1);
            }
            else {
                FindObjectOfType<AudioManager>().PlaySound("Error");
                loginLog_txt.text = www.downloadHandler.text;
            }
        }
    }

    IEnumerator Register(string _regisUser, string _regisPass) {
        WWWForm form = new();
        form.AddField("user_inp", _regisUser);
        form.AddField("pass_inp", Hekshuuu.HashString(_regisPass));

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/Register.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            //Show results as text
            regisLog_txt.text = www.downloadHandler.text;
            loginUser_inp.text = _regisUser;
            loginPass_inp.text = _regisPass;
            Invoke("GoBack", 1);
        }
    }

    IEnumerator GetPlayer(string _player_id, Action onGetComplete) {
        WWWForm form = new();
        form.AddField("getPlayer_id", _player_id);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/GetPlayerInfo.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            string[] result_str = www.downloadHandler.text.Split("*");
            prace = result_str[0];
            pclass = result_str[1];
            pmaxhp = float.Parse(result_str[2]);
            onGetComplete();
        }
    }
    
}

