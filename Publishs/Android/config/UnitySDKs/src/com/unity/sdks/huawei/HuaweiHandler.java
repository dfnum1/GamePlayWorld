package com.unity.sdks.huawei;

import android.content.Intent;
import android.content.IntentSender;
import android.text.TextUtils;
import android.util.Log;

import com.huawei.hmf.tasks.OnFailureListener;
import com.huawei.hmf.tasks.OnSuccessListener;
import com.huawei.hmf.tasks.Task;
import com.huawei.hms.common.ApiException;
import com.huawei.hms.jos.*;
import com.huawei.hms.jos.games.Games;
import com.huawei.hms.jos.games.PlayersClient;
import com.huawei.hms.jos.games.player.Player;
import com.huawei.hms.support.account.AccountAuthManager;
import com.huawei.hms.support.account.request.AccountAuthParams;
import com.huawei.hms.support.account.request.AccountAuthParamsHelper;
import com.huawei.hms.support.account.result.AccountAuthResult;
import com.huawei.hms.support.account.result.AuthAccount;
import com.huawei.hms.utils.ResourceLoaderUtil;
import com.huawei.hms.iap.Iap;
import com.huawei.hms.iap.IapApiException;
import com.huawei.hms.iap.IapClient;
import com.huawei.hms.iap.entity.ConsumeOwnedPurchaseReq;
import com.huawei.hms.iap.entity.ConsumeOwnedPurchaseResult;
import com.huawei.hms.iap.entity.IsEnvReadyResult;
import com.huawei.hms.iap.entity.OrderStatusCode;
import com.huawei.hms.iap.entity.OwnedPurchasesReq;
import com.huawei.hms.iap.entity.OwnedPurchasesResult;
import com.huawei.hms.iap.entity.ProductInfo;
import com.huawei.hms.iap.entity.ProductInfoReq;
import com.huawei.hms.iap.entity.ProductInfoResult;
import com.huawei.hms.iap.entity.PurchaseIntentReq;
import com.huawei.hms.iap.entity.PurchaseIntentResult;
import com.huawei.hms.iap.entity.PurchaseIntentWithPriceReq;
import com.huawei.hms.iap.entity.StartIapActivityReq;
import com.huawei.hms.iap.entity.StartIapActivityResult;
import com.huawei.hms.support.api.client.Status;
import com.unity.sdks.ConfigUtl;
import com.unity3d.player.UnityPlayer;
import org.json.JSONException;

public class HuaweiHandler
{
    /** requestCode for pull up the pay page */
    public static final int REQ_CODE_BUYWITHPRICE = 4001;

    /** requestCode for pull up the pmsPay page */
    public static final int REQ_CODE_BUY = 4002;

    /** requestCode for pull up the login page or agreement page for createPurchaseIntentWithPrice interface*/
    public static final int REQ_CODE_BUYWITHPRICE_CONTINUE = 4005;

    /** requestCode for pull up the login page for isEnvReady interface */
    public static final int REQ_CODE_LOGIN = 2001;
    
	private static IapClient ms_IAPClient;
    private  static  boolean hasInit;
    public static AccountAuthParams getHuaweiIdParams() {
        return new AccountAuthParamsHelper().setId().createParams();
    }
    public static void Init()
    {
        hasInit = false;
        AccountAuthParams params = AccountAuthParams.DEFAULT_AUTH_REQUEST_PARAM_GAME;
        JosAppsClient appsClient = JosApps.getJosAppsClient(UnityPlayer.currentActivity);
        Task<Void> initTask;
        ResourceLoaderUtil.setmContext(UnityPlayer.currentActivity);  // 设置防沉迷提示语的Context，此行必须添加
        AppParams appParams=new AppParams(params, new AntiAddictionCallback() {
            @Override
            public void onExit() {
                // 该回调会在如下两种情况下返回:
                // 1.未成年人实名帐号在白天登录游戏，华为会弹框提示玩家不允许游戏，玩家点击“确定”，华为返回回调
                // 2.未成年实名帐号在国家允许的时间登录游戏，到晚上9点，华为会弹框提示玩家已到时间，玩家点击“知道了”，华为返回回调
                // 您可在此处实现游戏防沉迷功能，如保存游戏、调用帐号退出接口或直接游戏进程退出(如System.exit(0))
				UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAntiAddictionExit", "");
            }
        });

        initTask = appsClient.init(appParams);
        ms_IAPClient = Iap.getIapClient(UnityPlayer.currentActivity);
        initTask.addOnSuccessListener(new OnSuccessListener<Void>() {
            @Override
            public void onSuccess(Void aVoid) {
                hasInit = true;
                // 游戏初始化成功后需要调用一次浮标显示接口
                Games.getBuoyClient(UnityPlayer.currentActivity).showFloatWindow();
                // 必须在init成功后，才可以实现登录功能
                // signIn();
                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnInitSuccess", "");
            }
        }).addOnFailureListener(
                new OnFailureListener() {
                    @Override
                    public void onFailure(Exception e) {
                        if (e instanceof ApiException) {
                            ApiException apiException = (ApiException) e;
                            int statusCode = apiException.getStatusCode();
                            Log.w("HUAWEI", "Init Failure:" + statusCode);
                            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnInitFailure", "" +statusCode);
                            if (statusCode == 907135003)
                            {
                                // 907135003表示玩家取消HMS Core升级或组件升级
                                Init();
                            }
                        }
                    }
                });

    }

    public  static  void ShowFloat(boolean show)
    {
        if(show) Games.getBuoyClient(UnityPlayer.currentActivity).showFloatWindow();
        else Games.getBuoyClient(UnityPlayer.currentActivity).hideFloatWindow();
    }

    public  static void Login()
    {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if(!hasInit)
                {
                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPopMessage", "1"); //未初始化
                    return;
                }
                Task<AuthAccount> authAccountTask = AccountAuthManager.getService(UnityPlayer.currentActivity, getHuaweiIdParams()).silentSignIn();
                authAccountTask.addOnSuccessListener(
                                new OnSuccessListener<AuthAccount>() {
                                    @Override
                                    public void onSuccess(AuthAccount authAccount) {
                                        String msg = "{";
                                        msg += "\"uid\":\"" +authAccount.getUid() + "\",";
                                        msg += "\"token\":\"" +authAccount.getAccessToken() + "\",";
                                        msg += "\"nickName\":\"" +authAccount.getDisplayName() + "\",";
                                        msg += "\"unionId\":\"" +authAccount.getUnionId() + "\",";
                                        msg += "\"age\":\"" +authAccount.getAgeRange() + "\",";
                                        msg += "\"opneId\":\"" +authAccount.getOpenId() + "\",";
                                        msg += "\"gender\":\"" +authAccount.getGender() + "\",";
                                        msg += "\"homezone\":\"" +authAccount.getHomeZone() + "\",";
                                        msg += "\"email\":\"" +authAccount.getEmail() + "\"";
                                        msg += "}";
                                        Log.d("HUAWEI", "Login:" + msg);
                                        try
                                        {
                                            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAccountData", authAccount.toJson() ); //未初始化
                                        } catch (JSONException var7) {
                                            //showLog("Failed to convert json from signInResult.");
                                        }
                                        getGamePlayer();
                                    }
                                })
                        .addOnFailureListener(
                                new OnFailureListener() {
                                    @Override
                                    public void onFailure(Exception e) {
                                        if (e instanceof ApiException) {
                                            ApiException apiException = (ApiException) e;
                                            String msg = "{";
                                            msg += "\"error\":\"" +apiException.getStatusCode() + "\",";
                                            msg += "\"msg\":\"signIn failed\"";
                                            msg += "}";
                                            Log.w("HUAWEI", "Login Fail:" + msg);
                                            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLoginFail", msg);
                                            // 在此处实现华为帐号显式授权
                                            startActivityForResult();
                                        }
                                    }
                                });
            }
        });
    }

    public static void startActivityForResult()
    {
        Intent intent = AccountAuthManager.getService(UnityPlayer.currentActivity,getHuaweiIdParams()).getSignInIntent();
        String jsonSignInResult = intent.getStringExtra("HUAWEIID_SIGNIN_RESULT");
        if (TextUtils.isEmpty(jsonSignInResult))
        {
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPopMessage", "3");
            return;
        }
        try
        {
            AccountAuthResult signInResult = new AccountAuthResult().fromJson(jsonSignInResult);
            if (0 == signInResult.getStatus().getStatusCode()) {
                //showLog("Sign in success.");
                //showLog("Sign in result: " + signInResult.toJson());

                getGamePlayer();
            }
            else
            {
                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPopMessage", "3");
               // showLog("Sign in failed: " + signInResult.getStatus().getStatusCode());
            }
        } catch (JSONException var7) {
            //showLog("Failed to convert json from signInResult.");
        }
    }

    static void getGamePlayer()
    {
        PlayersClient client = Games.getPlayersClient(UnityPlayer.currentActivity);
        // 执行游戏登录
        Task<Player> task = client.getGamePlayer();
        task.addOnSuccessListener(new OnSuccessListener<Player>() {
            @Override
            public void onSuccess(Player player) {
                String accessToken = player.getAccessToken();
                String displayName = player.getDisplayName(); // 免授权登录场景下无法获取玩家昵称
                String unionId = player.getUnionId();
                String openId = player.getOpenId();
                // 获取玩家信息成功，校验服务器端的玩家信息，校验通过后允许进入游戏
                String msg = "{";
                msg += "\"uid\":\"" +player.getPlayerId() + "\",";
                msg += "\"token\":\"" +player.getAccessToken() + "\",";
                msg += "\"nickName\":\"" +player.getDisplayName() + "\",";
                msg += "\"unionId\":\"" +player.getUnionId() + "\",";
                msg += "\"opneId\":\"" +player.getOpenId() + "\"";
                msg += "}";
                Log.d("HUAWEI", "GetPlayer:" + msg);
                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLoginSuccess", msg ); //
            }
        }).addOnFailureListener(new OnFailureListener() {
            @Override
            public void onFailure(Exception e) {
                if (e instanceof ApiException) {
                    String result = "rtnCode:" + ((ApiException) e).getStatusCode();
                    String msg = "{";
                    msg += "\"error\":\"" +((ApiException) e).getStatusCode() + "\",";
                    msg += "\"msg\":\"获取玩家信息失败，不允许进入游戏，并根据错误码处理\"";
                    msg += "}";
                    Log.w("HUAWEI", "GetPlayer Failure:" + msg);
                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLoginFail", msg);
                    if (7400 == ((ApiException) e).getStatusCode()||7018 == ((ApiException) e).getStatusCode()) {
                        // 7400表示用户未签署联运协议，需要继续调用init接口
                        // 7018表示初始化失败，需要继续调用init接口
                        Init();
                    }
                }
            }
        });
    }

    public  static  void Logout()
    {
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
            	Task<Void> authAccountTask = AccountAuthManager.getService(UnityPlayer.currentActivity, getHuaweiIdParams()).signOut();
           	 authAccountTask.addOnSuccessListener(
                        new OnSuccessListener<Void>() {
                            @Override
                            public void onSuccess(Void authAccount) {
                            	Log.d("HUAWEI", "Logout Success");
                           	 UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLogout", ""); //
                            }
                        })
                .addOnFailureListener(
                        new OnFailureListener() {
                            @Override
                            public void onFailure(Exception e) {
                            	Log.w("HUAWEI", "Logout Failure:" + e.toString());
                           	 UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLogout", ""); //
                            }
                        });
            }});
    }
    
    public static void Pay( final int amount, final String order, final String goodsName,final String goodsDes, final String userId,
            final String callback)
	{
    	UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
            	ProductInfo productItem = new ProductInfo();
            	productItem.setProductId(order);
            	productItem.setProductName(goodsName);
            	productItem.setPrice(String.valueOf(amount));
            	productItem.setCurrency("CNY");
            	productItem.setProductDesc(goodsDes);
            	//productItem.setPriceType();
            	
            	PurchaseIntentWithPriceReq req = new PurchaseIntentWithPriceReq();
            	req.setProductId(productItem.getProductId());
            	req.setPriceType(productItem.getPriceType());
            	req.setCurrency(productItem.getCurrency());
                req.setDeveloperPayload("testCreatePurchaseIntentWithPrice");
                req.setSdkChannel("1");
                req.setProductName(productItem.getProductName());
                req.setAmount(productItem.getPrice());
            	
                Task<PurchaseIntentResult> task = ms_IAPClient.createPurchaseIntentWithPrice(req);
                task.addOnSuccessListener(new OnSuccessListener<PurchaseIntentResult>() {
                    @Override
                    public void onSuccess(PurchaseIntentResult result) {
                        Log.i("HUAWEI", "createPurchaseIntentWithPrice, success");
                        if (result == null) {

                            return;
                        }
                        Status status = result.getStatus();
                        if (status == null || status.getResolution() == null) {
                            return;
                        }
                        // verify signature, and pull up cashier page.
                        boolean isSuccess = true;// CipherUtil.doCheck(result.getPaymentData(), result.getPaymentSignature(), CipherUtil.getPublicKey());
                        if (isSuccess) {
                        	try {
                        		status.startResolutionForResult(UnityPlayer.currentActivity, REQ_CODE_BUYWITHPRICE);

                            	String msg = "{";
                                msg += "\"payment\":\"" +result.getPaymentData() + "\",";
                                msg += "\"signature\":\"" +result.getPaymentSignature() + "\"";
                                msg += "}";
                                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPaySuccess", msg);
                            } catch (IntentSender.SendIntentException exp) {
                                Log.e("HUAWEI", exp.getMessage());
                            }
                        	

                        } else {
                            // Verify signature fail.
                        }
                    }
                }).addOnFailureListener(new OnFailureListener() {
                    @Override
                    public void onFailure(Exception e) {
                        Log.e("HUAWEI", "createPurchaseIntentWithPrice, fail");
                        Log.e("HUAWEI", "createPurchaseIntentWithPrice, " + e.getMessage());
                        if(e instanceof IapApiException)
                        {
                        	IapApiException iapApiException = (IapApiException)e;
                        	Log.i("HUAWEI", "returnCode: " + iapApiException.getStatusCode());
                        	
                        	String msg = "{";
                            msg += "\"error\":\"" +iapApiException.getStatusCode() + "\"";
                            msg += "}";
                            Log.w("HUAWEI", "GetPlayer Failure:" + msg);
                            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPayFail", msg);
                        }
                    }
                });
            }});
	}
}
