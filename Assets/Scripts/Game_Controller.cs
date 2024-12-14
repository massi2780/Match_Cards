using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    [SerializeField] private Sprite BG_Image;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private Button restartButton; // دکمه ریستارت
    [SerializeField] private Image[] stars; 
    [SerializeField] private Sprite filledStar; 
    [SerializeField] private Sprite emptyStar;

    public Sprite[] puzzles;
    public List<Sprite> gamePuzzles = new List<Sprite>();
    public List<Button> btns = new List<Button>();

    private bool firstGuess, secondGuess;
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    void Start()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        GetButtons();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;

        StartCoroutine(ShowCardsTemporarily());
    }

    IEnumerator ShowCardsTemporarily()
    {
        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].image.sprite = gamePuzzles[i];
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].image.sprite = BG_Image;
        }
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton");
        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = BG_Image;
        }
    }

    void AddGamePuzzles()
    {
        int looper = btns.Count;
        int index = 0;

        for (int i = 0; i < looper; i++)
        {
            if (index == looper / 2)
            {
                index = 0;
            }
            gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }

    void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => PickPuzzle());
        }
    }

    void PickPuzzle()
    {
        Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        int clickedButtonIndex = btns.IndexOf(clickedButton);

        if (firstGuess && clickedButtonIndex == firstGuessIndex)
        {
            Debug.Log("Cannot click the same card twice!");
            return;
        }

        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = clickedButtonIndex;
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;

            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = clickedButtonIndex;
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;

            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];
            countGuesses++;

            StartCoroutine(CheckThePuzzlesMatch());
        }
    }

    IEnumerator CheckThePuzzlesMatch()
    {
        yield return new WaitForSeconds(0.5f);
        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            CheckTheGameIsFinished();
        }
        else
        {
            btns[firstGuessIndex].image.sprite = BG_Image;
            btns[secondGuessIndex].image.sprite = BG_Image;
        }

        firstGuess = secondGuess = false;
    }

    void CheckTheGameIsFinished()
    {
        countCorrectGuesses++;
        if (countCorrectGuesses == gameGuesses)
        {
            Debug.Log("Game finished!!");

            if (endGamePanel != null)
            {
                endGamePanel.SetActive(true);

                continueButton.onClick.AddListener(ContinueToNextLevel);
                backToMenuButton.onClick.AddListener(BackToMenu);
                restartButton.onClick.AddListener(RestartLevel);

                ShowStars(); 
            }
        }
    }
    void ShowStars()
    {
        if (countGuesses <= gameGuesses + 2)
        {
            DisplayStars(3);
        }
        else if (countGuesses <= gameGuesses * 2) 
        {
            DisplayStars(2);
        }
        else 
        {
            DisplayStars(1);
        }
    }

    void DisplayStars(int starCount)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < starCount)
            {
                stars[i].sprite = filledStar;
            }
            else
            {
                stars[i].sprite = emptyStar;
            }
        }
    }

    void ContinueToNextLevel()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        int currentLevel = PlayerPrefs.GetInt("UnlockedLevels", 1);

        PlayerPrefs.SetInt("UnlockedLevels", currentLevel + 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Level" + (currentLevel + 1));
    }

    void BackToMenu()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        SceneManager.LoadScene("Main");
    }

    void RestartLevel()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ریستارت لول
    }

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
