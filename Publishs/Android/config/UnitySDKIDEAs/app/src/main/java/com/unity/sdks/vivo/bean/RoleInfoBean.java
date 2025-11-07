package com.unity.sdks.vivo.bean;

public class RoleInfoBean {
    private String balance;  //用户余额
    private String vip;  //用户vip等级
    private String level; //用户角色等级
    private String party;  //工会
    private String roleId; //角色ID
    private String roleName; //角色名
    private String serverName; //区服信息

    public  RoleInfoBean()
    {

    }

    public RoleInfoBean(String balance, String vip, String level, String party, String roleId, String roleName, String serverName) {
        this.balance = balance;
        this.vip = vip;
        this.level = level;
        this.party = party;
        this.roleId = roleId;
        this.roleName = roleName;
        this.serverName = serverName;
    }

    public String getBalance() {
        return balance;
    }
    public void setBalance(String val) {
        balance = val;
    }

    public String getVip() {
        return vip;
    }
    public void setVip(String val) {
        vip = val;
    }

    public String getLevel() {
        return level;
    }
    public void setLevel(String val) {
        level = val;
    }

    public String getParty() {
        return party;
    }
    public void setParty(String val) {
        party = val;
    }

    public String getRoleId() {
        return roleId;
    }
    public void setRoleId(String val) {
        roleId = val;
    }

    public String getRoleName() {
        return roleName;
    }
    public void setRoleName(String val) {
        roleName = val;
    }
    public String getServerName() {
        return serverName;
    }
    public void setServerName(String val) {
        serverName = val;
    }

    @Override
    public String toString() {
        return "RoleInfoBean{" +
                "balance='" + balance + '\'' +
                ", vip='" + vip + '\'' +
                ", level='" + level + '\'' +
                ", party='" + party + '\'' +
                ", roleId='" + roleId + '\'' +
                ", roleName='" + roleName + '\'' +
                ", serverName='" + serverName + '\'' +
                '}';
    }
}
