using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private Button playButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private GameObject[] levelIcons;

    [Header("About Panel Elements")]
    [SerializeField] private GameObject aboutPanel;   // پنل About
    [SerializeField] private Button aboutButton;      // دکمه About
    [SerializeField] private Button backButton;       // دکمه Back

    private int unlockedLevels;

    void Start()
    {
        mapPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);

        if (aboutPanel != null)
            aboutPanel.SetActive(false);  

        foreach (var icon in levelIcons)
        {
            icon.SetActive(false);
        }

        playButton.onClick.AddListener(OpenMap);
        closeButton.onClick.AddListener(CloseMap);

        if (aboutButton != null)
            aboutButton.onClick.AddListener(OpenAboutPanel);

        if (backButton != null)
            backButton.onClick.AddListener(CloseAboutPanel);

        unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);

        UpdateLevelIcons();
    }

    void OpenMap()
    {
        mapPanel.SetActive(true);
        foreach (var icon in levelIcons)
        {
            icon.SetActive(true);
        }
        closeButton.gameObject.SetActive(true);
    }

    void CloseMap()
    {
        mapPanel.SetActive(false);
        foreach (var icon in levelIcons)
        {
            icon.SetActive(false);
        }
        closeButton.gameObject.SetActive(false);
    }

    void UpdateLevelIcons()
    {
        for (int i = 0; i < levelIcons.Length; i++)
        {
            Button levelButton = levelIcons[i].GetComponent<Button>();

            if (i < unlockedLevels)
            {
                levelIcons[i].GetComponent<Image>().color = Color.white;
                levelButton.interactable = true;

                int levelIndex = i;
                levelButton.onClick.AddListener(() => LoadLevel(levelIndex));
            }
            else
            {
                levelIcons[i].GetComponent<Image>().color = Color.gray;
                levelButton.interactable = false;
            }
        }
    }

    void LoadLevel(int levelIndex)
    {
        Debug.Log("Loading Level: " + levelIndex);
        SceneManager.LoadScene("Level" + (levelIndex + 1));
    }

    public void UnlockNextLevel()
    {
        if (unlockedLevels < levelIcons.Length)
        {
            unlockedLevels++;
            PlayerPrefs.SetInt("UnlockedLevels", unlockedLevels);
            PlayerPrefs.Save();
        }
    }

    void OpenAboutPanel()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(true);
    }

    void CloseAboutPanel()
    {
        if (aboutPanel != null)
            aboutPanel.SetActive(false);
    }
}
