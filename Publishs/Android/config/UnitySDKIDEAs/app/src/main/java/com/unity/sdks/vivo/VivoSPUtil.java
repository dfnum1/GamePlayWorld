package com.unity.sdks.vivo;
import android.content.Context;
import android.content.SharedPreferences;
public class VivoSPUtil {
    public static final String PRIVACY_PASS = "privacyPass"; //隐私协议是否通过
    private Context mContext;
    private SharedPreferences mSP;
    private static class Holder {
        private static VivoSPUtil sINSTANCE = new VivoSPUtil();
    }
    public static VivoSPUtil getInstance() {
        return Holder.sINSTANCE;
    }
    public void init(Context context) {
        mContext = context;
        mSP = mContext.getSharedPreferences(context.getPackageName(), Context.MODE_PRIVATE);
    }
    private VivoSPUtil(){}

    public void putBoolean(String key, boolean value) {
        mSP.edit().putBoolean(key, value).apply();
    }

    public boolean getBoolean(String key, Boolean defaultValue) {
        return mSP.getBoolean(key, defaultValue);
    }
}
