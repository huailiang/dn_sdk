﻿using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using UnityEngine.Networking;
using LiteWebView;

public class Main : MonoBehaviour
{

    const string gif = "https://timgsa.baidu.com/timg?image&quality=80&size=b9999_10000&sec=1497885550184&di=48b9289901fcd8c5dfa773873d9e6b00&imgtype=0&src=http%3A%2F%2Fimage.manyule.com%2Fbbs%2Fdata%2Fattachment%2Fforum%2F201508%2F04%2F160054selefffv3cgxfelx.gif";
    private static string text = "hello world";
    public GameObject m_imgObj;
    private string texname;
    private Texture2D m_Tex;
    public Text m_info;
    private string tex;
    private WebView webView;

    private GUIStyle statu;

    void Start()
    {
        Debug.Log(Application.streamingAssetsPath);

        string path = Application.streamingAssetsPath + "/test.txt";
        webView = gameObject.AddComponent<WebView>();
        webView.Init();

        statu = new GUIStyle();
        statu.fontSize = 32;

#if UNITY_ANDROID && !UNITY_EDITOR
       // do nothing
#else
        var txt = File.ReadAllText(path);
        Debug.Log(txt);
#endif
    }


    private void OnGUI()
    {
        GUI.Label(new Rect(210, 10, 100, 80), text, statu);
        if (GUI.Button(new Rect(60, 10, 140, 40), "像素密度"))
        {
            int density = NativeBridge.sington.NGetDensity();
            m_info.text = "PPI:" + density;
        }
        if (GUI.Button(new Rect(60, 70, 140, 40), "运营商"))
        {
            string str = NativeBridge.sington.NCheckSIM();
            m_info.text = "运营商：" + str;
        }
        if (GUI.Button(new Rect(60, 130, 140, 40), "加载gif"))
        {
            StartCoroutine(GifHander());
        }
#if UNITY_ANDROID
        if (GUI.Button(new Rect(60, 190, 140, 40), "NDK"))
        {
            Debug.Log(Android.GetMemory());
            NDKRead("test.txt");
        }
        if (GUI.Button(new Rect(60, 250, 140, 40), "Streaming"))
        {
            string file = "test.txt";
            int code = Android.CopyStreamingAsset(file);
            Debug.Log("code: " + code);
            string txt = Android.LoadSteamString(file);
            Debug.Log(txt);
        }
        if (GUI.Button(new Rect(60, 310, 140, 40), "UnZip"))
        {
            Android.UnZip("lua.zip");
        }
#endif
        GUIWebview();
    }


    private void GUIWebview()
    {
        float left = 200;
        tex = GUI.TextField(new Rect(left, 100, 360, 60), tex);
        left += 200;
        if (GUI.Button(new Rect(left + 200, 100, 120, 70), "load"))
        {
            webView.Show(200, 0, 0, 0);
            webView.LoadUrl(tex);
        }
        if (GUI.Button(new Rect(left + 150, 10, 100, 70), "loadhtm"))
        {
            webView.RegistJsInterfaceAction("ShowMsg", ShowMsg);
            webView.Show(160, 0, 0, 0);
            webView.LoadLocal("/litewebview_test.html");
        }
        if (GUI.Button(new Rect(left + 300, 10, 100, 80), "calljs"))
        {
            webView.CallJS("callJS", "https://youku.com/");
        }
        if (GUI.Button(new Rect(left + 450, 10, 100, 80), "close"))
        {
            webView.Close();
        }
    }

    IEnumerator GifHander()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(gif))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                string path = Application.temporaryCachePath + "/" + gif.GetHashCode() + ".jpg";
                Debug.Log("save path:" + path);
                
                NativeBridge.sington.NToJPG(path, www.downloadHandler.data);
                Debug.Log("trans jpg finish! path:" + path);

                m_Tex = new Texture2D(100, 100);
#if UNITY_EDITOR
                m_Tex = Resources.Load<Texture2D>("Icon-180");
#else
                m_Tex.LoadImage(File.ReadAllBytes(path));
#endif
                Sprite tempSprite = Sprite.Create(m_Tex, new Rect(0, 0, m_Tex.width, m_Tex.height), Vector2.zero);
                m_imgObj.GetComponent<Image>().sprite = tempSprite;
            }

        }
    }


    public void NDKRead(string file)
    {
#if UNITY_ANDROID
        Debug.Log("add result: " + NativeRead.Add(2, 45));

        MemoryStream stream = new MemoryStream();
        var ptr = IntPtr.Zero;
        int size = NativeRead.ReadAssetsBytes(file, ref ptr);
        Debug.Log("native size: " + size);
        if (size > 0)
        {
            if (ptr == IntPtr.Zero)
            {
                Debug.LogError("read failed!");
            }
            else
            {
                stream.SetLength(size);
                stream.Position = 0;
                Marshal.Copy(ptr, stream.GetBuffer(), 0, size);
                var reader = new BinaryReader(stream);
                Debug.Log(reader.ReadString());
                NativeRead.ReleaseBytes(ptr);
            }
        }
#endif
    }

    void ShowMsg(string info)
    {
        text = info;
    }

}
