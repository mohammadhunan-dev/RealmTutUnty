using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AuthenticationManager : MonoBehaviour
{

    public static VisualElement root;
    public static Label loginOrRegisterSubtitle;
    public static Button loginOrRegisterAndStartButton;
    public static Button toggleLoginOrRegisterUIButton;
    public static bool isShowingRegisterUI = false;
    public static Realms.Sync.User loggedInUser;
    public static TextField userInput;
    public static TextField passInput;


    // Start is called before the first frame update
    void Start()
    {

        root = GetComponent<UIDocument>().rootVisualElement;

        loginOrRegisterSubtitle = root.Q<Label>("login-or-register-subtitle");
        loginOrRegisterAndStartButton = root.Q<Button>("login-or-register-and-start-button");
        toggleLoginOrRegisterUIButton = root.Q<Button>("toggle-login-or-register-ui-button");

        userInput = root.Q<TextField>("username-input");
        passInput = root.Q<TextField>("password-input");
        passInput.isPasswordField = true;


        toggleLoginOrRegisterUIButton.clicked += () =>
        {
            // if the registerUI is already visible, switch to the loginUI and set isShowingRegisterUI to false
            if (isShowingRegisterUI == true) 
            {
                switchToLoginUI();
                isShowingRegisterUI = false;
            }
            else
            {
                switchToRegisterUI();
                isShowingRegisterUI = true;
            }
        };

        loginOrRegisterAndStartButton.clicked += async () =>
        {
            if (isShowingRegisterUI == true)
            {
                onPressRegister();
            }
            else
            {
                onPressLogin();
            }


        };
    }

    public static void switchToLoginUI()
    {
        loginOrRegisterSubtitle.text = "Login";
        loginOrRegisterAndStartButton.text = "Login & Start Game";
        toggleLoginOrRegisterUIButton.text = "Don't have an account yet? Register";
    }

    public static void switchToRegisterUI()
    {
        loginOrRegisterSubtitle.text = "Register";
        loginOrRegisterAndStartButton.text = "Signup & Start Game";
        toggleLoginOrRegisterUIButton.text = "Have an account already? Login";
    }

    public static async void onPressRegister()
    {
        try
        {
            loggedInUser = await RealmController.Instance.OnPressRegister(userInput.value, passInput.value);

            if (loggedInUser != null)
            {
                root.AddToClassList("hide");
            }

        }
        catch (Exception ex)
        {
            Debug.Log("an exception was thrown:" + ex.Message);
        }
    }

    public static async void onPressLogin()
    {
        try
        {
            loggedInUser = await RealmController.Instance.OnPressLogin(userInput.value, passInput.value);

            if (loggedInUser != null)
            {
                root.AddToClassList("hide");
            }

        }
        catch (Exception ex)
        {
            Debug.Log("an exception was thrown:" + ex.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
