using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Class: ThreeButtonController
// This class is used for three button selection for questionaire.
public class ThreeButtonController : MonoBehaviour
{
    public GameObject option1;
    public GameObject option2;
    public GameObject option3;

    public Text text1;
    public Text text2;
    public Text text3;

    private bool option1Flag = false;
    private bool option2Flag = false;
    private bool option3Flag = false;

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
            //text1.color = new Color32(10, 172, 254, 255);
            option2.GetComponent<Button>().image.sprite = deselected;
            //text2.color = new Color32(155, 155, 155, 255);
            option3.GetComponent<Button>().image.sprite = deselected;
            //text3.color = new Color32(155, 155, 155, 255);

            option1Flag = true;
            option2Flag = false;
            option3Flag = false;
        }
        else
        {
            option1.GetComponent<Button>().image.sprite = deselected;
            //text1.color = new Color32(155, 155, 155, 255);
            option2.GetComponent<Button>().image.sprite = deselected;
            //text2.color = new Color32(155, 155, 155, 255);
            option3.GetComponent<Button>().image.sprite = deselected;
            //color = new Color32(155, 155, 155, 255);

            option1Flag = false;
            option2Flag = false;
            option3Flag = false;
        }
    }

    public void option2Select()
    {
        if (!option2Flag)
        {
            option1.GetComponent<Button>().image.sprite = deselected;
            //text1.color = new Color32(155, 155, 155, 255);
            option2.GetComponent<Button>().image.sprite = selected;
            //text2.color = new Color32(10, 172, 254, 255);
            option3.GetComponent<Button>().image.sprite = deselected;
            //text3.color = new Color32(155, 155, 155, 255);

            option1Flag = false;
            option2Flag = true;
            option3Flag = false;
        }
        else
        {
            option1.GetComponent<Button>().image.sprite = deselected;
            //text1.color = new Color32(155, 155, 155, 255);
            option2.GetComponent<Button>().image.sprite = deselected;
            //text2.color = new Color32(155, 155, 155, 255);
            option3.GetComponent<Button>().image.sprite = deselected;
            //text3.color = new Color32(155, 155, 155, 255);

            option1Flag = false;
            option2Flag = false;
            option3Flag = false;
        }
    }

    public void option3Select()
    {
        if (!option3Flag)
        {
            option1.GetComponent<Button>().image.sprite = deselected;
            //text1.color = new Color32(155, 155, 155, 255);
            option2.GetComponent<Button>().image.sprite = deselected;
            //text2.color = new Color32(155, 155, 155, 255);
            option3.GetComponent<Button>().image.sprite = selected;
            //text3.color = new Color32(10, 172, 254, 255);

            option1Flag = false;
            option2Flag = false;
            option3Flag = true;
        }
        else
        {
            option1.GetComponent<Button>().image.sprite = deselected;
            //text1.color = new Color32(155, 155, 155, 255);
            option2.GetComponent<Button>().image.sprite = deselected;
            //text2.color = new Color32(155, 155, 155, 255);
            option3.GetComponent<Button>().image.sprite = deselected;
            //text3.color = new Color32(155, 155, 155, 255);

            option1Flag = false;
            option2Flag = false;
            option3Flag = false;
        }
    }
}
