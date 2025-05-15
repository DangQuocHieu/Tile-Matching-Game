using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeScreenHUD : MonoBehaviour
{
    [SerializeField] private Button _playButton;

    void Start()
    {
        AddButtonListener();
    }

    private void AddButtonListener()
    {
        _playButton.onClick.AddListener(()=>{
            SceneManager.LoadSceneAsync("Character Selection Scene");
        });
    }
}
