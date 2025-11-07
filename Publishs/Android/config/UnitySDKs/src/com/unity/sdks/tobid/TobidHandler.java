package com.unity.sdks.tobid;

import com.sigmob.windad.WindAdOptions;
import com.sigmob.windad.WindAds;
import com.sigmob.windad.WindAdError;
import com.sigmob.windad.WindAds;
import com.sigmob.windad.rewardVideo.WindRewardAdRequest;
import com.sigmob.windad.rewardVideo.WindRewardInfo;
import com.sigmob.windad.rewardVideo.WindRewardVideoAd;
import com.sigmob.windad.rewardVideo.WindRewardVideoAdListener;
import com.sigmob.windad.interstitial.WindInterstitialAd;
import com.sigmob.windad.interstitial.WindInterstitialAdListener;
import com.sigmob.windad.interstitial.WindInterstitialAdRequest;
import com.sigmob.windad.natives.NativeADEventListener;
import com.sigmob.windad.natives.WindNativeAdData;
import com.sigmob.windad.natives.WindNativeAdRequest;
import com.sigmob.windad.natives.WindNativeUnifiedAd;

import com.unity.sdks.ConfigUtl;
import com.unity3d.player.UnityPlayer;

import android.util.Log;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

public class TobidHandler
{
	static WindRewardVideoAd ms_RewardAd = null ;
	static WindInterstitialAd ms_FullScreenAd = null;
    public  static boolean Init()
    {
        WindAds ads = WindAds.sharedAds();
        return ads.startWithOptions(UnityPlayer.currentActivity, new WindAdOptions(ConfigUtl.getMetaData("tobid_appid"), ConfigUtl.getMetaData("tobid_appkey")));
    }
    
    public static void requestPermission()
    {
    	WindAds.requestPermission(UnityPlayer.currentActivity);
    }
    

    public static void ShowBannerAd(final String posId)
    {
        String msg = "{";
        msg += "\"type:\"" + "\"Banner\",";
        msg += "\"error:\"" + "\"404\"";
        msg += "}";
        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnShowError", msg);
    }

    public static void ShowRewardAd(final String posId) {
    	if(ms_RewardAd!=null) {
            String msg = "{";
            msg += "\"type:\"" + "\"Reward\",";
            msg += "\"error:\"" + "\"444\"";
            msg += "}";
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnShowError", msg);
    		return;
    	}
        UnityPlayer.currentActivity.runOnUiThread(new Runnable() {
            @Override
            public void run() {
            	String userID = ConfigUtl.getRuntimeData("user_id");
            	if(userID == null) userID = "";
            	
                // TODO Auto-generated method stub
            	Map<String, Object> options = new HashMap<String,Object>();
                options.put("user_id", userID);
                ms_RewardAd = new WindRewardVideoAd(new WindRewardAdRequest(posId, userID, options));
                ms_RewardAd.setWindRewardVideoAdListener(new WindRewardVideoAdListener() {
                    @Override
                    public void onRewardAdLoadSuccess(final String placementId) {
                        // 广告获取成功回调...
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + placementId + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdLoadSuccess", msg);
                        
                        if(ms_RewardAd!=null)
                        {
                        	 HashMap showOption = new HashMap();
                        	 String adTitle = ConfigUtl.getRuntimeData("ad_title");
                        	 if(adTitle == null)adTitle = "";
                        	 String adDesc = ConfigUtl.getRuntimeData("ad_desc");
                        	 if(adDesc == null)adDesc = "";
                        	 
                             showOption.put(WindAds.AD_SCENE_ID, adTitle);
                             showOption.put(WindAds.AD_SCENE_DESC, adDesc);
                             ms_RewardAd.show(showOption);
                        }
                    }

                    @Override
                    public void onRewardAdPreLoadSuccess(final String s) {
                    	Log.d("windSDK", "------onRewardAdPreLoadSuccess------" + s);
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + s + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdPreLoadSuccess", msg);
                    }
                    
                    @Override
                    public void onRewardAdPreLoadFail(final String s) {
                    	Log.d("windSDK", "------onRewardAdPreLoadFail------" + s);
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + s + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdPreLoadFail", msg);
                        ms_RewardAd = null;
                    }

                    
                    @Override
                    public void onRewardAdPlayEnd(final String placementId) {
                        Log.d("windSDK", "------onRewardAdPlayEnd------" + placementId);
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + placementId + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdPlayEnd", msg);
                        ms_RewardAd = null;
                    }
                    
                    @Override
                    public void onRewardAdPlayStart(final String placementId) {
                        Log.d("windSDK", "------onRewardAdPlayStart------" + placementId);
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + placementId + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdPlayStart", msg);
                    }

                    @Override
                    public void onRewardAdClicked(final String placementId) {
                        Log.d("windSDK", "------onRewardAdClicked------" + placementId);
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + placementId + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClicked", msg);
                    }

                    @Override
                    public void onRewardAdClosed(final String placementId) {
                        Log.d("windSDK", "------onRewardAdClosed------" + placementId);
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + placementId + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClosed", msg);
                        ms_RewardAd = null;
                    }

                    @Override
                    public void onRewardAdRewarded(WindRewardInfo rewardInfo, String placementId) {
                        Log.d("windSDK", "------onRewardAdRewarded------" + rewardInfo.toString() + ":" + placementId);
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + placementId + "\",";
                        msg += "\"rewards:\"" + "\"" + rewardInfo.toString() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdRewarded", msg);
                    }

                    @Override
                    public void onRewardAdLoadError(WindAdError error, String placementId) {
                        Log.d("windSDK", "------onRewardAdLoadError------" + error.toString() + ":" + placementId);
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + placementId + "\",";
                        msg += "\"error:\"" + "\"" + error.toString() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdLoadError", msg);
                        ms_RewardAd = null;
                    }

                    @Override
                    public void onRewardAdPlayError(WindAdError error, String placementId) {
                        Log.d("windSDK", "------onRewardAdPlayError------" + error.toString() + ":" + placementId);
                        String msg = "{";
                        msg += "\"type:\"" + "\"Reward\",";
                        msg += "\"posId:\"" + "\"" + placementId + "\",";
                        msg += "\"error:\"" + "\"" + error.toString() + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdPlayError", msg);
                        ms_RewardAd = null;
                    }
                });
                if(ms_RewardAd!=null)ms_RewardAd.loadAd();
            }
        });
    }

    public static void ShowFullScreenAd(final String posId)
    {
    	if(ms_FullScreenAd !=null) {
            String msg = "{";
            msg += "\"type:\"" + "\"FullScreen\",";
            msg += "\"error:\"" + "\"444\"";
            msg += "}";
            UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnShowError", msg);
    		return;
    	}
        UnityPlayer.currentActivity.runOnUiThread(new Runnable()
        {
            @Override
            public void run()
            {
            	String userID = ConfigUtl.getRuntimeData("user_id");
            	if(userID == null) userID = "";
            	Map<String, Object> options = new HashMap<String,Object>();
                options.put("user_id", userID);
                
            	ms_FullScreenAd = new WindInterstitialAd( new WindInterstitialAdRequest(posId, userID, options));
            	ms_FullScreenAd.setWindInterstitialAdListener(new WindInterstitialAdListener() {

					@Override
					public void onInterstitialAdClicked(String arg0) {
						// TODO Auto-generated method stub
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreen\",";
                        msg += "\"posId:\"" + "\"" + arg0 + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClicked", msg);
					}

					@Override
					public void onInterstitialAdClosed(String arg0) {
						// TODO Auto-generated method stub
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreen\",";
                        msg += "\"posId:\"" + "\"" + arg0 + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdClosed", msg);
                        ms_FullScreenAd = null;
					}

					@Override
					public void onInterstitialAdLoadError(WindAdError arg0, String arg1) {
						// TODO Auto-generated method stub
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreen\",";
                        msg += "\"posId:\"" + "\"" + arg1 + "\"";
                        msg += "\"error:\"" + "\"" + arg0 + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdLoadError", msg);
                        ms_FullScreenAd = null;
					}

					@Override
					public void onInterstitialAdLoadSuccess(String arg0) {
						// TODO Auto-generated method stub
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreen\",";
                        msg += "\"posId:\"" + "\"" + arg0 + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdLoadSuccess", msg);
                        if(ms_FullScreenAd!=null)
                        {
                        	 HashMap showOption = new HashMap();
                        	 String adTitle = ConfigUtl.getRuntimeData("ad_title");
                        	 if(adTitle == null)adTitle = "";
                        	 String adDesc = ConfigUtl.getRuntimeData("ad_desc");
                        	 if(adDesc == null)adDesc = "";
                        	 
                             showOption.put(WindAds.AD_SCENE_ID, adTitle);
                             showOption.put(WindAds.AD_SCENE_DESC, adDesc);
                        	ms_FullScreenAd.show(showOption);
                        }
					}

					@Override
					public void onInterstitialAdPlayEnd(String arg0) {
						// TODO Auto-generated method stub
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreen\",";
                        msg += "\"posId:\"" + "\"" + arg0 + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdPlayEnd", msg);
                        ms_FullScreenAd = null;
					}

					@Override
					public void onInterstitialAdPlayError(WindAdError arg0, String arg1) {
						// TODO Auto-generated method stub
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreen\",";
                        msg += "\"error:\"" + "\"" + arg0 + "\"";
                        msg += "\"posId:\"" + "\"" + arg1 + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdPlayError", msg);
                        ms_FullScreenAd = null;
					}

					@Override
					public void onInterstitialAdPlayStart(String arg0) {
						// TODO Auto-generated method stub
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreen\",";
                        msg += "\"posId:\"" + "\"" + arg0 + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdPlayStart", msg);
					}

					@Override
					public void onInterstitialAdPreLoadFail(String arg0) {
						// 广告填充失败 ,placementId 为回调广告位
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreen\",";
                        msg += "\"posId:\"" + "\"" + arg0 + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnAdPreLoadFail", msg);
                        ms_FullScreenAd = null;
					}

					@Override
					public void onInterstitialAdPreLoadSuccess(String arg0) {
						//广告服务填充成功，placementId 为回调广告位
                        String msg = "{";
                        msg += "\"type:\"" + "\"FullScreen\",";
                        msg += "\"posId:\"" + "\"" + arg0 + "\"";
                        msg += "}";
                        UnityPlayer.UnitySendMessage(ConfigUtl.UnityListener, "OnPreLoadSuccess", msg);
					}
            		
            	});
            	if(ms_FullScreenAd!=null)ms_FullScreenAd.loadAd();
            }
        });
    }
}
