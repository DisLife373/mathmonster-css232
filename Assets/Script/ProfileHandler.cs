using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ProfileHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text username_txt;
    [SerializeField] private TMP_Text race_txt;
    [SerializeField] private TMP_Text class_txt;
    [SerializeField] private TMP_Text skillRaceName;
    [SerializeField] private TMP_Text skillRaceDetail;
    [SerializeField] private TMP_Text skillClassName;
    [SerializeField] private TMP_Text skillClassDetail;
    [SerializeField] private TMP_Text zone_txt;
    [SerializeField] private TMP_Text stage_txt;
    [SerializeField] private List<GameObject> character_list;
    [SerializeField] private GameObject edit_panel;
    [SerializeField] private TMP_InputField edit_inp;

    private string username = "";
    private string prace = "";
    private string pclass = "";
    private string zone = "";
    private string stage = "";
    private Dictionary<string, List<string>> skill_dict;

    void Awake() {
        skill_dict = new Dictionary<string, List<string>>() {
            {"Human", new List<string>() {
                    "Human Strength", "+ Additional Earth Resistance"
                }
            },
            {"Elf", new List<string>() {
                    "Magic Wisdom", "+ Additional Fire Damage\n+ Additional Ice Damage\n+ Additional Lightning Damage"
                }
            },
            {"Elementalist", new List<string>() {
                    "Elemental Proficiency", "+ Additional Fire Damage\n+ Additional Ice Damage\n+ Additional Lightning Damage"
                }
            },
            {"Necromancer", new List<string>() {
                    "Soul Gathering", "Slightly heal yourself when kill a monster"
                }
            }
        };

        StartCoroutine(LoadGamePage());
    }
    
    void Start()
    {
        edit_panel.SetActive(false);

        foreach (GameObject cg in character_list) {
            cg.SetActive(false);
            if (cg.name == prace + pclass) {
                cg.SetActive(true);
                skillRaceName.text = skill_dict[prace][0];
                skillRaceDetail.text = skill_dict[prace][1];
                skillClassName.text = skill_dict[pclass][0];
                skillClassDetail.text = skill_dict[pclass][1];
            }
        }

        username_txt.text = username;
        race_txt.text = prace;
        class_txt.text = pclass;
        zone_txt.text = zone;
        stage_txt.text = stage;
    }

    public void GoBack() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        Main.instance.Load(Main.Scene.Menu);
    }

    public void EditUser() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        edit_panel.SetActive(true);
    }

    public void ConfirmEdit() {
        edit_panel.SetActive(false);
        StartCoroutine(UpdateUsername(edit_inp.text));
    }
    
    public void CancelEdit() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        edit_panel.SetActive(false);
    }

    IEnumerator LoadGamePage() {
        WWWForm form = new();
        form.AddField("sendUser", UserInfo.username);
        

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/LoadProfile.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            //Show results as text
            if (!www.downloadHandler.text.Contains("Can't find User.")) {
                string[] user_str = www.downloadHandler.text.Split(",");
                username = user_str[0];

                if(!www.downloadHandler.text.Contains("There is no Character.")) {
                    prace = user_str[1];
                    pclass = user_str[2];
                }

                if(!www.downloadHandler.text.Contains("Can't find any Saves.")) {
                    zone = user_str[3];
                    stage = user_str[4];
                }
                else if (www.downloadHandler.text.Contains("Can't find any Saves.")) {
                    zone = "0";
                    stage = "0";
                }
            }
            else {
                Debug.Log("Error");
            }
        }
    }
    IEnumerator UpdateUsername(string _editUser) {
        WWWForm form = new();
        form.AddField("edit_inp", _editUser);
        form.AddField("old_user", UserInfo.username);

        UnityWebRequest www = UnityWebRequest.Post("http://localhost/MathMonster/EditUser.php", form);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success) {
            Debug.Log(www.error);
        }
        else {
            //Show results as text
            if (www.downloadHandler.text.Contains(_editUser)) {
                FindObjectOfType<AudioManager>().PlaySound("Commit");
                string text = www.downloadHandler.text.Replace("Update Username Complete!", "");
                username_txt.text = text;
                UserInfo.SetUserInfo(text);
            }
            else if (www.downloadHandler.text.Contains("Can't Find User")) {
                FindObjectOfType<AudioManager>().PlaySound("Error");
                username_txt.text = "Can't Edit.";
            }
            else if (www.downloadHandler.text.Contains("This username already exits.")) {
                FindObjectOfType<AudioManager>().PlaySound("Error");
                username_txt.text = "Already Exits.";
            }
            else {
                FindObjectOfType<AudioManager>().PlaySound("Error");
                Debug.Log(www.downloadHandler.text);
            }
            
        }
    }
}
