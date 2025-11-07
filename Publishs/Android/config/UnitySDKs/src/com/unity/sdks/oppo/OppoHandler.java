package com.unity.sdks.oppo;

import android.text.TextUtils;
import com.unity.sdks.ConfigUtl;
import com.unity3d.player.UnityPlayer;
import com.nearme.game.sdk.GameCenterSDK;
import com.nearme.game.sdk.callback.*;
import com.nearme.game.sdk.common.model.biz.PayInfo;
import com.nearme.game.sdk.common.model.biz.ReportUserGameInfoParam;
import com.nearme.game.sdk.common.model.biz.ReqUserInfoParam;
import com.nearme.game.sdk.pay.PayResponse;

import android.util.Log;

import java.util.Random;

public class OppoHandler
{
    public static void Init( final String appSecret, final String gameObject)
    {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable(){
            @Override
            public void run() {
                // TODO Auto-generated method stub
                GameCenterSDK.init(appSecret, UnityPlayer.currentActivity);
                Log.e("初始化成功", "int calling...");
                UnityPlayer.UnitySendMessage(gameObject, "OnInitSuccess", "");
            }
        });
    }
    public  static void Login(final String gameObject)
    {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                // TODO Auto-generated method stub
                GameCenterSDK.getInstance().doLogin(UnityPlayer.currentActivity, new ApiCallback() {

                    @Override
                    public void onSuccess(String msg) {
                        // TODO Auto-generated method stub
                        Log.e("登录成功", "login calling...");
                        UnityPlayer.UnitySendMessage(gameObject, "OnLoginSuccess", msg);
                    }

                    @Override
                    public void onFailure(String msg, int code) {
                        // TODO Auto-generated method stub
                        UnityPlayer.UnitySendMessage(gameObject, "OnLoginFail", msg);
                    }
                });
            }
        });
    }
    public static void ExitSdk(final String gameObject) {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                // TODO Auto-generated method stub
                GameCenterSDK.getInstance().onExit(UnityPlayer.currentActivity, new GameExitCallback() {

                    @Override
                    public void exitGame() {
                        // TODO Auto-generated method stub
                        // CP 实现游戏退出操作，也可以直接调用
                        // AppUtil工具类里面的实现直接强杀进程~
//						 AppUtil.exitGameProcess(UnityPlayer.currentActivity);
                        UnityPlayer.UnitySendMessage(gameObject, "OnExitGame","退出成功");
                    }
                });
            }
        });
    }

    private static void GetUserInfo(final String gameObject, String token, String ssoid) {
        GameCenterSDK.getInstance().doGetUserInfo(new ReqUserInfoParam(token, ssoid), new ApiCallback() {

            @Override
            public void onSuccess(String resultMsg) {// json格式
                // Toast.makeText(DemoActivity.this, resultMsg,
                // Toast.LENGTH_LONG).show();
                UnityPlayer.UnitySendMessage(gameObject, "OnGetUserInfoSuccess", resultMsg);
            }

            @Override
            public void onFailure(String resultMsg, int resultCode) {

            }
        });
    }

    public static void SendUserInfo(final String gameObject, final String appId, final String serverId,
                                    final String roleName, final String lv) {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {

            @Override
            public void run() {
                // TODO Auto-generated method stub
                GameCenterSDK.getInstance().doReportUserGameInfoData(
                        new ReportUserGameInfoParam(appId, serverId, roleName, lv), new ApiCallback() {

                            @Override
                            public void onSuccess(String resultMsg) {
                                UnityPlayer.UnitySendMessage(gameObject, "OnSendUserInfoSuccess", "上传成功");
                                // Toast.makeText(DemoActivity.this, "success",
                                // Toast.LENGTH_LONG).show();
                            }

                            @Override
                            public void onFailure(String resultMsg, int resultCode) {
                                UnityPlayer.UnitySendMessage(gameObject, "OnSendUserInfoFail", resultMsg);
                                // Toast.makeText(DemoActivity.this, resultMsg,
                                // Toast.LENGTH_LONG).show();
                            }
                        });
            }
        });
    }

    public static void Pay(final String gameObject, final int amount, final String order, final String goodsName,final String goodsDes, final String userId,
                           final String callback) {

        if(amount <=0)
        {
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPayFailed", "pay amount <=0");
            return;
        }
        Log.e("充值", "pay calling...");
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {

            @Override
            public void run() {
                String cpOrder = order;
                // TODO Auto-generated method stub
                if(TextUtils.isEmpty(cpOrder))
                    cpOrder = System.currentTimeMillis() + new Random().nextInt(1000) + "";
                // CP 支付参数
                // int amount = 1; // 支付金额，单位分
                PayInfo payInfo = new PayInfo( cpOrder, "自定义字段",
                        amount);
                payInfo.setProductDesc(goodsDes);
                payInfo.setProductName(goodsName);
                payInfo.setCallbackUrl(callback);
                payInfo.setAttach( userId);

                GameCenterSDK.getInstance().doSinglePay(UnityPlayer.currentActivity, payInfo, new SinglePayCallback() {
                    // add OPPO 支付成功处理逻辑~
                    public void onSuccess(String resultMsg) {
                        UnityPlayer.UnitySendMessage(gameObject, "OnPaySuccess", "支付成功");
                    }

                    // add OPPO 支付失败处理逻辑~
                    public void onFailure(String resultMsg, int resultCode) {
                        if (PayResponse.CODE_CANCEL != resultCode) {
                            UnityPlayer.UnitySendMessage(gameObject, "OnPayFail", "支付失败");
                        } else { // 取消支付处理
                            UnityPlayer.UnitySendMessage(gameObject, "OnPayCancel", "支付取消");
                        }
                    }

                    /*
                     * bySelectSMSPay 为true表示回调来自于支付渠道列表选择短信支付，false表示没有
                     * 网络等非主动选择短信支付时候的回调
                     */
                    public void onCallCarrierPay(PayInfo payInfo, boolean bySelectSMSPay) {
                        // add 运营商支付逻辑~
                        // Toast.makeText(DemoActivity.this,
                        // "运营商支付",Toast.LENGTH_SHORT).show();
                        UnityPlayer.UnitySendMessage(gameObject, "onCallCarrierPay", "运营商支付");
                    }
                });

            }
        });
    }

    public static void onResume() {
//        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
//
//            @Override
//            public void run() {
//                // TODO Auto-generated method stub
//                GameCenterSDK.getInstance().onResume(UnityPlayer.currentActivity);
//            }
//        });

    }

    public static void onPause() {
//        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
//
//            @Override
//            public void run() {
//                // TODO Auto-generated method stub
//                GameCenterSDK.getInstance().onPause();
//            }
//        });
    }
}
