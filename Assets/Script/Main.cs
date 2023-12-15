using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public static Main instance;

    public enum Scene {
        Authorization,
        Menu,
        CharacterCreation,
        Tutorial,
        Game,
        Zone_Stage,
        Profile,
    }
    
    
    void Start()
    {
        if (instance != null) {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        GameObject.DontDestroyOnLoad(this.gameObject);

        
    }

    public void Load(Scene scene) {
        SceneManager.LoadScene(scene.ToString());
    }
}
