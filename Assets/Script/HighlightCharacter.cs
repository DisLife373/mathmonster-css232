using UnityEngine;
using UnityEngine.UI;

public class HighlightCharacter : MonoBehaviour
{
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform choice1;
    [SerializeField] private RectTransform choice2;

    private Image choice1_img;
    private Image choice2_img;

    private bool isSwitched = false;
    void Start()
    {
        choice1_img = choice1.GetComponent<Image>();
        choice2_img = choice2.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (panel.anchoredPosition.x > 500 && !isSwitched) {
            if (choice1.localScale.y == 1) choice1.localScale *= 1.1f;
            if (choice2.localScale.y != 1) choice2.localScale /= 1.1f;
            choice1_img.color = new Color32(255,255,255,255);
            choice2_img.color = new Color32(162,162,162,255);
            if (choice1.gameObject.name.Contains("_")) {
                string[] slicer = choice1.gameObject.name.Split("_");
                CharacterCreationHandler.instance.raceCC = slicer[0];
                CharacterCreationHandler.instance.classCC = slicer[1];
            }
            else {
                CharacterCreationHandler.instance.raceCC = choice1.gameObject.name;
            }
            CharacterCreationHandler.instance.GetInfo();
            isSwitched = true;
        }
        else if (panel.anchoredPosition.x < 500 && isSwitched) {
            if (choice2.localScale.y == 1) choice2.localScale *= 1.1f;
            if (choice1.localScale.y != 1) choice1.localScale /= 1.1f;
            choice2_img.color = new Color32(255,255,255,255);
            choice1_img.color = new Color32(162,162,162,255);
            if (choice2.gameObject.name.Contains("_")) {
                string[] slicer = choice2.gameObject.name.Split("_");
                CharacterCreationHandler.instance.raceCC = slicer[0];
                CharacterCreationHandler.instance.classCC = slicer[1];
            }
            else {
                CharacterCreationHandler.instance.raceCC = choice2.gameObject.name;
            }
            CharacterCreationHandler.instance.GetInfo();
            isSwitched = false;
        }
        
    }

    public void Clickchoice1() {
        panel.anchoredPosition = new Vector2(666.9752f, 2.8f);
    }
    public void Clickchoice2() {
        panel.anchoredPosition = new Vector2(329.1649f, 2.8f);
    }
}
