package com.unity.sdks.vivo;
import android.text.TextUtils;
import android.util.Log;
import com.unity.sdks.ConfigUtl;
import com.unity.sdks.vivo.bean.OrderBean;
import com.unity.sdks.vivo.bean.RoleInfoBean;
import com.unity3d.player.UnityPlayer;
import org.json.JSONObject;
import android.content.Context;
import com.unity3d.player.UnityPlayer;
import com.vivo.unionsdk.open.VivoConfigInfo;
import com.vivo.unionsdk.open.VivoUnionSDK;
import com.vivo.unionsdk.open.AuthenticCallback;
import com.vivo.unionsdk.open.FillRealNameCallback;
import com.vivo.unionsdk.open.OrderResultInfo;
import com.vivo.unionsdk.open.VivoAccountCallback;
import com.vivo.unionsdk.open.VivoConstants;
import com.vivo.unionsdk.open.VivoExitCallback;
import com.vivo.unionsdk.open.VivoPayCallback;
import com.vivo.unionsdk.open.VivoPayInfo;
import com.vivo.unionsdk.open.VivoRoleInfo;
import com.vivo.unionsdk.open.VivoUnionSDK;

import java.net.UnknownServiceException;
import java.util.ArrayList;
import java.util.List;

public class VivoHandler
{
    public  static void Init(final String appId, final  String appKey, final  String cpId,boolean passPrivacy, boolean debug)
    {
        VivoSPUtil.getInstance().init(UnityPlayer.currentActivity);
        VivoConfig.APP_ID = appId;
        VivoConfig.APP_KEY = appKey;
        VivoConfig.CP_ID = cpId;
        if(passPrivacy)
        {
            boolean isPassPrivacy = VivoSPUtil.getInstance().getBoolean(VivoSPUtil.PRIVACY_PASS, false);//游戏是否同意了隐私协议
            VivoConfigInfo sdkConfig = new VivoConfigInfo();
            sdkConfig.setPassPrivacy(passPrivacy);
            VivoUnionSDK.initSdk(UnityPlayer.currentActivity, appId,debug, sdkConfig);
        }
        else
        {
            VivoUnionSDK.initSdk(UnityPlayer.currentActivity, appId, debug);
        }

        VivoUnionSDK.registerAccountCallback(UnityPlayer.currentActivity,new VivoAccountCallback()
        {
            @Override
            public void onVivoAccountLogin(String userName, String uid, String authToken) {
                // 1. 收到登录成功回调后，调用服务端接口校验登录有效性。arg2返回值为authtoken。服务端接口详见文档。校验登录代码略。
                VivoConfig.UID = uid;
                VivoConfig.USE_NAME = userName;
                VivoConfig.USE_TOKEN = authToken;
                VivoRoleInfo roleInfo = new VivoRoleInfo(uid, "1", userName, "0", "0");

                // 2. 登录成功后上报角色信息
                VivoUnionSDK.reportRoleInfo(roleInfo);
                String msg = "{";
                msg += "\"uid\":\"" +uid + "\",";
                msg += "\"name\":\"" +userName + "\",";
                msg += "\"token\":\"" +authToken + "\"";
                msg += "}";

                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLoginSuccess", msg);

                //登录成功
                VivoUnionSDK.queryMissOrderResult(uid);
            }

            @Override
            public void onVivoAccountLogout(int i) {
                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLogoutSuccess", VivoConfig.UID);
            }

            @Override
            public void onVivoAccountLoginCancel() {
                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLoginCancel", VivoConfig.UID);
            }
        });

        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnInitSuccess", "");
    }

    public  static  void Login()
    {

    }

    public  static  void Exit(){
        VivoUnionSDK.exit(UnityPlayer.currentActivity, new VivoExitCallback() {
            @Override
            public void onExitCancel() {
                //退出取消
                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLogoutCancel", VivoConfig.UID);
            }

            @Override
            public void onExitConfirm() {
                //退出确认
                UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLogoutSuccess", VivoConfig.UID);
                VivoConfig.UID = "";
                VivoConfig.USE_NAME="";
                VivoConfig.USE_TOKEN="";
            }
        });
    }

    public  static void Pay(int amount, final String order, final String goodsName, final String goodsDes, final String notifyUrl, final  String extInfo)
    {
        if(TextUtils.isEmpty(VivoConfig.UID))
        {
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPayFailed", "还未登陆");
            return;
        }
        if(amount <=0)
        {
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPayFailed", "pay amount <=0");
            return;
        }
        RoleInfoBean bean = new RoleInfoBean();
        bean.setRoleId(VivoConfig.UID);
        bean.setRoleName(VivoConfig.USE_NAME);
        bean.setLevel("1");

        OrderBean orderBean = new OrderBean();
        long time = System.currentTimeMillis();
        if(TextUtils.isEmpty(order)) orderBean.setCpOrderNumber(String.valueOf(time)); //游戏生成订单号
        else orderBean.setCpOrderNumber(order);
        orderBean.setExtInfo(extInfo); //穿透参数
        orderBean.setNotifyUrl(notifyUrl);
        orderBean.setOrderAmount(String.valueOf(amount));
        orderBean.setProductDesc(goodsDes);
        orderBean.setProductName(goodsName);
        orderBean.setRoleInfoBean(bean);;
        VivoPayInfo vivoPayInfo = VivoSign.createPayInfo(VivoConfig.UID, orderBean);
        VivoUnionSDK.payV2(UnityPlayer.currentActivity,vivoPayInfo, new VivoPayCallback() {
            // 客户端返回的支付结果不可靠，请再查询服务器，以服务器端最终的支付结果为准；
            @Override
            public void onVivoPayResult(int i, OrderResultInfo orderResultInfo) {
                if (i == VivoConstants.PAYMENT_RESULT_CODE_SUCCESS) {

                    String msg ="{";
                    msg += "\"order\":\"" +orderResultInfo.getCpOrderNumber() + "\",";
                    msg += "\"resultCode\":\"" +orderResultInfo.getResultCode() + "\",";
                    msg += "\"uid\":\"" +VivoConfig.UID + "\",";
                    msg += "\"transNo\":\"" +orderResultInfo.getTransNo() + "\"";
                    msg += "}";
                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener,"OnPaySuccess", msg);
                    /**
                     * 后续流程：游戏确认支付成功--->发货--->确认收到道具--->调用reportOrderComplete(transNo, false)
                     */

                    List<String> list = new ArrayList<String>();
                    list.add(orderResultInfo.getTransNo());
                    VivoUnionSDK.reportOrderComplete(list,false); //4、调用reportOrderComplete(transNo, 是否补单)
                } else if (i == VivoConstants.PAYMENT_RESULT_CODE_CANCEL)
                {
                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener,"OnPayCancel", orderResultInfo.getCpOrderNumber());
                }
                else if (i == VivoConstants.PAYMENT_RESULT_CODE_UNKNOWN)
                {
                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener,"OnPayUnkown", orderResultInfo.getCpOrderNumber());
                }
                else
                {
                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener,"OnPayFail", orderResultInfo.getCpOrderNumber());
                }
            }

        });
    }
}
