using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Game_Controller : MonoBehaviour
{
    [SerializeField]
    private Sprite BG_Image;
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
            btn.onClick.AddListener(() => pickPuzzle());
        }
    }

    void pickPuzzle()
    {
        Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        int clickedButtonIndex = btns.IndexOf(clickedButton);

        Debug.Log("Button clicked: " + clickedButton.name);

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
            yield return new WaitForSeconds(0.05f);

            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            CheckTheGameIsFinished();
        }
        else
        {
            yield return new WaitForSeconds(0.05f);

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
            Debug.Log("It took you " + countGuesses + " guesses to finish the game.");
        }
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
