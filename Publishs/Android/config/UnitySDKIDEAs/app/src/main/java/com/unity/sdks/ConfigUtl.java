package com.unity.sdks;

import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.webkit.WebView;
import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.Map;

public class ConfigUtl {
    public  static  String UnityListener;
    public  static WebView webView;

    public  static String getMetaData(String key)
    {
        try {
            ApplicationInfo ai = UnityPlayer.currentActivity.getPackageManager().getApplicationInfo(UnityPlayer.currentActivity.getPackageName(), PackageManager.GET_META_DATA);
            Bundle bundle = ai.metaData;
            return bundle.getString(key);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }
}
