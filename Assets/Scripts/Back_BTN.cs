using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back_BTN : MonoBehaviour
{
    // Start is called before the first frame update
 public  void Back_Main()
    {
        SceneManager.LoadScene("Main");
    }
}
