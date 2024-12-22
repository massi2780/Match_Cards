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
    [SerializeField] private Button restartButton;
    [SerializeField] private Image[] stars;
    [SerializeField] private Sprite filledStar;
    [SerializeField] private Sprite emptyStar;

    [SerializeField] private float gameTime = 80f;
    [SerializeField] private Text timerText;
    [SerializeField] private GameObject TimerText;
    [SerializeField] private GameObject puzzleField;

    private bool timerIsRunning = true;

    public Sprite[] puzzles;
    public List<Sprite> gamePuzzles = new List<Sprite>();
    public List<Button> btns = new List<Button>();

    private bool firstGuess, secondGuess;
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;
  //  public Saver saver;

    void Start()
    {
        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        Saver saver = FindObjectOfType<Saver>();
        if (saver != null)
        {
            saver.Load_Datas();
            int currentLevel = saver.Data_object.UnlockedLevel;
            Debug.Log("Current Level Loaded: Level" + currentLevel);
        }

        // ادامه‌ی کد قبلی...
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

        yield return new WaitForSeconds(4f);

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

    void Update()
    {
        if (timerIsRunning)
        {
            if (gameTime > 0)
            {
                gameTime -= Time.deltaTime;
                UpdateTimerUI(gameTime);
            }
            else
            {
                gameTime = 0;
                timerIsRunning = false;
                TimeUp();
            }
        }
    }

    void TimeUp()
    {
        Debug.Log("Time is up! Game over.");

        if (TimerText != null)
            TimerText.SetActive(false);
       
        
        if (puzzleField != null)
            puzzleField.SetActive(false);
       



        if (endGamePanel != null)
        {
            endGamePanel.SetActive(true);

            DisplayStars(0);
            continueButton.interactable = false;

            backToMenuButton.onClick.AddListener(BackToMenu);


            restartButton.onClick.AddListener(RestartLevel);

        }

        foreach (Button btn in btns)
        {
            btn.interactable = false;
        }
    }


    void UpdateTimerUI(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void CheckTheGameIsFinished()
    {
        countCorrectGuesses++;
        if (countCorrectGuesses == gameGuesses)
        {
            timerIsRunning = false;

            Debug.Log("Game finished!!");

            if (endGamePanel != null)
            {
                endGamePanel.SetActive(true);

                ShowStars();
                int earnedStars = CalculateStars();

                if (earnedStars == 0)
                {
                    Debug.Log("No stars earned. Continue button disabled.");
                    continueButton.interactable = false;
                }
                else
                {
                    continueButton.interactable = true;
                    continueButton.onClick.AddListener(ContinueToNextLevel);
                }

                backToMenuButton.onClick.AddListener(BackToMenu);
                restartButton.onClick.AddListener(RestartLevel);
            }
        }
    }

    int CalculateStars()
    {
        if (countGuesses <= gameGuesses + 1)
            return 3;
        else if (countGuesses <= gameGuesses + 3)
            return 2;
        else if (countGuesses <= gameGuesses + 5)
            return 1;
        else
            return 0;
    }

    void ShowStars()
    {
        int earnedStars = CalculateStars();
        DisplayStars(earnedStars);
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


        Saver saver = FindObjectOfType<Saver>();
        if (saver != null)
        {
            saver.Load_Datas();


            saver.Data_object.UnlockedLevel++;
            saver.Save_Datas();


            MapController mapController = FindObjectOfType<MapController>();
            if (mapController != null)
            {
                mapController.UnlockNextLevelInMap(); 

            }


            int nextLevel = saver.Data_object.UnlockedLevel;
            if (nextLevel > 5) 

            {
                SceneManager.LoadScene("endGame");

            }
            else
            {
                SceneManager.LoadScene("Level" + nextLevel); 

            }
        }
        else
        {
            Debug.LogError("Saver not found!");
        }
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

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
