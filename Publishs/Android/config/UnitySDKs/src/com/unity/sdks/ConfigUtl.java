package com.unity.sdks;

import android.content.pm.ApplicationInfo;
import android.content.pm.PackageManager;
import android.os.Bundle;
import android.util.Log;
import android.webkit.WebView;
import com.unity3d.player.UnityPlayer;

import java.util.HashMap;
import java.util.Map;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
public class ConfigUtl {
    public  static  String UnityListener;
    static Map<String, String> UserRuntimeDatas;

    public  static String getMetaData(String key)
    {
        try {
            ApplicationInfo ai = UnityPlayer.currentActivity.getPackageManager().getApplicationInfo(UnityPlayer.currentActivity.getPackageName(), PackageManager.GET_META_DATA);
            Bundle bundle = ai.metaData;
            if(bundle != null)
            {
            	if(bundle.containsKey(key))
            	{
            		Object objValue = bundle.get(key);
            		if(objValue!=null) return objValue.toString();
            		else Log.e("UnitySDK", "no meta key:"+key+"= null");
            	}
            	else
            	{
            		Log.e("UnitySDK", "no meta key:" + key);
            	}
            }
            else Log.e("UnitySDK", "meta bundle null");
            return null;
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }
    
    public static boolean HasMetaData(String key)
    {
        try {
            ApplicationInfo ai = UnityPlayer.currentActivity.getPackageManager().getApplicationInfo(UnityPlayer.currentActivity.getPackageName(), PackageManager.GET_META_DATA);
            Bundle bundle = ai.metaData;
            if(bundle != null)
            {
            	if(bundle.containsKey(key))
            	{
            		return true;
            	}
            	else
            	{
            		return false;
            	}
            }
            return false;
        } catch (Exception e) {
            e.printStackTrace();
        }
        return false;
    }
    
    public  static int getMetaDataInt(String key)
    {
        try {
            ApplicationInfo ai = UnityPlayer.currentActivity.getPackageManager().getApplicationInfo(UnityPlayer.currentActivity.getPackageName(), PackageManager.GET_META_DATA);
            Bundle bundle = ai.metaData;
            if(bundle != null)
            {
            	if(bundle.containsKey(key))
            	{
            		return bundle.getInt(key);
            	}
            	else
            	{
            		return 0;
            	}
            }
            return 0;
        } catch (Exception e) {
            e.printStackTrace();
        }
        return 0;
    }
    
    public  static String getRuntimeData(String key)
    {
        try {
        	if(UserRuntimeDatas == null) return null;
        	if(UserRuntimeDatas.containsKey(key))
        		return UserRuntimeDatas.get(key);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }
    
    public static void setRuntimeData(String key, String value)
    {
        try {
        	if(UserRuntimeDatas == null) UserRuntimeDatas = new HashMap<String,String>();
        	UserRuntimeDatas.put(key, value);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
