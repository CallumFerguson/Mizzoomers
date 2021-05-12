using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;


public class FirebaseManager : MonoBehaviour
{
    // Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    // Login variables
    [Header("Login")]
    public InputField usernameField;
    public InputField emailField;
    public InputField passwordField;
    public Text warningLoginText;

    //warningLoginText.text = "";

    private void Awake()
    {
        // Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // if they are able to initialize firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        // set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    // function for login
    public void PlayButton()
    {
        StartCoroutine(Play(usernameField.text, emailField.text, passwordField.text));
    }

    private IEnumerator Play(string _username, string _email, string _password)
    {
        if (_username == "")
        {
            warningLoginText.text = "Missing Username";
        }
        else if (_password == "")
        {
            warningLoginText.text = "Missing password";
        }
        else if (_email == "")
        {
            warningLoginText.text = "Missing email";
        }
        else if (_username == "" && _password == "")
        {
            warningLoginText.text = "Missing Username and Password";
        }
        else if (_username == "" && _email == "")
        {
            warningLoginText.text = "Missing Username and Email";
        }
        else if (_password == "" && _email == "")
        {
            warningLoginText.text = "Missing Password and Email";
        }
        else if (_username == "" &&_password == "" && _email == "")
        {
            warningLoginText.text = "Please enter in your information";
        }
        else
        {
            // call the Firebase auth signin function passing the email and password
            var playTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            // wait until task completes
            yield return new WaitUntil(predicate: () => playTask.IsCompleted);

            if(playTask.Exception != null)
            {
                // error handling
                Debug.LogWarning(message: $"Failed to play task with {playTask.Exception}");
                FirebaseException firebaseEx = playTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Play Failed";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Username";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Username Already in Use";
                        break;
                }
                warningLoginText.text = message;
            }
            else
            {
                // user has now been created
                User = playTask.Result;

                if(User != null)
                {
                    // create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    // call the Firebase auth update user profile function passing the profile with the username
                    var profileTask = User.UpdateUserProfileAsync(profile);

                    // wait until the task completes
                    yield return new WaitUntil(predicate: () => profileTask.IsCompleted);

                    if(profileTask.Exception != null)
                    {
                        // error handling
                        Debug.LogWarning(message: $"Failed to create profile with {profileTask.Exception}");
                        FirebaseException firebaseEx = profileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                    }
                }
                warningLoginText.text = "Success";
            }
        }


        
        
    }
    
    
}
