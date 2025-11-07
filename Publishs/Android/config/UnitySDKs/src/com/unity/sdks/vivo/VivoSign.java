package com.unity.sdks.vivo;
import com.unity.sdks.vivo.bean.OrderBean;
import com.vivo.unionsdk.open.VivoPayInfo;
import java.util.HashMap;
public class VivoSign {
    //TODO 有服务器的游戏，请让服务器去计算验签，如果没有服务器，可以通过这段代码去生成验签
    public static String getSignature(OrderBean orderBean)
    {
        HashMap<String, String> params = new HashMap<String, String>();
        params.put(VivoConstant.APP_ID_PARAM, VivoConfig.APP_ID);
        params.put(VivoConstant.CP_ORDER_NUMBER, orderBean.getCpOrderNumber());
        params.put(VivoConstant.EXT_INFO, orderBean.getExtInfo());
        params.put(VivoConstant.NOTIFY_URL, orderBean.getNotifyUrl());
        params.put(VivoConstant.ORDER_AMOUNT, orderBean.getOrderAmount());
        params.put(VivoConstant.PRODUCT_DESC, orderBean.getProductDesc());
        params.put(VivoConstant.PRODUCT_NAME, orderBean.getProductName());

        params.put(VivoConstant.BALANCE, orderBean.getRoleInfoBean().getBalance());
        params.put(VivoConstant.VIP, orderBean.getRoleInfoBean().getVip());
        params.put(VivoConstant.LEVEL, orderBean.getRoleInfoBean().getLevel());
        params.put(VivoConstant.PARTY, orderBean.getRoleInfoBean().getParty());
        params.put(VivoConstant.ROLE_ID, orderBean.getRoleInfoBean().getRoleId());
        params.put(VivoConstant.ROLE_NAME, orderBean.getRoleInfoBean().getRoleName());
        params.put(VivoConstant.SERVER_NAME, orderBean.getRoleInfoBean().getServerName());
        return VivoSignUtils.getVivoSign(params, VivoConfig.APP_KEY);
    }

    /**
     * 登录vivo帐号后，创建VivoPayInfo
     *
     * @param uid       用户id
     * @param orderBean 订单信息
     * @return
     */
    public static VivoPayInfo createPayInfo(String uid , OrderBean orderBean)
    {
        //步骤1：计算支付参数签名
        String signature = getSignature(orderBean);
        //步骤2：创建VivoPayInfo
        VivoPayInfo vivoPayInfo = new VivoPayInfo.Builder()
                //基本支付信息
                .setAppId(VivoConfig.APP_ID)
                .setCpOrderNo(orderBean.getCpOrderNumber())
                .setExtInfo(orderBean.getExtInfo())
                .setNotifyUrl(orderBean.getNotifyUrl())
                .setOrderAmount(orderBean.getOrderAmount())
                .setProductDesc(orderBean.getProductDesc())
                .setProductName(orderBean.getProductName())
                //角色信息
                .setBalance(orderBean.getRoleInfoBean().getBalance())
                .setVipLevel(orderBean.getRoleInfoBean().getVip())
                .setRoleLevel(orderBean.getRoleInfoBean().getLevel())
                .setParty(orderBean.getRoleInfoBean().getParty())
                .setRoleId(orderBean.getRoleInfoBean().getRoleId())
                .setRoleName(orderBean.getRoleInfoBean().getRoleName())
                .setServerName(orderBean.getRoleInfoBean().getServerName())
                //计算出来的参数验签
                .setVivoSignature(signature)
                .build();

        return vivoPayInfo;
    }
}
