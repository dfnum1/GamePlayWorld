#!/bin/sh

#svn更新
# source /Users/stupiddog/Desktop/work/JSJProject/Shell/svnUpdate.sh
#unity本机安装目录
Build_Unity="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
#项目在本地的目录
Build_Unity_Project=$1
Build_CMD=$2
#日志路径
Build_Log="${Build_Unity_Project}/Shell/build_Android.log"
#打包平台
Build_Platform="Android"
#打包后工程位置
Build_Out_Path="${Build_Unity_Project}/Publishs/Android"
echo "开始执行unity脚本"
#打开unity并执行自动打包函数
${Build_Unity} -quit -batchmode -nographics -projectPath ${Build_Unity_Project} -logFile "${Build_Log}" -buildTarget "${Build_Platform}" -executeMethod TopGame.ED.AutoBuild.Build --out="${Build_Out_Path}" --externCmd=${Build_CMD}
echo '执行状态: '$?''
if [ $? -eq 0 ];then
	echo "执行成功"
else
	echo "执行失败"
	exit 1
fi
echo "unity脚本执行结束！"
