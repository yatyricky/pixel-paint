using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip[] KeyNotes;
    public float PlayKeyInterval;
    public float ContinueLastInterval;

    private AudioSource audioSource;
    private bool isPlaying;
    private bool isMute;
    private bool shouldContinueLastKey;
    private int lastKey;
    private bool continueAscending;
    private IEnumerator playLastKeyTimer;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        isMute = false;
        isPlaying = false;
        shouldContinueLastKey = false;
        lastKey = 0;
        continueAscending = true;
    }

    public void PlayOneKey()
    {
        if (isMute == false && isPlaying == false)
        {
            int sel = Mathf.FloorToInt(UnityEngine.Random.Range(0f, 1f) * KeyNotes.Length);
            if (shouldContinueLastKey == true)
            {
                if (lastKey == 0)
                {
                    continueAscending = true;
                }
                else if (lastKey == KeyNotes.Length - 1)
                {
                    continueAscending = false;
                }
                if (continueAscending == true)
                {
                    sel = lastKey + 1;
                } else
                {
                    sel = lastKey - 1;
                }
                if (sel > KeyNotes.Length - 1)
                {
                    sel = KeyNotes.Length - 1;
                }
                StopCoroutine(playLastKeyTimer);
            }
            lastKey = sel;
            audioSource.PlayOneShot(KeyNotes[sel]);
            StartCoroutine(DisallowPlayKeyNote());
            playLastKeyTimer = ShouldContinueWithLastKey();
            StartCoroutine(playLastKeyTimer);
        }
    }

    private IEnumerator ShouldContinueWithLastKey()
    {
        shouldContinueLastKey = true;
        yield return new WaitForSeconds(ContinueLastInterval);
        shouldContinueLastKey = false;
    }

    private IEnumerator DisallowPlayKeyNote()
    {
        isPlaying = true;
        yield return new WaitForSeconds(PlayKeyInterval);
        isPlaying = false;
    }

    internal bool IsMute()
    {
        return isMute;
    }

    internal void Mute(bool v)
    {
        isMute = v;
    }
}
