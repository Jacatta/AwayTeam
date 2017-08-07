using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class ImageFade : MonoBehaviour
{

    public Image img;
    public Animator anim;
    SceneLoader sceneL;
    //private GameObject Vader;

    private void Awake()
    {
        
 
        DontDestroyOnLoad(this);
        //Vader = GameObject.Find("LordFader");
        // img.GetComponent<Image>().enabled = false;

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            //Vader.SetActive(true);
            //img.GetComponentInParent<GameObject>().SetActive(true);
            FadetoQuit();
        }


    }


    public void Fadeto(int newscene)
    {

        if (Time.timeScale == 0)
            Time.timeScale = 1;

        Debug.Log("OK...FadeoutCalled");
        StartCoroutine(Fading(newscene));
        
        sceneL.quit();
    }

    public void FadetoQuit()
    {
        if (Time.timeScale == 0)
            Time.timeScale = 1;


        Debug.Log("OK...FadeoutCalled");
        StartCoroutine(Fading(0));

        sceneL.quit();

    }


    IEnumerator Fading(int scene)
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => img.color.a == 1);
        SceneManager.LoadScene(scene);
    }

}

