using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Class: TwoButtonController
// This class is used for two button selection for questionaire.
public class TwoButtonController : MonoBehaviour
{
    public GameObject option1;
    public GameObject option2;

    public Text text1;
    public Text text2;

    private bool option1Flag = false;
    private bool option2Flag = false;

    private Sprite selected;
    private Sprite deselected;

    private void Start()
    {
        Texture2D selectedTexture = Resources.Load<Texture2D>("UI/Login/optionSelected");
        selected = Sprite.Create(selectedTexture, new Rect(0, 0, selectedTexture.width, selectedTexture.height), Vector2.one * 0.5f, 100f);
        Texture2D deselectedTexture = Resources.Load<Texture2D>("UI/Login/option");
        deselected = Sprite.Create(deselectedTexture, new Rect(0, 0, deselectedTexture.width, deselectedTexture.height), Vector2.one * 0.5f, 100f);
    }

    public void option1Select()
    {
        if (!option1Flag)
        {
            option1.GetComponent<Button>().image.sprite = selected;
            text1.color = new Color32(255, 255, 255, 255);
            option2.GetComponent<Button>().image.sprite = deselected;
            text2.color = new Color32(67, 176, 120, 255);

            option1Flag = true;
            option2Flag = false;
        }
        else
        {
            option1.GetComponent<Button>().image.sprite = deselected;
            text1.color = new Color32(67, 176, 120, 255);
            option2.GetComponent<Button>().image.sprite = deselected;
            text2.color = new Color32(67, 176, 120, 255);

            option1Flag = false;
            option2Flag = false;
        }
    }

    public void option2Select()
    {
        if (!option2Flag)
        {
            option1.GetComponent<Button>().image.sprite = deselected;
            text1.color = new Color32(67, 176, 120, 255);
            option2.GetComponent<Button>().image.sprite = selected;
            text2.color = new Color32(255, 255, 255, 255);

            option1Flag = false;
            option2Flag = true;
        }
        else
        {
            option1.GetComponent<Button>().image.sprite = deselected;
            text1.color = new Color32(67, 176, 120, 255);
            option2.GetComponent<Button>().image.sprite = deselected;
            text2.color = new Color32(67, 176, 120, 255);

            option1Flag = false;
            option2Flag = false;
        }
    }
}
