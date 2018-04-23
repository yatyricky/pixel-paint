using UnityEngine;
using System.Collections;

public class MorePage : MonoBehaviour
{

    public void OnStoreClicked()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=app.smalltricks.picsart");
#endif
    }

    public void OnClickShare()
    {
#if UNITY_ANDROID
        // Create Refernece of AndroidJavaClass class for intent
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        // Create Refernece of AndroidJavaObject class intent
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        // Set action for intent
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

        intentObject.Call<AndroidJavaObject>("setType", "text/plain");

        //Set Subject of action
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "Check out Pixel Art");
        //Set title of action or intent
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TITLE"), "Check out Pixel Art");
        // Set actual data which you want to share
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Hi, I've found this great relaxing app. https://play.google.com/store/apps/details?id=app.smalltricks.picsart");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        // Invoke android activity for passing intent to share data
        currentActivity.Call("startActivity", intentObject);
#endif
    }

}
