using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Login : MonoBehaviour
{
    public void login()
    {
        SceneManager.LoadScene(0);
    }

    public void LoginMain()
    {
        SceneManager.LoadScene(1);
    }
}
