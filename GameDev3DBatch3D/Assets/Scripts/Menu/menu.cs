using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class menu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartButton(string scenename)
    {
        SceneManager.LoadScene(scenename);
    }


    public void QuitButton()
    {
        Application.Quit();
    }
}
