package com.unity.sdks.yingyongbao;
import com.tencent.ysdk.module.share.IShareApi;
import com.tencent.ysdk.module.share.ShareApi;
import com.unity.sdks.ConfigUtl;
import com.unity3d.player.UnityPlayer;
import com.tencent.ysdk.api.YSDKApi;
import com.tencent.ysdk.framework.common.ePlatform;

public class YingyongbaoHandler {
    static YSDKCallback ms_YSDKCallback = null;

    public static void Init() {
        ms_YSDKCallback = new YSDKCallback();
        YSDKApi.init();
        YSDKApi.setUserListener(ms_YSDKCallback);
        YSDKApi.setAntiAddictListener(ms_YSDKCallback);

       // IShareApi shareApi = ShareApi.getInstance();
      //  shareApi.regShareCallBack(ms_YSDKCallback);

        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnInitSuccess", "");
    }

    public static void Login(final String channel) {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                String temp = channel.toLowerCase();
                if (temp.compareTo("wx") == 0) YSDKApi.login(ePlatform.WX);
                else if (temp.compareTo("qq") == 0) YSDKApi.login(ePlatform.QQ);
                else if(channel.compareTo("auto") == 0) YSDKApi.autoLogin();
                else YSDKApi.login(ePlatform.Guest);
            }
        });


    }

    public static void Logout()
    {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable()
        {
            @Override
            public void run() {
                YSDKApi.logout();
            }
        });
    }

    public static void Pay( final int amount, final String order, final String goodsName,final String goodsDes, final String userId,
                           final String callback)
    {

    }

    public static void Share()
    {
    }
}
