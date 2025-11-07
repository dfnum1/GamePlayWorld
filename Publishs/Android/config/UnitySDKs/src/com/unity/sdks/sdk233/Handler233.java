package com.unity.sdks.sdk233;

import com.unity.sdks.ConfigUtl;
import com.unity3d.player.UnityPlayer;

import android.util.Log;
import android.content.Intent;
import java.util.Map;

import com.meta.android.mpg.account.callback.LoginResultCallback;
import com.meta.android.mpg.account.model.UserInfo;
import com.meta.android.mpg.cm.MetaAdApi;
import com.meta.android.mpg.cm.api.IAdCallback;
import com.meta.android.mpg.common.MetaApi;
import com.meta.android.mpg.common.callback.OnExitCallback;
import com.meta.android.mpg.initialize.InitCallback;
import com.meta.android.mpg.account.callback.AccountChangedListener;
import com.meta.android.mpg.payment.callback.PayResultCallback;
import com.meta.android.mpg.payment.constants.MetaPayResult;
import com.meta.android.mpg.payment.model.PayInfo;


public class Handler233
{
	static AccountChangedListener ms_AccountChangedListener = null;
    public  static boolean Init()
    {
    	ms_AccountChangedListener = new AccountChangedListener() {
		        @Override
		        public void onAccountChanged(UserInfo userInfo) {
 	            	String msg = "{";
 	                msg += "\"uid\":\"" +userInfo.uid + "\",";
 	                msg += "\"token\":\"" +userInfo.sid + "\",";
 	                msg += "\"nickName\":\"" +userInfo.userName + "\",";
 	                msg += "\"bindPhone\":\"" +userInfo.bindPhone + "\",";
 	                msg += "\"age\":\"" +userInfo.age + "\",";
 	                msg += "\"isGuest\":" + (userInfo.isGuest?"true":"false") + ",";
 	                msg += "\"avatar\":\"" +userInfo.userIcon + "\"";
 	                msg += "}";
 	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onSiwtchAccount", msg);
		        }
		        @Override
		        public void onAccountLogout() {
		        	 UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLogout", "");
		        }
		    };
		    
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
    		 @Override
             public void run() {
    			 MetaApi.initMetaSdk(UnityPlayer.currentActivity.getApplication(), ConfigUtl.getMetaData("appkey_233"), new InitCallback() {
    		            @Override
    		            public void onInitializeSuccess() {
    		                Log.i("SdkHelper", "init success");
    		                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onInitSuccess", "");
    		            }

    		            @Override
    		            public void onInitializeFail(int i, String s) {
    		                Log.i("SdkHelper", "init fail=" + s);
    		                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onInitFailure", s);
    		            }
    		        });

    			 //注册全局的账号切换监听
    			 MetaApi.registerAccountChangedListener(ms_AccountChangedListener);
    		 }
      	 });
        return  true;
    }
    
    public static void Login()
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
     		 @Override
              public void run() {
     			MetaApi.login(UnityPlayer.currentActivity, new LoginResultCallback() {
     	            @Override
     	            public void loginSuccess(UserInfo userInfo) {
     	                Log.i("SdkHelper", "loginSuccess");
     	            	String msg = "{";
     	                msg += "\"uid\":\"" +userInfo.uid + "\",";
     	                msg += "\"token\":\"" +userInfo.sid + "\",";
     	                msg += "\"nickName\":\"" +userInfo.userName + "\",";
     	                msg += "\"bindPhone\":\"" +userInfo.bindPhone + "\",";
     	                msg += "\"age\":\"" +userInfo.age + "\",";
     	                msg += "\"isGuest\":" + (userInfo.isGuest?"true":"false") + ",";
     	                msg += "\"avatar\":\"" +userInfo.userIcon + "\"";
     	                msg += "}";
     	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onLoginSuccess", msg);
     	            }

     	            @Override
     	            public void loginFail(int i, String s) {
     	                Log.i("SdkHelper", "loginFail");
       	            	String msg = "{";
     	                msg += "\"code\":\"" +i + "\",";
     	                msg += "\"error\":\"" +s + "\"";
     	                msg += "}";
     	               UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onLoginFailure", msg);

     	            }
     	        });
     		}
   	 });
    }
    
    public static void Logout()
    {
    	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLogout", "");
    }
    
    public static void Exit()
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
   		 @Override
            public void run() {
   			MetaApi.exitGame(UnityPlayer.currentActivity, new OnExitCallback() {
   	            @Override
   	            public void exitGame() {
   	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onExitSuccess", "");
   	            }
   	        });
   		}
   	 });
    }
    
    public static boolean Pay(final String userId, final String order, final String goodsId, final String price,final String goodsName, final String count, final String externParam, final String callbackurl)
    {
    	 UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
    		 @Override
             public void run() {
    		        Integer priceInt = 0;
    		        try {
    		        	priceInt = Integer.parseInt(price);
    		        } catch (NumberFormatException e) {
    		            System.out.println("无法将字符串解析为整数");
    		            return;
    		        }
    		        
    		        //透传字段
    		        PayInfo payInfo = new PayInfo.Builder()
    		                .setCpOrderId(order) //
    		                .setProductCode(goodsId) //商品编码
    		                .setProductName(goodsName) //商品名称
    		                .setPrice(priceInt) //商品价格，单位(分)
    		                .setProductCount(1) //商品数量，默认是1
    		                .setCpExtra(externParam)
    		                .build();

    		        MetaApi.pay(UnityPlayer.currentActivity, payInfo, new PayResultCallback() {

    		            @Override
    		            public void onPayResult(int code, String msg) {
    		            	Log.i("onPayResult", code + ":" + msg);
    		                if (code == MetaPayResult.CODE_PAY_SUCCESS) {
    		                	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPaySuccess", msg);
    		                } else {
    		                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPayFail", msg);
    		                }
    		            }
    		        });
    		 }
    	 });
    	 return true;
    }
    
    public static void ShowFullScreen(final int posId)
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
     		 @Override
              public void run() {
     			MetaAdApi.get().showInterstitialAd(posId, new IAdCallback() {
     	            @Override
     	            public void onAdShow() {
     	                //广告展示
     	            	String msg = "{";
     	                msg += "\"posId\":\"" +posId + "\",";
     	                msg += "\"type\":\"FullScreen\"";
     	                msg += "}";
       	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdShow", msg);
     	            }

     	            @Override
     	            public void onAdShowFailed(int errCode, String errMsg) {
     	                //展示失败
     	            	String msg = "{";
     	                msg += "\"posId\":\"" +posId + "\",";
     	                msg += "\"error\":\"" +errMsg+"\",";
     	               msg += "\"code\":\"" +errCode+"\",";
     	                msg += "\"type\":\"FullScreen\"";
     	                msg += "}";
       	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdShowFailed", msg);
     	            }

     	            @Override
     	            public void onAdClick() {
     	                //广告被点击
     	            	String msg = "{";
     	                msg += "\"posId\":\"" +posId + "\",";
     	                msg += "\"type\":\"FullScreen\"";
     	                msg += "}";
       	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdClick", msg);
     	            }

     	            @Override
     	            public void onAdClose() {
     	                //广告被关闭
     	            	String msg = "{";
     	                msg += "\"posId\":\"" +posId + "\",";
     	                msg += "\"type\":\"FullScreen\"";
     	                msg += "}";
       	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdClose", msg);
     	            }
     	        });
     		 }});
    }
    
    public static void ShowBanner(final int posId)
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
      		 @Override
               public void run() {
      			MetaAdApi.get().showBannerAd(posId, new IAdCallback() {
      	            @Override
      	            public void onAdShow() {
      	                //广告展示
     	            	String msg = "{";
     	                msg += "\"posId\":\"" +posId + "\",";
     	                msg += "\"type\":\"Banner\"";
     	                msg += "}";
       	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdShow", msg);
      	            }

      	            @Override
      	            public void onAdClick() {
      	                //广告被点击
     	            	String msg = "{";
     	                msg += "\"posId\":\"" +posId + "\",";
     	                msg += "\"type\":\"Banner\"";
     	                msg += "}";
       	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdClick", msg);
      	            }

      	            @Override
      	            public void onAdClose() {
      	                //广告被关闭
     	            	String msg = "{";
     	                msg += "\"posId\":\"" +posId + "\",";
     	                msg += "\"type\":\"Banner\"";
     	                msg += "}";
       	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdClose", msg);
      	            }

					@Override
					public void onAdShowFailed(int arg0, String arg1) {
						// TODO Auto-generated method stub
     	            	String msg = "{";
     	                msg += "\"posId\":\"" +posId + "\",";
     	                msg += "\"error\":\"" +arg1+"\",";
     	               msg += "\"code\":\"" +arg0+"\",";
     	                msg += "\"type\":\"Banner\"";
     	                msg += "}";
       	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdShowFailed", msg);
					}
      	        });
      		 }});
    }
    
    public static void ShowRewards(final int posId)
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
   		 @Override
            public void run() {
   			MetaAdApi.get().showVideoAd(posId, new IAdCallback.IVideoIAdCallback() {
   	            @Override
   	            public void onAdShow() {
 	            	String msg = "{";
 	                msg += "\"posId\":\"" +posId + "\",";
 	                msg += "\"type\":\"Reward\"";
 	                msg += "}";
   	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdShow", msg);
   	            }

   	            @Override
   	             public void onAdShowFailed(int errCode, String errMsg) {
   	                // 播放失败
   	                Log.d("MetaAdApi", "onAdShowFailed： " + errMsg);
 	            	String msg = "{";
 	                msg += "\"posId\":\"" +posId + "\",";
 	                msg += "\"code\":\"" +errCode + "\",";
 	                msg += "\"error\":\"" +errMsg+"\",";
 	                msg += "\"type\":\"Reward\"";
 	                msg += "}";
   	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdShowFailed", msg);
   	            }

   	            @Override
   	            public void onAdClick() {
   	                //点击广告
 	            	String msg = "{";
 	                msg += "\"posId\":\"" +posId + "\",";
 	                msg += "\"type\":\"Reward\"";
 	                msg += "}";
   	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdClick", msg);
   	            }

   	            @Override
   	            public void onAdClose() {
   		    //  广告关闭
 	            	String msg = "{";
 	                msg += "\"posId\":\"" +posId + "\",";
 	                msg += "\"type\":\"Reward\"";
 	                msg += "}";
   	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdClose", msg);
   	            }
   	          

   	            @Override
   	            public void onAdClickSkip() {
   	                // 播放点击跳过
   	                Log.d("MetaAdApi", "onAdClickSkip");
 	            	String msg = "{";
 	                msg += "\"posId\":\"" +posId + "\",";
 	                msg += "\"type\":\"Reward\"";
 	                msg += "}";
   	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdClickSkip", msg);
   	            }

   	            @Override
   	            public void onAdReward() {
   	                //发放激励
   	                Log.d("MetaAdApi", "onAdReward");
 	            	String msg = "{";
 	                msg += "\"posId\":\"" +posId + "\",";
 	                msg += "\"type\":\"Reward\"";
 	                msg += "}";
   	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdReward", msg);
   	            }

   	            @Override
   	            public void onAdClose(Boolean aBoolean) {
   	                // 广告关闭, 废弃 ,建议使用onAdClose 和 onAdReward
   	                Log.d("MetaAdApi", "onAdClose");
 	            	String msg = "{";
 	                msg += "\"posId\":\"" +posId + "\",";
 	                msg += "\"type\":\"Reward\"";
 	                msg += "}";
   	            	UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onAdClose", msg);
   	            }
   	        });
   		 	}
   		 });
    }
}
