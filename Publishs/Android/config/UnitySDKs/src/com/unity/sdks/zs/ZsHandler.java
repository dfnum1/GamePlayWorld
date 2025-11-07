package com.unity.sdks.zs;

import com.appfame.zssdk.ZsApplication;
import com.appfame.zssdk.ZsSdk;
import com.appfame.zssdk.listener.InitZsSdkListener;
import com.unity.sdks.ConfigUtl;
import com.unity3d.player.UnityPlayer;
import com.appfame.zssdk.extra.ZsSdkUserInfo;
import com.appfame.zssdk.listener.LoginZsSdkListener;
import com.appfame.zssdk.listener.LogoutZsSdkListener;
import com.appfame.zssdk.listener.ExitZsSdkListener;
import com.appfame.zssdk.listener.PayZsSdkListener;
import com.appfame.zssdk.extra.GameRoleInfo;
import com.appfame.zssdk.extra.ZsOrderInfo;
import java.util.Map;

public class ZsHandler
{
    public  static boolean Init()
    {
    	ZsApplication.setContext(UnityPlayer.currentActivity.getApplication());
        ZsSdk.getInstance().initApplicationContext(UnityPlayer.currentActivity.getApplication());
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
    		 @Override
             public void run() {
        ZsSdk.getInstance().init( UnityPlayer.currentActivity, ConfigUtl.getMetaDataInt("zs_appid"), ConfigUtl.getMetaData("zs_appkey"),  new InitZsSdkListener() {
            @Override
            public void onInitSuccess() {
                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onInitSuccess", "");
            }

            @Override
            public void onInitFailure(String msg) {
            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onInitFailure", msg);
            }
        });
    		 }
      	 });
        return  true;
    }
    
    public static void Login()
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
     		 @Override
              public void run() {
    	ZsSdk.getInstance().login(UnityPlayer.currentActivity, new LoginZsSdkListener() {
            @Override
            public void onLoginSuccess(ZsSdkUserInfo loginInfo) {
            	String msg = "{";
                msg += "\"uid\":\"" +loginInfo.getUserId() + "\",";
                msg += "\"token\":\"" +loginInfo.getUserToken() + "\",";
                msg += "\"nickName\":\"" +loginInfo.getNickName() + "\",";
                msg += "\"device\":\"" +loginInfo.getMobile() + "\",";
                msg += "\"avatar\":\"" +loginInfo.getAvatarurl() + "\"";
                msg += "}";
            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onLoginSuccess", msg);
            }

            @Override
            public void onLoginFailure(String msg) {
            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onLoginFailure", msg);
            }
        });
     		}
   	 });
    }
    
    public static void Logout()
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
      		 @Override
               public void run() {
    	ZsSdk.getInstance().logout(new LogoutZsSdkListener() {
    	    @Override
    	    public void onLogoutSuccess() {
    	    	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onLogoutSuccess", "");
    	    }

    	    @Override
    	    public void onLogoutFailure(String msg) {
    	    	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onLogoutFailure", msg);
    	    }
    	});
        		}
      	 });
    }
    
    public static void Exit()
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
   		 @Override
            public void run() {
    	ZsSdk.getInstance().exitGame(UnityPlayer.currentActivity,new ExitZsSdkListener() {
    	    @Override
    	    public void onSuccess() {
    	    	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onExitSuccess", "");
    	    }

    	    @Override
    	    public void onError(String msg) {
    	    	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onExitError", msg);
    	    }
    	});
   		}
   	 });
    }
    
    public static void ShowFloatMenu(final boolean bShow)
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
      		 @Override
               public void run() {
      			if(bShow) ZsSdk.getInstance().showFloatMenu(UnityPlayer.currentActivity);
      			else ZsSdk.getInstance().destoryFloatMenu(UnityPlayer.currentActivity);
      		 }
      		 });
    }
    
    public static void UpdateRole(final String userId, final String roleName, final String serverName, final String serverId, final String level, final boolean isNewCreate)
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
   		 @Override
            public void run() {
   			GameRoleInfo gameRoleInfo = new GameRoleInfo();
   			gameRoleInfo.setRoleName(roleName);
   			gameRoleInfo.setRoleID(userId);
   			gameRoleInfo.setServerName(serverName);
   			gameRoleInfo.setServerID(serverId);
   			gameRoleInfo.setLevel(level);
   			gameRoleInfo.setVipLevel(level);
   			gameRoleInfo.setRoleCreate(isNewCreate);
   			ZsSdk.getInstance().report(gameRoleInfo);
   		 }
   		 });
    }
    
    public static void Pay(final String userId, final String order, final String goodsId, final String price,final String goodsName, final String count, final String externParam, final String callbackurl)
    {
    	 UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
    		 @Override
             public void run() {
    			 ZsOrderInfo zsOrderInfo = new ZsOrderInfo();
    		    	//商品ID
    		    	zsOrderInfo.setGoodsID(goodsId);
    		    	//订单号
    		    	zsOrderInfo.setOrderID(order);
    		    	//支付价格 以"元"为单位.
    		    	zsOrderInfo.setGoodsPrices(price);
    		    	//商品名称
    		    	zsOrderInfo.setGoodsName(goodsName);
    		    	//商品数量
    		    	zsOrderInfo.setGoodsAccount(count);
    		    	zsOrderInfo.setExtraParams(externParam);
    		    	zsOrderInfo.setCallbackUrl(callbackurl);
    		    	//
    		    	ZsSdk.getInstance().pay(UnityPlayer.currentActivity,zsOrderInfo, new PayZsSdkListener() {
    		    	    @Override
    		    	    public void onPayEnd(String CpOrderNo) {
    		    	    	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onPayEnd", CpOrderNo);
    		    	    }

    		    	    @Override
    		    	    public void onPayFailure(String msg) {
    		    	    	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onPayFailure", msg);
    		    	    }
    		    	});
    		 }
    	 });
    	
    }
}
