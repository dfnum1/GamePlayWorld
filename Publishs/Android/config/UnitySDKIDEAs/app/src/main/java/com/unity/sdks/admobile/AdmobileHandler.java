package com.unity.sdks.admobile;

import cn.admobiletop.adsuyi.ADSuyiSdk;
import cn.admobiletop.adsuyi.ad.ADSuyiBannerAd;
import cn.admobiletop.adsuyi.ad.ADSuyiFullScreenVodAd;
import cn.admobiletop.adsuyi.ad.ADSuyiRewardVodAd;
import cn.admobiletop.adsuyi.ad.data.ADSuyiAdInfo;
import cn.admobiletop.adsuyi.ad.data.ADSuyiFullScreenVodAdInfo;
import cn.admobiletop.adsuyi.ad.data.ADSuyiRewardVodAdInfo;
import cn.admobiletop.adsuyi.ad.error.ADSuyiError;
import cn.admobiletop.adsuyi.ad.listener.ADSuyiBannerAdListener;
import cn.admobiletop.adsuyi.ad.listener.ADSuyiFullScreenVodAdListener;
import cn.admobiletop.adsuyi.ad.listener.ADSuyiRewardVodAdListener;
import cn.admobiletop.adsuyi.config.ADSuyiInitConfig;
import com.unity.sdks.ConfigUtl;
import com.unity3d.player.UnityPlayer;

import java.util.Map;

public class AdmobileHandler
{
    public  static boolean Init()
    {
        ADSuyiInitConfig.Builder config = new ADSuyiInitConfig.Builder();
        config.isCanUseWifiState("true".compareTo(ConfigUtl.getMetaData("admobile_usewifi"))==0);
        config.appId(ConfigUtl.getMetaData("admobile_appid"));
        config.isCanUseLocation("true".compareTo(ConfigUtl.getMetaData("admobile_uselocation"))==0);
        config.isCanUsePhoneState("true".compareTo(ConfigUtl.getMetaData("admobile_imei"))==0);
        config.openFloatingAd(true);
        ADSuyiSdk.getInstance().init( UnityPlayer.currentActivity,  config.build() );

        // 设置个性化开关。true为开启、false为关闭，请在初始化ADSuyi后进行控制。
        ADSuyiSdk.setPersonalizedAdEnabled("true".compareTo(ConfigUtl.getMetaData("admobile_personalized"))==0);
        return  false;
    }

    public static void ShowBannerAd(final String posId)
    {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                // TODO Auto-generated method stub
                ADSuyiBannerAd bannerAd = new ADSuyiBannerAd(UnityPlayer.currentActivity,null);
                bannerAd.setListener(new ADSuyiBannerAdListener(){
                    @Override
                    public void onAdReceive(ADSuyiAdInfo adSuyiAdInfo) {
                        // 广告获取成功回调...
                        String msg = "{";
                        msg += "\"type:\"" + "\"Banner\",";
                        msg += "\"platform:\"" + "\"" + adSuyiAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdReceive", msg);
                    }

                    @Override
                    public void onAdExpose(ADSuyiAdInfo adSuyiAdInfo) {
                        // 广告展示回调，有展示回调不一定是有效曝光，如网络等情况导致上报失败
                        String msg = "{";
                        msg += "\"type:\"" + "\"Banner\",";
                        msg += "\"platform:\"" + "\"" + adSuyiAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdExpose", msg);
                    }

                    @Override
                    public void onAdClick(ADSuyiAdInfo adSuyiAdInfo) {
                        // 广告点击回调，有点击回调不一定是有效点击，如网络等情况导致上报失败
                        String msg = "{";
                        msg += "\"type:\"" + "\"Banner\",";
                        msg += "\"platform:\"" + "\"" + adSuyiAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClick", msg);
                    }

                    @Override
                    public void onAdClose(ADSuyiAdInfo adSuyiAdInfo) {
                        // 广告关闭回调
                        String msg = "{";
                        msg += "\"type:\"" + "\"Banner\",";
                        msg += "\"platform:\"" + "\"" + adSuyiAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClose", msg);
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdComplete", msg);
                    }

                    @Override
                    public void onAdFailed(ADSuyiError adSuyiError) {
                        // 广告获取失败回调...
                        String msg = "{";
                        msg += "\"type:\"" + "\"Banner\",";
                        msg += "\"platform:\"" + "\"" + adSuyiError.getError() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiError.getPosId() + "\",";
                        msg += "\"code:\"" + "\"" + adSuyiError.getCode() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdFailed", msg);
                    }
                });
                bannerAd.loadAd(posId);
            }
        });
    }

    public static void ShowRewardAd(final String posId) {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
                // TODO Auto-generated method stub
                ADSuyiRewardVodAd rewardAd = new ADSuyiRewardVodAd(UnityPlayer.currentActivity);
                rewardAd.setListener(new ADSuyiRewardVodAdListener() {
                    @Override
                    public void onAdReceive(ADSuyiRewardVodAdInfo adSuyiAdInfo) {
                        // 广告获取成功回调...
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"platform:\"" + "\"" + adSuyiAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdReceive", msg);
                        adSuyiAdInfo.showRewardVod(UnityPlayer.currentActivity);
                    }

                    @Override
                    public void onVideoCache(ADSuyiRewardVodAdInfo adSuyiRewardVodAdInfo) {
                        // 部分渠道存在激励展示类广告，不会回调该方法，建议在onAdReceive做广告展示处理
                    }

                    @Override
                    public void onVideoComplete(ADSuyiRewardVodAdInfo adSuyiRewardVodAdInfo) {
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"platform:\"" + "\"" + adSuyiRewardVodAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiRewardVodAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdComplete", msg);
                    }

                    @Override
                    public void onVideoError(ADSuyiRewardVodAdInfo adSuyiRewardVodAdInfo, ADSuyiError adSuyiError) {
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"platform:\"" + "\"" + adSuyiRewardVodAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiRewardVodAdInfo.getPlatformPosId() + "\"";
                        msg += "\"code:\"" + "\"" + adSuyiError.getCode() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdError", msg);
                    }

                    @Override
                    public void onReward(ADSuyiRewardVodAdInfo adSuyiRewardVodAdInfo) {
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        for (Map.Entry<String, Object> entry : adSuyiRewardVodAdInfo.getRewardMap().entrySet()) {
                            msg += "\"platform:\"" + "\"" + entry.getValue().toString() + "\",";
                        }
                        msg += "\"platform:\"" + "\"" + adSuyiRewardVodAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiRewardVodAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdReward", msg);
                    }

                    @Override
                    public void onAdExpose(ADSuyiRewardVodAdInfo adSuyiAdInfo) {
                        // 广告展示回调，有展示回调不一定是有效曝光，如网络等情况导致上报失败
                        String msg = "{";
                        msg += "\"type:\"" + "\"Banner\",";
                        msg += "\"platform:\"" + "\"" + adSuyiAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdExpose", msg);
                    }

                    @Override
                    public void onAdClick(ADSuyiRewardVodAdInfo adSuyiAdInfo) {
                        // 广告点击回调，有点击回调不一定是有效点击，如网络等情况导致上报失败
                        String msg = "{";
                        msg += "\"type:\"" + "\"Banner\",";
                        msg += "\"platform:\"" + "\"" + adSuyiAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClick", msg);
                    }

                    @Override
                    public void onAdClose(ADSuyiRewardVodAdInfo adSuyiAdInfo) {
                        // 广告关闭回调
                        String msg = "{";
                        msg += "\"type:\"" + "\"Banner\",";
                        msg += "\"platform:\"" + "\"" + adSuyiAdInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiAdInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClose", msg);
                    }

                    @Override
                    public void onAdFailed(ADSuyiError adSuyiError) {
                        // 广告获取失败回调...
                        String msg = "{";
                        msg += "\"type:\"" + "\"Banner\",";
                        msg += "\"platform:\"" + "\"" + adSuyiError.getError() + "\",";
                        msg += "\"posId:\"" + "\"" + adSuyiError.getPosId() + "\"";
                        msg += "\"code:\"" + "\"" + adSuyiError.getCode() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdFailed", msg);
                    }
                });
                rewardAd.loadAd(posId);
            }
        });
    }

    public static void ShowFullScreenAd(final String posId)
    {
        UnityPlayer.currentActivity.runOnUiThread(new Runnable()
        {
            @Override
            public void run()
            {
                // TODO Auto-generated method stub
                ADSuyiFullScreenVodAd fullAd = new ADSuyiFullScreenVodAd (UnityPlayer.currentActivity);
                // 设置全屏视频监听
                fullAd.setListener(new ADSuyiFullScreenVodAdListener()
                {
                    @Override
                    public void onAdReceive(ADSuyiFullScreenVodAdInfo adInfo)
                    {
                        // 广告获取成功回调...
                        // 全屏视频广告对象一次成功拉取的广告数据只允许展示一次
                        // 广告展示
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreenAd\",";
                        msg += "\"platform:\"" + "\"" + adInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdReceive", msg);

                        adInfo.showFullScreenVod(UnityPlayer.currentActivity);
                    }

                    @Override
                    public void onVideoCache(ADSuyiFullScreenVodAdInfo adInfo) {
                        // 广告视频缓存成功回调...
                    }

                    @Override
                    public void onVideoComplete(ADSuyiFullScreenVodAdInfo adInfo) {
                        // 广告观看完成回调...
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreenAd\",";
                        msg += "\"platform:\"" + "\"" + adInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdComplete", msg);
                    }

                    @Override
                    public void onVideoError(ADSuyiFullScreenVodAdInfo adInfo, ADSuyiError adSuyiError) {
                        // 广告播放错误回调...
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreenAd\",";
                        msg += "\"platform:\"" + "\"" + adInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adInfo.getPlatformPosId() + "\",";
                        msg += "\"error:\"" + "\"" + adSuyiError.getError() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdError", msg);
                    }

                    @Override
                    public void onAdExpose(ADSuyiFullScreenVodAdInfo adInfo) {
                        // 广告展示回调，有展示回调不一定是有效曝光，如网络等情况导致上报失败
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreenAd\",";
                        msg += "\"platform:\"" + "\"" + adInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdExpose", msg);
                    }

                    @Override
                    public void onAdClick(ADSuyiFullScreenVodAdInfo adInfo) {
                        // 广告点击回调，有点击回调不一定是有效点击，如网络等情况导致上报失败
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreenAd\",";
                        msg += "\"platform:\"" + "\"" + adInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClick", msg);
                    }

                    @Override
                    public void onAdClose(ADSuyiFullScreenVodAdInfo adInfo) {
                        // 广告点击关闭回调
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreenAd\",";
                        msg += "\"platform:\"" + "\"" + adInfo.getPlatform() + "\",";
                        msg += "\"posId:\"" + "\"" + adInfo.getPlatformPosId() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClose", msg);
                    }

                    @Override
                    public void onAdFailed(ADSuyiError error) {
                        // 广告获取失败回调...
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreenAd\",";
                        msg += "\"posId:\"" + "\"" + error.getPosId() + "\",";
                        msg += "\"posId:\"" + "\"" + error.getCode() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdFailed", msg);
                    }
                });
                fullAd.loadAd(posId);
            }
        });
    }
}
