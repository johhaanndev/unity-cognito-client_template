using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using Assets.Scripts.Utilities;

namespace Assets.Scripts.UI
{
    // Manages all the text and button inputs
    // Also acts like the main manager script for the game.
    public class UIInputManager : MonoBehaviour
    {
        public static string CachePath;

        [Header("Login")]
        public TMP_InputField emailFieldLogin;
        public TMP_InputField passwordFieldLogin;
        public Button loginButton;
        public Button switchToRegister;

        [Header("Register")]
        public TMP_InputField usernameRegisterField;
        public TMP_InputField emailRegisterField;
        public TMP_InputField passwordRegisterField;
        public Button signupButton;
        public Button switchToLoginButton;

        [Header("Auth Interface")]
        public Button startButton;
        public Button logoutButton;

        private AuthenticationManager _authenticationManager;
        private GameObject _loginInterface;
        private GameObject _registerInterface;
        private GameObject _authenticatedInterface;
        private GameObject _loading;
        private GameObject _welcome;
        private GameObject _confirmEmail;
        private FieldsTabulation tabulation;

        private void displayComponentsFromAuthStatus(bool loggedIn)
        {
            if (loggedIn)
            {
                // User authenticated, show welcome screen with options
                _loading.SetActive(false);
                _loginInterface.SetActive(false);
                _registerInterface.SetActive(false);
                _authenticatedInterface.SetActive(true);
                _welcome.SetActive(true);
            }
            else
            {
                // User not authenticated, show login scene
                _loading.SetActive(false);
                _loginInterface.SetActive(true);
                _authenticatedInterface.SetActive(false);
            }

            // clear out passwords
            passwordFieldLogin.text = string.Empty;
            passwordRegisterField.text = string.Empty;

            // set focus to email field on login form
            tabulation.SetFieldIndex(-1);
        }

        private async void onLoginClicked()
        {
            _loginInterface.SetActive(false);
            _loading.SetActive(true);
            _confirmEmail.SetActive(false);

            bool successfulLogin = await _authenticationManager.Login(emailFieldLogin.text, passwordFieldLogin.text);
            displayComponentsFromAuthStatus(successfulLogin);

        }

        private async void onSignupClicked()
        {
            _loginInterface.SetActive(false);
            _loading.SetActive(true);

            bool successfulSignup = await _authenticationManager.Signup(usernameRegisterField.text, emailRegisterField.text, passwordRegisterField.text);

            if (successfulSignup)
            {
                // here we disable the register interface and show the email confirmation advise
                _registerInterface.SetActive(false);
                _confirmEmail.SetActive(true);

                // copy over the new credentials to make the process smoother
                emailFieldLogin.text = emailRegisterField.text;
                passwordFieldLogin.text = passwordRegisterField.text;

                // set focus to email field on login form
                tabulation.SetFieldIndex(0);
            }
            else
            {
                _confirmEmail.SetActive(false);

                // set focus to email field on signup form
                tabulation.SetFieldIndex(3);
            }

            _loading.SetActive(false);
            _loginInterface.SetActive(true);
        }

        private void onLogoutClick()
        {
            _authenticationManager.SignOut();
            displayComponentsFromAuthStatus(false);
        }

        private void onStartClick()
        {
            SceneManager.LoadScene("GameScene");
            Debug.Log("Changed to GameScene");
        }


        private void onSwitchToRegister()
        {
            _loginInterface.SetActive(false);
            _registerInterface.SetActive(true);
        }

        private void onSwitchToLogin()
        {
            _registerInterface.SetActive(false);
            _loginInterface.SetActive(true);
        }

        private async void RefreshToken()
        {
            bool successfulRefresh = await _authenticationManager.RefreshSession();
            displayComponentsFromAuthStatus(successfulRefresh);
        }

        void Start()
        {
            Debug.Log("UIInputManager: Start");
            // check if user is already authenticated 
            // We perform the refresh here to keep our user's session alive so they don't have to keep logging in.
            RefreshToken();

            signupButton.onClick.AddListener(onSignupClicked);
            loginButton.onClick.AddListener(onLoginClicked);
            startButton.onClick.AddListener(onStartClick);
            logoutButton.onClick.AddListener(onLogoutClick);

            switchToRegister.onClick.AddListener(onSwitchToRegister);
            switchToLoginButton.onClick.AddListener(onSwitchToLogin);
        }

        void Awake()
        {
            CachePath = Application.persistentDataPath;

            tabulation = GetComponent<FieldsTabulation>();

            // Heriarchy objects references
            _loginInterface = GameObject.Find("LoginInterface");
            _registerInterface = GameObject.Find("RegisterInterface");
            _authenticatedInterface = GameObject.Find("AuthInterface");
            _loading = GameObject.Find("Loading");
            _welcome = GameObject.Find("Welcome");
            _confirmEmail = GameObject.Find("ConfirmEmail");

            _loginInterface.SetActive(false); // start as false so we don't just show the login screen during attempted token refresh
            _registerInterface.SetActive(false);
            _authenticatedInterface.SetActive(false);
            _welcome.SetActive(false);
            _confirmEmail.SetActive(false);
            _loginInterface.SetActive(true);

            _authenticationManager = FindObjectOfType<AuthenticationManager>();

            //_lambdaManager = FindObjectOfType<LambdaManager>();
        }
    }
}