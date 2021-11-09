using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KCTM.Network;
using KCTM.Network.Data;

// Class: RegisterController
// This class is used to register new user and login
// You do not have to have all the information to register.
public class RegisterController : MonoBehaviour
{
    private string email;
    private string password;
    private string username;
    private string accountName;
    private string language;
    private string age;
    private string country;

    [SerializeField]
    private GameObject firstPage;
    [SerializeField]
    private GameObject questionairePanel;
    [SerializeField]
    private GameObject errorPanel;

    [SerializeField]
    private IntroManager introManager;

    [SerializeField]
    private Button registerButton;
    
    private void Start()
    {
        language = "KO";
        country = "KR";
    }

    private void OnEnable()
    {
        email = "";
        password = "";
        username = "";
        accountName = "";
        language = "";
        age = "";
        country = "";
        registerButton.interactable = false;
    }

    public void BacktoFirstPage()
    {
        firstPage.SetActive(true);
        gameObject.SetActive(false);
    }

    private bool ValidateData()
    {
        if(email.Length < 0 ||
            email.IndexOf("@") <= 0)
        {
            return false;
        }
        if (password.Length < 5 ||
            password.Length > 24)
        {
            return false;
        }
        /*
        if (accountName.Length <= 0)
        {
            return false;
        }
        */
        if (username.Length <= 0)
        {
            return false;
        }
        /*
        if (language.Length <= 0)
        {
            return false;
        }
        if (country.Length <= 0)
        {
            return false;
        }
        */
        return true;
    }

    public void ChangeEmail(InputField email)
    {
        this.email = email.text;
        registerButton.interactable = ValidateData();
    }

    public void ChangePassword(InputField password)
    {
        this.password = password.text;
        registerButton.interactable = ValidateData();
    }

    public void ChangeUsername(InputField username)
    {
        this.username = username.text;
        registerButton.interactable = ValidateData();
    }

    public void ChangeName(InputField accountName)
    {
        this.accountName = accountName.text;
        registerButton.interactable = ValidateData();
    }

    public void ChangeLanguage(InputField language)
    {
        this.language = language.text;
        registerButton.interactable = ValidateData();
    }

    public void ChangeAge(InputField age)
    {
        this.age = age.text;
        registerButton.interactable = ValidateData();
    }

    public void ChangeCountry(InputField country)
    {
        this.country = country.text;
        registerButton.interactable = ValidateData();
    }

    public void Register()
    {
        Texture2D defaultProfileImage = Resources.Load<Texture2D>("ProfileImages/default");
        byte[] bytes = defaultProfileImage.EncodeToPNG();

        WWWForm formData = new WWWForm();
        formData.AddField("email", email);
        formData.AddField("password", password);
        formData.AddField("name", "");
        formData.AddField("username", username);
        formData.AddField("age", "");
        formData.AddField("type", "NORMAL");
        formData.AddBinaryData("profileimage", bytes, "default.png");


        NetworkManager.Instance.Post("/user/signup", formData, ResponseHandler, RegisterHandler, FailureHandler);
    }

    private void ResponseHandler()
    {

    }

    private void RegisterHandler(Result result)
    {
        introManager.Login(email, password);
        gameObject.SetActive(false);
    }

    private void FailureHandler(Result result)
    {
        Debug.LogError(result.error + " : " + result.msg);
        errorPanel.SetActive(true);
        errorPanel.transform.Find("Text").gameObject.GetComponent<Text>().text = "Registration Failed. Please check your information.";
    }

    public void RegitrationErrorButton()
    {
        StartCoroutine(FadeErrorPanel());
    }

    IEnumerator FadeErrorPanel()
    {
        CanvasGroup canvasGroup = errorPanel.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }
        errorPanel.SetActive(false);
        canvasGroup.alpha = 1;
        yield return null;
    }
}
