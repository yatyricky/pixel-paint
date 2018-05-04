using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitScreenSize : MonoBehaviour
{
    public float TimeToLoadHallScene;

    private void Awake()
    {
        float ratio = (float)Screen.width / Screen.height;
        if (Mathf.Abs(ratio - Configs.STANDARD_RATIO) > Configs.BIG_EPSILON)
        {
            Configs.SCREEN_WIDTH = Configs.DESIGN_HEIGHT * ratio;
            Configs.SHOULD_CHANGE_WIDTH = true;
            Configs.LEVEL_WIDTH = Configs.LEVEL_DESIGN_WIDTH_RATIO * Configs.SCREEN_WIDTH;
            Configs.LEVEL_HEIGHT = Configs.LEVEL_WIDTH / Configs.LEVEL_RATIO;
            Configs.LEVEL_MARGIN = Configs.SCREEN_WIDTH * ((1f - Configs.LEVEL_DESIGN_WIDTH_RATIO * 2f) / 3f);
        }
    }

    private void Start()
    {
        StartCoroutine(LoadHallScene());
        Screen.fullScreen = true;
    }

    private IEnumerator LoadHallScene()
    {
        yield return new WaitForSeconds(TimeToLoadHallScene);
        SceneManager.LoadScene("Hall");
    }
}
