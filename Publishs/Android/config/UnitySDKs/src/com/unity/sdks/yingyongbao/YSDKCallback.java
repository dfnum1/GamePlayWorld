package com.unity.sdks.yingyongbao;

import com.tencent.ysdk.api.YSDKApi;
import com.tencent.ysdk.module.antiaddiction.listener.AntiAddictListener;
import com.tencent.ysdk.module.antiaddiction.listener.AntiRegisterWindowCloseListener;
import com.tencent.ysdk.module.antiaddiction.model.AntiAddictRet;
import com.tencent.ysdk.module.pay.PayListener;
import com.tencent.ysdk.module.pay.PayRet;
import com.tencent.ysdk.module.share.ShareCallBack;
import com.tencent.ysdk.module.share.impl.ShareRet;
import com.tencent.ysdk.module.user.*;
import com.tencent.ysdk.framework.common.eFlag;
import com.unity.sdks.ConfigUtl;
import com.unity3d.player.UnityPlayer;

public class YSDKCallback implements UserListener, PayListener, AntiAddictListener, AntiRegisterWindowCloseListener, ShareCallBack
{
    @Override
    public void onLoginLimitNotify(AntiAddictRet antiAddictRet) {
        if(antiAddictRet.flag == AntiAddictRet.RET_SUCC)
        {
            String msg = "{";
            msg += "\"title:\"" + "\"" + antiAddictRet.title + "\",";
            msg += "\"content:\"" + "\"" + antiAddictRet.content + "\",";
            msg += "\"all:\"" + "\"" + antiAddictRet.toString() + "\"";
            msg += "}";
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onLoginLimitNotify", msg);
        }
    }

    @Override
    public void onTimeLimitNotify(AntiAddictRet antiAddictRet) {
        if(antiAddictRet.flag == AntiAddictRet.RET_SUCC)
        {
            String msg = "{";
            msg += "\"title:\"" + "\"" + antiAddictRet.title + "\",";
            msg += "\"content:\"" + "\"" + antiAddictRet.content + "\",";
            msg += "\"all:\"" + "\"" + antiAddictRet.toString() + "\"";
            msg += "}";
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onLoginLimitNotify", msg);
        }
    }

    @Override
    public void OnPayNotify(PayRet payRet) {
        if(payRet.flag == PayRet.RET_SUCC)
        {
            //支付流程成功
            switch (payRet.payState) {
                //支付成功
                case PayRet.PAYSTATE_PAYSUCC:
                    String msg = "{";
                    msg += "\"amont:\"" + "\"" + payRet.realSaveNum + "\",";
                    msg += "\"channel:\"" + "\"" + payRet.payChannel + "\",";
                    msg += "\"provideState:\"" + "\"" + payRet.provideState + "\",";
                    msg += "\"extendInfo:\"" + "\"" + payRet.extendInfo + "\",";
                    msg += "\"all:\"" + "\"" + payRet.toString() + "\"";
                    msg += "}";
                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPaySuccess", msg);
                    break;
                //取消支付
                case PayRet.PAYSTATE_PAYCANCEL:
                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPayCancel", payRet.toString());
                    break;
                default:
                {
                    String msg1 = "{";
                    msg1 += "\"flag:\"" + "\"" + payRet.flag + "\",";
                    msg1 += "\"all:\"" + "\"" + payRet.toString() + "\"";
                    msg1 += "}";
                    UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPayFail", msg1);
                    break;
                }
            }
        }
        else
        {
            String msg = "{";
            msg += "\"flag:\"" + "\"" + payRet.flag + "\",";
            msg += "\"all:\"" + "\"" + payRet.toString() + "\"";
            msg += "}";
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPayFail", msg);
        }
    }

    @Override
    public void OnLoginNotify(UserLoginRet userLoginRet) {
        if(userLoginRet.flag == eFlag.Succ)
        {
            if(userLoginRet.getLoginType() != UserLoginRet.LOGIN_TYPE_TIMER)
                YSDKApi.setAntiAddictGameStart();

            String msg = "{";
            msg += "\"payToken\":" + "\"" + userLoginRet.getPayToken() + "\",";
            msg += "\"loginType\":" + "\"" + userLoginRet.getLoginType() + "\",";
            msg += "\"token\":" + "\"" + userLoginRet.getAccessToken() + "\",";
            msg += "\"getChannel\":" + "\"" + userLoginRet.getRegChannel() + "\",";
            msg += "\"userType\":" + "\"" + userLoginRet.getUserType() + "\",";
            msg += "\"uid\":" + "\"" + userLoginRet.open_id + "\",";
            msg += "\"name\":" + "\"" + userLoginRet.nick_name + "\",";
            msg += "\"all\":" + "\"" + userLoginRet.toString() + "\"";
            msg += "}";
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLoginSuccess", msg);
        }
        else
        {
            YSDKApi.logout();
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLoginFail", String.valueOf(userLoginRet.flag));
        }
    }

    @Override
    public void OnWakeupNotify(WakeupRet wakeupRet) {
        String msg = "{";
        msg += "\"flag\":" + "\"" + wakeupRet.flag+ "\",";
        msg += "\"all\":" + "\"" + wakeupRet.toString() + "\"";
        msg += "}";
        if(wakeupRet.flag == eFlag.Wakeup_YSDKLogining)
        {
            //用拉起的账号登录，登录结果在OnLoginNotify()中回调
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLoginFail", msg);
        }
        else
        {
            //异账号时，游戏需要弹出提示框让用户选择需要登录的账号
            YSDKApi.logout();
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnLoginFail", msg);
        }
    }

    @Override
    public void OnRelationNotify(UserRelationRet relationRet) {
        final String lineBreak = "\n";
        StringBuilder builder = new StringBuilder();
        builder.append("flag:").append(relationRet.flag).append(lineBreak)
                .append("msg:").append(relationRet.msg).append(lineBreak)
                .append("platform:").append(relationRet.platform).append(lineBreak);

        if (relationRet.persons != null && relationRet.persons.size() > 0) {
            PersonInfo personInfo = (PersonInfo) relationRet.persons.firstElement();
            builder.append("UserInfoResponse json:").append(lineBreak)
                    .append("nick_name: ").append(personInfo.nickName).append(lineBreak)
                    .append("open_id: ").append(personInfo.openId).append(lineBreak)
                    .append("userId: ").append(personInfo.userId).append(lineBreak)
                    .append("gender: ").append(personInfo.gender).append(lineBreak)
                    .append("picture_small: ").append(personInfo.pictureSmall).append(lineBreak)
                    .append("picture_middle: ").append(personInfo.pictureMiddle).append(lineBreak)
                    .append("picture_large: ").append(personInfo.pictureLarge).append(lineBreak)
                    .append("provice: ").append(personInfo.province).append(lineBreak)
                    .append("city: ").append(personInfo.city).append(lineBreak)
                    .append("country: ").append(personInfo.country).append(lineBreak);
        } else {
            builder.append("relationRet.persons is bad");
        }
        // 发送结果到结果展示界面
        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnRelationNotify", builder.toString());
    }

    @Override
    public void onWindowClose() {
        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "onWindowClose","请重新登录游戏");
    }

    @Override
    public void onSuccess(ShareRet shareRet) {

    }

    @Override
    public void onError(ShareRet shareRet) {

    }

    @Override
    public void onCancel(ShareRet shareRet) {

    }
}
