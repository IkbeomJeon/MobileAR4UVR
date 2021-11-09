using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Newtonsoft.Json;

using KCTM.Network;
using KCTM.Network.Data;

// Class: IntroManager
// This class is manager class for login scene.
public class IntroManager : MonoBehaviour
{
    string uri;

    private string email;
    private string password;

    public GameObject firstPanel;
    public GameObject registerPanel;
    public GameObject loginPanel;
    public GameObject questionairePanel;
    public GameObject resultPanel;

    public GameObject errorPanel;
    public Button loginNextButton;

    public bool enableQuestionnaire;


#if UNITY_EDITOR
    [UnityEditor.MenuItem("PlayerInfo/Clear")]
    public static void ClearPlayerInfo()
    {
        PlayerPrefs.DeleteAll();
    }
#endif
    
    private void OnEnable()
    {
        email = "";
        password = "";
        loginNextButton.interactable = false;
    }


    private void Start()
    {
        uri = ServerURL.Instance.uri;
        //PlayerPrefs.DeleteAll();
        //Screen.orientation = ScreenOrientation.portrait;

        firstPanel.SetActive(true);
        registerPanel.SetActive(false);
        loginPanel.SetActive(false);
        //questionairePanel.SetActive(false);
        //resultPanel.SetActive(false);
        errorPanel.SetActive(false);
        NetworkManager.Instance.basicUri = uri;
        /*
        NetworkManager.Instance.basicUri = uri;

        email = "";
        password = "";

        if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
        {
            email = PlayerPrefs.GetString("email");
            password = PlayerPrefs.GetString("password");
            Login();
        }
        */
    }

    /*
     * Function: ValidateData
     *
     * Details:
     * - This function validates email and password.
     * - Needs update
     */
    private bool ValidateData()
    {
        if (email.Length < 0 ||
            email.IndexOf("@") <= 0)
        {
            return false;
        }
        if (password.Length < 5 ||
            password.Length > 24)
        {
            return false;
        }
        return true;
    }

    /*
     * Function: FirsttoRegister
     *
     * Details:
     * - Button action: switches from very first page to register page.
     */
    public void FirsttoRegister()
    {
        firstPanel.SetActive(false);
        registerPanel.SetActive(true);
        loginPanel.SetActive(false);
        questionairePanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    /*
     * Function: FirsttoLogin
     *
     * Details:
     * - Button action: switches from very first page to login page.
     */
    public void FirsttoLogin()
    {
        firstPanel.SetActive(false);
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        questionairePanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    /*
     * Function: ToQuestionaire
     *
     * Details:
     * - Button action: switch to questionaire page (only if you need questionaire)
     */
    public void ToQuestionaire()
    {
        firstPanel.SetActive(false);
        registerPanel.SetActive(false);
        loginPanel.SetActive(false);
        questionairePanel.SetActive(true);
        resultPanel.SetActive(false);
    }

    /*
    * Function: ResettoQuesitonaire
    *
    * Details:
    * - Button action: qustionaire redo
    */
    public void ResettoQuesitonaire()
    {
        firstPanel.SetActive(false);
        registerPanel.SetActive(false);
        loginPanel.SetActive(false);
        questionairePanel.SetActive(true);
        resultPanel.SetActive(false);
    }

    /*
    * Function: BacktoFirstPage
    *
    * Details:
    * - Button action: back to first page, back arrow button
    */
    public void BacktoFirstPage()
    {
        firstPanel.SetActive(true);
        registerPanel.SetActive(false);
        loginPanel.SetActive(false);
        questionairePanel.SetActive(false);
        resultPanel.SetActive(false);
    }

    /*
    * Function: Login
    *
    * Details:
    * - After getting email/password from user, send server info and login
    * - callback function: LoginCallback(Result result)
    */
    public void Login()
    {

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("email", email);
        data.Add("password", password);

        PlayerPrefs.SetString("email", email);
        PlayerPrefs.SetString("password", password);
        PlayerPrefs.Save();

        NetworkManager.Instance.Post("/user/signin", data, ResponseHandler, LoginCallback, LoginFailHandler);

    }

    /*
    * Function: Login(string eMail, string passWord)
    *
    * Details:
    * - After getting email/password from user, send server info and login
    * - Not used at the moment
    */
    public void Login(string eMail, string passWord)
    {

        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("email", eMail);
        data.Add("password", passWord);

        PlayerPrefs.SetString("email", eMail);
        PlayerPrefs.SetString("password", passWord);
        PlayerPrefs.Save();

        NetworkManager.Instance.Post("/user/signin", data, ResponseHandler, LoginCallback, LoginFailHandler);

    }

    /*
    * Function: LoginCallback
    *
    * Details:
    * - At successful login, start mainscene.
    * - If you need to enable questionaire, change bool of enablequestionnaire variable.
    * - GetFriendList call gets friends list but not used at all.
    */
    private void LoginCallback(Result result)
    {
        AccountInfo.Instance.user = JsonConvert.DeserializeObject<User>(result.result.ToString());

        //var email = PlayerPrefs.GetString("email");
        //var password = PlayerPrefs.GetString("password");

        GetFriendList(AccountInfo.Instance.user.id);

        if (enableQuestionnaire)
        {
            if (!PlayerPrefs.HasKey("tourtype") || !PlayerPrefs.HasKey("contenttype"))
            {
                ToQuestionaire();
            }
            else
                LoadMainScene();
        }
        else
            LoadMainScene();
    }

    /*
     * Function: LoginFailHandler
     *
     * Details:
     * - If login fails, display error message.
     */
    private void LoginFailHandler(Result result)
    {
        errorPanel.SetActive(true);
        errorPanel.transform.Find("Text").gameObject.GetComponent<Text>().text = "Login Failed!";
    }

    /*
     * Function: LoginErrorButton
     *
     * Details:
     * - Button action for closing button on error message.
     * - Calls FadeErrorPanel => mainly aesthetic UI 
     */
    public void LoginErrorButton()
    {
        StartCoroutine(FadeErrorPanel());
    }

    /*
     * Function: IEnumerator FadeErrorPanel
     *
     * Details:
     * - Slowly change alpha of canvas to 0, deactive the canvas, and reset to 1
     */
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

    /*
     * Function: GetFriendList
     *
     * Details:
     * - Gets friends list from server. Not used at the moment
     */
    public void GetFriendList(long id)
    {
        NetworkManager.Instance.Get("/user/friend/list", FriendListHandler, FailureHandler);
    }

    /*
     * Function: LoadMainScene
     *
     * Details:
     * - Change scene to mainscene. Make sure to have mainscene index of 1 in build settings.
     */
    public void LoadMainScene()
    {
#if UNITY_EDITOR
        var oper = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
#else
        var oper = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);   
#endif

        StartCoroutine(LoadScene(oper));
        
    }

    /*
     * Function: FriendListHandler
     *
     * Details:
     * - Callback function to getfriendlist. does nothing at the moment.
     */
    private void FriendListHandler(Result result)
    {
        AccountInfo.Instance.userRelationships = JsonConvert.DeserializeObject<List<User>>(result.result.ToString());
    }

    /*
     * Function: LoadScene
     *
     * Details:
     * - Support function for LoadMainScene
     */
    private IEnumerator LoadScene(AsyncOperation oper)
    {
        while(!oper.isDone)
        {
            yield return null;
        }

    }

    private void ResponseHandler()
    {
        
    }

    private Result ResponseToResult(byte[] response)
    {
        string responseString = Encoding.UTF8.GetString(response);
        Result result = JsonConvert.DeserializeObject<Result>(responseString);

        return result;
    }

    private void FailureHandler(Result result)
    {

    }

    /*
     * Function: SetEmail
     *
     * Details:
     * - Gets email from inputfield and save it to email variable
     */
    public void SetEmail(InputField email)
    {
        this.email = email.text;
        loginNextButton.interactable = ValidateData();
    }

    /*
     * Function: SetPassword
     *
     * Details:
     * - Gets password from inputfield and save it to password variable
     */
    public void SetPassword(InputField password)
    {
        this.password = password.text;
        loginNextButton.interactable = ValidateData();
    }
}
