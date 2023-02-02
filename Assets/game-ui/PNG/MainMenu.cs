using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /*// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    public void StartButton_Click()
    {
        SceneManager.LoadScene("Game");
    }

    public void RegisterButton_Click()
    {

    }

   /* public void RegisterButton_Click()
    {
        MenuCanvas.SetActive(false);
        RegisterCanvas.SetActive(true);
    }*/

}
