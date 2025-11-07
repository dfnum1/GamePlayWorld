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
import com.unity.sdks.huawei.HuaweiHandler;
import com.unity.sdks.oppo.OppoHandler;
import com.unity.sdks.vivo.VivoConfig;
import com.unity.sdks.vivo.VivoHandler;
import com.unity.sdks.yingyongbao.YingyongbaoHandler;
import com.unity3d.player.UnityPlayer;
import org.json.JSONObject;

public class SDKHandler
{
    public  static void SetListener(final  String name)
    {
        ConfigUtl.UnityListener = name;
    }
    public  static boolean Init(final String sdkChannel, final  String jsonSetting)
    {
        try
        {
            JSONObject jsonObject = new JSONObject(jsonSetting);
            if(sdkChannel.compareTo("oppo") == 0)
            {
                if(jsonObject.has("appSecret")) {
                    OppoHandler.Init(jsonObject.getString("appSecret"),ConfigUtl.UnityListener);
                    return  true;
                }
            }
            else if(sdkChannel.compareTo("vivo") == 0)
            {
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
            Log.e("UnitySDK", "no sdk config:".concat(sdkChannel));
            return;
        }
        catch (Exception e)
        {
            Log.e("UnitySDK", e.toString());
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
    }
}
