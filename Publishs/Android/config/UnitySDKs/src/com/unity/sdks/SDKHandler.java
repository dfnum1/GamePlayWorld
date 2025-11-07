package com.unity.sdks;

import android.app.AlertDialog;
import android.content.DialogInterface;
import android.util.Log;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.RelativeLayout;
import com.nearme.game.sdk.GameCenterSDK;
import com.nearme.game.sdk.callback.GameExitCallback;
import com.unity.sdks.admobile.AdmobileHandler;
import com.unity.sdks.tobid.TobidHandler;
import com.unity.sdks.huawei.HuaweiHandler;
import com.unity.sdks.oppo.OppoHandler;
import com.unity.sdks.sdk233.Handler233;
import com.unity.sdks.vivo.VivoConfig;
import com.unity.sdks.vivo.VivoHandler;
import com.unity.sdks.zs.ZsHandler;
import com.unity.sdks.yingyongbao.YingyongbaoHandler;
import com.unity3d.player.UnityPlayer;
import org.json.JSONObject;

public class SDKHandler
{
	public static void SetRuntimeData(final String key, final String value)
	{
		ConfigUtl.setRuntimeData(key, value);
	}
	
    public  static void SetListener(final  String name)
    {
        ConfigUtl.UnityListener = name;
    }
    public  static boolean Init(final String sdkChannel, final  String jsonSetting)
    {
        try
        {
            if(sdkChannel.compareTo("oppo") == 0)
            {
            	JSONObject jsonObject = new JSONObject(jsonSetting);
                if(jsonObject.has("appSecret")) {
                    OppoHandler.Init(jsonObject.getString("appSecret"),ConfigUtl.UnityListener);
                    return  true;
                }
            }
            else if(sdkChannel.compareTo("vivo") == 0)
            {
            	JSONObject jsonObject = new JSONObject(jsonSetting);
                if(jsonObject.has("appId") && jsonObject.has("appKey"))
                {
                    String cpId = jsonObject.has("cpId")?jsonObject.getString("cpId"):"";
                    boolean passPrivacy = jsonObject.has("passPrivacy")?jsonObject.getBoolean("passPrivacy") : false;
                    boolean debug = jsonObject.has("debug")?jsonObject.getBoolean("debug"):false;
                    VivoHandler.Init(jsonObject.getString("appId"), jsonObject.getString("appKey"), cpId,passPrivacy,debug);
                    if(jsonObject.has("payBackUtl"))
                        VivoConfig.payBackUtl = jsonObject.getString("payBackUtl");
                    return  true;
                }
                else
                    Log.e("UnitySDK", "vivo no appId and appKey:");
            }
            else if(sdkChannel.compareTo("yingyongbao") == 0)
            {
                YingyongbaoHandler.Init();
                return  true;
            }
            else if(sdkChannel.compareTo("admobile") == 0)
            {
                return AdmobileHandler.Init();
            }
            else if(sdkChannel.compareTo("huawei") == 0)
            {
                HuaweiHandler.Init();
                return  true;
            }
            else if(sdkChannel.compareTo("tobid") == 0)
            {
                return TobidHandler.Init();
            }
            else if(sdkChannel.compareTo("zs") == 0)
            {
                return ZsHandler.Init();
            }
            else if(sdkChannel.compareTo("233") == 0)
            {
                return Handler233.Init();
            }
            Log.e("UnitySDK", "no sdk config:".concat(sdkChannel));
            return  false;
        }
        catch (Exception e)
        {
            Log.e("UnitySDK", e.toString());
        }
        return  false;
    }
    public  static void Login(final  String sdkChannel, final String json)
    {
        try
        {
            if(sdkChannel.compareTo("oppo") == 0)
            {
                OppoHandler.Login(ConfigUtl.UnityListener);
                return;
            }
            else if(sdkChannel.compareTo("vivo") == 0)
            {
                VivoHandler.Login();
                return;
            }
            else if(sdkChannel.compareTo("yingyongbao") == 0)
            {
                YingyongbaoHandler.Login(json);
                return;
            }
            else if(sdkChannel.compareTo("huawei") == 0)
            {
                HuaweiHandler.Login();
                return;
            }
            else if(sdkChannel.compareTo("zs") == 0)
            {
                ZsHandler.Login();
                return;
            }
            else if(sdkChannel.compareTo("233") == 0)
            {
            	Handler233.Login();
                return;
            }
            Log.e("UnitySDK", "no sdk config:".concat(sdkChannel));
            return;
        }
        catch (Exception e)
        {
            Log.e("UnitySDK", e.toString());
        }
    }

    public  static void Logout(final  String sdkChannel)
    {
        try
        {
            if(sdkChannel.compareTo("oppo") == 0)
            {
                OppoHandler.ExitSdk(ConfigUtl.UnityListener);
                return;
            }
            else if(sdkChannel.compareTo("vivo") == 0)
            {
                VivoHandler.Exit();
                return;
            }
            else if(sdkChannel.compareTo("yingyongbao") == 0)
            {
                YingyongbaoHandler.Logout();
                return;
            }
            else if(sdkChannel.compareTo("huawei") == 0)
            {
                HuaweiHandler.Logout();
                return;
            }
            else if(sdkChannel.compareTo("zs") == 0)
            {
                ZsHandler.Logout();
                return;
            }
            else if(sdkChannel.compareTo("233") == 0)
            {
            	Handler233.Logout();
                return;
            }
            Log.e("UnitySDK", "no sdk config:".concat(sdkChannel));
            return;
        }
        catch (Exception e)
        {
            Log.e("UnitySDK", e.toString());
        }
    }
    
    public  static void Exit(final  String sdkChannel)
    {
        try
        {
            if(sdkChannel.compareTo("oppo") == 0)
            {
                OppoHandler.ExitSdk(ConfigUtl.UnityListener);
                return;
            }
            else if(sdkChannel.compareTo("vivo") == 0)
            {
                VivoHandler.Exit();
                return;
            }
            else if(sdkChannel.compareTo("yingyongbao") == 0)
            {
                return;
            }
            else if(sdkChannel.compareTo("huawei") == 0)
            {
                return;
            }
            else if(sdkChannel.compareTo("zs") == 0)
            {
                ZsHandler.Exit();
                return;
            }
            else if(sdkChannel.compareTo("233") == 0)
            {
            	Handler233.Exit();
                return;
            }
            Log.e("UnitySDK", "no sdk config:".concat(sdkChannel));
            return;
        }
        catch (Exception e)
        {
            Log.e("UnitySDK", e.toString());
        }
    }

    public  static void Pay(final String sdkChannel, final String payJson)
    {
        try
        {
            if(sdkChannel.compareTo("oppo") == 0)
            {
                JSONObject jsonObject = new JSONObject(payJson);
                OppoHandler.Pay(ConfigUtl.UnityListener,(jsonObject.has("amount")?jsonObject.getInt("amount"):0),
                        jsonObject.has("order")?jsonObject.getString("order"):"",
                        jsonObject.has("goodsName")?jsonObject.getString("goodsName"):"",
                        jsonObject.has("goodsDes")?jsonObject.getString("goodsDes"):"",
                        jsonObject.has("userId")?jsonObject.getString("userId"):"",
                        jsonObject.has("callback")?jsonObject.getString("callback"):"");
                return;
            }
            else if(sdkChannel.compareTo("vivo") == 0)
            {
                JSONObject jsonObject = new JSONObject(payJson);
                VivoHandler.Pay(jsonObject.has("amount")?jsonObject.getInt("amount"):0,
                        jsonObject.has("order")?jsonObject.getString("order"):"",
                        jsonObject.has("goodsName")?jsonObject.getString("goodsName"):"",
                        jsonObject.has("goodsDes")?jsonObject.getString("goodsDes"):"",
                        jsonObject.has("callback")?jsonObject.getString("callback"):"",
                        jsonObject.has("extInfo")?jsonObject.getString("extInfo"):"");
                return;
            }
            else if(sdkChannel.compareTo("yingyongbao") == 0)
            {
                JSONObject jsonObject = new JSONObject(payJson);
                YingyongbaoHandler.Pay(jsonObject.has("amount")?jsonObject.getInt("amount"):0,
                        jsonObject.has("order")?jsonObject.getString("order"):"",
                        jsonObject.has("goodsName")?jsonObject.getString("goodsName"):"",
                        jsonObject.has("goodsDes")?jsonObject.getString("goodsDes"):"",
                        jsonObject.has("callback")?jsonObject.getString("callback"):"",
                        jsonObject.has("extInfo")?jsonObject.getString("extInfo"):"");
                return;
            }
            else if(sdkChannel.compareTo("huawei") == 0)
            {
                JSONObject jsonObject = new JSONObject(payJson);
                HuaweiHandler.Pay(jsonObject.has("amount")?jsonObject.getInt("amount"):0,
                        jsonObject.has("order")?jsonObject.getString("order"):"",
                        jsonObject.has("goodsName")?jsonObject.getString("goodsName"):"",
                        jsonObject.has("goodsDes")?jsonObject.getString("goodsDes"):"",
                        jsonObject.has("callback")?jsonObject.getString("callback"):"",
                        jsonObject.has("extInfo")?jsonObject.getString("extInfo"):"");
                return;
            }
            else if(sdkChannel.compareTo("zs") == 0)
            {
                JSONObject jsonObject = new JSONObject(payJson);
                ZsHandler.Pay("0",
                        jsonObject.has("order")?jsonObject.getString("order"):"",
                        jsonObject.has("goodsId")?jsonObject.getString("goodsId"):"",
                        jsonObject.has("price")?jsonObject.getString("price"):"",
                        jsonObject.has("goodsName")?jsonObject.getString("goodsName"):"",
                        jsonObject.has("count")?jsonObject.getString("count"):"",
                        jsonObject.has("extInfo")?jsonObject.getString("extInfo"):"",
                        jsonObject.has("callback")?jsonObject.getString("callback"):"");
                return;
            }
            else if(sdkChannel.compareTo("233") == 0)
            {
                JSONObject jsonObject = new JSONObject(payJson);
                Handler233.Pay("0",
                        jsonObject.has("order")?jsonObject.getString("order"):"",
                        jsonObject.has("goodsId")?jsonObject.getString("goodsId"):"",
                        jsonObject.has("price")?jsonObject.getString("price"):"",
                        jsonObject.has("goodsName")?jsonObject.getString("goodsName"):"",
                        jsonObject.has("count")?jsonObject.getString("count"):"",
                        jsonObject.has("extInfo")?jsonObject.getString("extInfo"):"",
                        jsonObject.has("callback")?jsonObject.getString("callback"):"");
                return;
            }
            Log.e("UnitySDK", "no sdk config:".concat(sdkChannel));
            return;
        }
        catch (Exception e)
        {
            Log.e("UnitySDK", e.toString());
        }
    }
    
    public static void ShowFloatMenu(final String channel, final boolean bShow)
    {
    	try
        {
            if(channel.compareTo("oppo") == 0)
            {
                return;
            }
            else if(channel.compareTo("vivo") == 0)
            {
                return;
            }
            else if(channel.compareTo("yingyongbao") == 0)
            {
                return;
            }
            else if(channel.compareTo("huawei") == 0)
            {
                return;
            }
            else if(channel.compareTo("zs") == 0)
            {
            	ZsHandler.ShowFloatMenu(bShow);
                return;
            }
            Log.e("UnitySDK", "no sdk config:".concat(channel));
            return;
        }
        catch (Exception e)
        {
            Log.e("UnitySDK", e.toString());
        }
    }
    
    public static void UpdateRole(final String sdkChannel, final String payJson)
    {
    	try
        {
            if(sdkChannel.compareTo("oppo") == 0)
            {
                return;
            }
            else if(sdkChannel.compareTo("vivo") == 0)
            {
                return;
            }
            else if(sdkChannel.compareTo("yingyongbao") == 0)
            {
                return;
            }
            else if(sdkChannel.compareTo("huawei") == 0)
            {
                return;
            }
            else if(sdkChannel.compareTo("zs") == 0)
            {
                JSONObject jsonObject = new JSONObject(payJson);
                ZsHandler.UpdateRole(jsonObject.has("userId")?jsonObject.getString("userId"):"",
                        jsonObject.has("userName")?jsonObject.getString("userName"):"",
                        jsonObject.has("svrName")?jsonObject.getString("svrName"):"",
                        jsonObject.has("svrId")?jsonObject.getString("svrId"):"",
                        jsonObject.has("level")?jsonObject.getString("level"):"",
                        jsonObject.has("isNewCreate")?jsonObject.getBoolean("isNewCreate"):false);
                return;
            }
            Log.e("UnitySDK", "no sdk config:".concat(sdkChannel));
            return;
        }
        catch (Exception e)
        {
            Log.e("UnitySDK", e.toString());
        }
    }
    
    public static void RequestPermission(final String sdkChannel)
    {
    	if(sdkChannel.compareTo("tobid") == 0)
    	{
    		TobidHandler.requestPermission();
    	}
    }

    public static  void Ads(final String sdkChannel, final String type, final String posId)
    {
        if(sdkChannel.compareTo("admobile") == 0)
        {
            if(type.compareTo("Reward") == 0)
            {
                AdmobileHandler.ShowRewardAd(posId);
            }
            else if(type.compareTo("Banner") == 0)
            {
                AdmobileHandler.ShowBannerAd(posId);
            }
            else if(type.compareTo("FullScreen") == 0)
            {
                AdmobileHandler.ShowFullScreenAd(posId);
            }
        }
        else if(sdkChannel.compareTo("tobid") == 0)
        {
            if(type.compareTo("Reward") == 0)
            {
                TobidHandler.ShowRewardAd(posId);
            }
            else if(type.compareTo("Banner") == 0)
            {
            	TobidHandler.ShowBannerAd(posId);
            }
            else if(type.compareTo("FullScreen") == 0)
            {
            	TobidHandler.ShowFullScreenAd(posId);
            }
        }
        else if(sdkChannel.compareTo("233") == 0)
        {
        	try {
        		int posInt = Integer.parseInt(posId);
                if(type.compareTo("Reward") == 0)
                {
                    Handler233.ShowRewards(posInt);
                }
                else if(type.compareTo("Banner") == 0)
                {
                	Handler233.ShowBanner(posInt);
                }
                else if(type.compareTo("FullScreen") == 0)
                {
                	Handler233.ShowFullScreen(posInt);
                }
        	}
        	catch (NumberFormatException e) {
	            System.out.println("无法将字符串解析为整数");
	            return;
	        }
        }
    }
}
