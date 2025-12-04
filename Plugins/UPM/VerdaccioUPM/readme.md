1.安装node ,要求18 以上

2.适用powershell 安装  verdaccio
 npm i -g verdaccio@latest
 
 确认版本: verdaccio --version
 
3.mkdir D:\VerdaccioUPM
4.cd D:\VerdaccioUPM
5.verdaccio   # 直接回车，看到报错也没关系

6.添加固定ip
  nrm add localnpm http://192.168.1.250:4873
  
  可用 nrm ls 查看有哪些源
  
7.切换到6创建的源
  nrm use localnpm
 
8.创建用npm 登录用户
  npm-cli-login -u admin -p 123456 -e qigui.li@gmail.com -r 

9.执行1.CreateTask.ps1 
  使用管理员权限的powershell
  
10. 2.StartTask.ps1 
  启动

11. 3.ReStartTask.ps1
  重启
  
12. 4.Publish.ps1
	修 Bug →  patch +1  0.1.0 → 0.1.1
	新功能 → minor +1  0.1.0 → 0.2.0
	不兼容 → major +1  0.1.0 → 1.0.0
  发布:
	4.Publish.ps1 为不做任何升级的替换
	4.Publish.ps1 patch 修Bug
	4.Publish.ps1 minor 新功能
	4.Publish.ps1 major 不兼容
	
	-tag 表示git 仓库创建tag版本并提交
	比如 4.Publish.ps1 patch -tag
	
13. unity 安装
	Unity 端添加 Scoped Registry
	Edit → Project Settings → Package Manager → Scoped Registries
	新增：
	Name: Private
	URL: http://192.168.1.100:4873
	Scope(s): com.company
	打开 Package Manager → 左上角选择 My Registries，即可安装/更新你的私有包。	
	
  

