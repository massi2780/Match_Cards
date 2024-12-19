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
    [SerializeField] private GameObject aboutPanel;
    [SerializeField] private Button aboutButton;
    [SerializeField] private Button backButton;

    private int unlockedLevels;
    private Saver saver;

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

        saver = FindObjectOfType<Saver>();
        if (saver != null)
        {
            saver.Load_Datas();
            unlockedLevels = saver.Data_object.UnlockedLevel;
        }
        else
        {
            unlockedLevels = 1;
        }

        playButton.onClick.AddListener(OpenMap);
        closeButton.onClick.AddListener(CloseMap);

        if (aboutButton != null)
            aboutButton.onClick.AddListener(OpenAboutPanel);

        if (backButton != null)
            backButton.onClick.AddListener(CloseAboutPanel);

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
                levelButton.onClick.RemoveAllListeners();
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
        if (levelIndex >= 5)
        {
            Debug.Log("All levels completed! Loading EndGame.");
            SceneManager.LoadScene("EndGame");
        }
        else
        {
            Debug.Log("Loading Level: " + levelIndex);
            SceneManager.LoadScene("Level" + (levelIndex + 1));
        }
    }

    public void UnlockNextLevel()
    {
        if (unlockedLevels < levelIcons.Length)
        {
            unlockedLevels++;
            if (saver != null)
            {
                saver.Data_object.UnlockedLevel = unlockedLevels;
                saver.Save_Datas();
            }
            else
            {
                Debug.LogError("Saver instance not found! Data won't be saved.");
            }

            UpdateLevelIcons();
        }
    }
    public void UnlockNextLevelInMap()
    {
        UpdateLevelIcons();
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
