using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] private Image oldP_panel;
    [SerializeField] private Image newP_panel;
    [SerializeField] private GameObject exit_panel;

    [SerializeField] private GameObject phe_gobj;
    [SerializeField] private GameObject phn_gobj;
    [SerializeField] private GameObject pee_gobj;
    [SerializeField] private GameObject pen_gobj;

    void Start()
    {
        if (UserInfo.last_zone == 0 && UserInfo.last_stage == 0) {
            newP_panel.gameObject.SetActive(true);
            oldP_panel.gameObject.SetActive(false);

            SetNullPlayer();
        }
        else {
            newP_panel.gameObject.SetActive(false);
            oldP_panel.gameObject.SetActive(true);

            SetPlayerMenu(UserInfo.player_race, UserInfo.player_class);
        }
        Debug.Log("Menu " + UserInfo.last_zone + " " + UserInfo.last_stage);
    }

    private void SetPlayerMenu(string _race, string _class) {
        if (_race == "Human" && _class == "Elementalist") {
            phe_gobj.SetActive(true);
            phn_gobj.SetActive(false);
            pee_gobj.SetActive(false);
            pen_gobj.SetActive(false);
        }
        else if (_race == "Human" && _class == "Necromancer") {
            phe_gobj.SetActive(false);
            phn_gobj.SetActive(true);
            pee_gobj.SetActive(false);
            pen_gobj.SetActive(false);
        }
        else if (_race == "Elf" && _class == "Elementalist") {
            phe_gobj.SetActive(false);
            phn_gobj.SetActive(false);
            pee_gobj.SetActive(true);
            pen_gobj.SetActive(false);
        }
        else if (_race == "Elf" && _class == "Necromancer") {
            phe_gobj.SetActive(false);
            phn_gobj.SetActive(false);
            pee_gobj.SetActive(false);
            pen_gobj.SetActive(true);
        }
    }

    private void SetNullPlayer() {
        phe_gobj.SetActive(false);
        phn_gobj.SetActive(false);
        pee_gobj.SetActive(false);
        pen_gobj.SetActive(false);
    }

    private void QuitGame() {
        Application.Quit();
    }

    public void Continue() {
        if (UserInfo.last_zone > 1) {
            UserInfo.SetZoneStage(1, 4);
            Main.instance.Load(Main.Scene.Game);
        }
        else {
            FindObjectOfType<AudioManager>().PlaySound("Button");
            Main.instance.Load(Main.Scene.Game);
        }
        
    }
    public void Newgame() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        Main.instance.Load(Main.Scene.CharacterCreation);
    }
    public void Loadgame() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        Main.instance.Load(Main.Scene.Zone_Stage);
    }
    public void Proflie() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        Main.instance.Load(Main.Scene.Profile);
    }
    public void Exit() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        exit_panel.SetActive(true);
    }
    public void ConfirmExit() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        QuitGame();
    }
    public void CancelExit() {
        FindObjectOfType<AudioManager>().PlaySound("Button");
        exit_panel.SetActive(false);
    }

}
