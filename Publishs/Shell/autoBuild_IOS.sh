#!/bin/sh

#svn更新
# source /Users/stupiddog/Desktop/work/JSJProject/Shell/svnUpdate.sh
#unity本机安装目录
Build_Unity="/Applications/Unity/Unity.app/Contents/MacOS/Unity"
#项目在本地的目录
Build_Unity_Project=$1
Build_CMD=$2
#日志路径
Build_Log="${Build_Unity_Project}/Shell/build_IOS.log"
#打包平台
Build_Platform="iOS"
echo "是否只打包AB：${Only_Build_AB}"
#打包后工程位置
Build_Out_Path="${Build_Unity_Project}/Publishs/IOS"
echo "开始执行unity脚本"

if [ -f ${Build_Unity_Project}/Publishs/Temps/il2cppbuild.txt ]
then
	rm -r -f ${Build_Unity_Project}/Publishs/Temps/il2cppbuild.txt
fi

#打开unity并执行自动打包函数
${Build_Unity} -quit -batchmode -nographics -projectPath ${Build_Unity_Project} -logFile "${Build_Log}" -buildTarget "${Build_Platform}" -executeMethod TopGame.ED.AutoBuild.Build --out="${Build_Out_Path}"  --externCmd=${Build_CMD}
status=$?
echo '执行状态: '$status''
echo "unity脚本执行完毕！开始打ipa"

if [ -f ${Build_Unity_Project}/Publishs/Temps/il2cppbuild.txt ]
then
	XCodeIL2cpp=$(cat ${PROJECT_BUILD_ROOT}/Publishs/Temps/il2cppbuild.txt)
	if [ -f ${XCodeIL2cpp} ]
	then
		echo "编译ilcpp"
		chmod +x ${Build_Unity_Project}/Tools/HybridCLRData/iOSBuild/build.sh
		cd ${Build_Unity_Project}/Tools/HybridCLRData/iOSBuild
		./build.sh
		cp -r -f ${Build_Unity_Project}/Tools/HybridCLRData/iOSBuild/build/libil2cpp.a ${XCodeIL2cpp}
		echo "复制libil2cpp.a 到 ${XCodeIL2cpp}"
	fi
fi

#native_project_name=Unity-iPhone
#output_path="${Build_Out_Path}/IPA"
#native_ios_project=${Build_Out_Path}

#clean工程
#-project TestAutoPacking.xcodeproj:编译项目名称
#-scheme TestAutoPacking:scheme名称(一般会与你的项目名称相同)
#-configuration Release:(Debug/Release)
#echo "开始clean工程 路径: ${native_ios_project}/JSJ/${native_project_name}.xcodeproj"
#xcodebuild clean -project ${native_ios_project}/JSJ/${native_project_name}.xcodeproj -scheme ${native_project_name} -configuration Release

#archive导出.xcarchive文件
#-project TestAutoPacking.xcodeproj:同clean步骤中一样
#-scheme TestAutoPacking:同clean步骤中一样
#-archivePath /dandy/xmeAutoArchive/TestAutoPacking.xcarchive:导出.xcarchive文件的目录以及文件名称
#PROVISIONING_PROFILE=${provisionName}
#echo "开始导出.xcarchive文件 路径:${output_path}"
#xcodebuild archive -project ${native_ios_project}/JSJ/${native_project_name}.xcodeproj -configuration Release -scheme ${native_project_name} -archivePath ${output_path}/${native_project_name}.xcarchive PROVISIONING_PROFILE=3dadf109-7e0e-4f72-8de9-8eb4a5504a58

#生成ipa
#-archivePath /dandy/xmeAutoArchive/TestAutoPacking.xcarchive:刚刚导出的.xcarchive文件的目录
#-exportPath /dandy/xmeAutoArchive/TestAutoPacking:将要导出的ipa文件的目录以及文件名
# -exportOptionsPlist ${tool_path}/exportOptions.plist
#xcodebuild -exportArchive -archivePath ${output_path}/${native_project_name}.xcarchive -exportPath ${output_path} -exportOptionsPlist ${output_path}
#echo "ipa打包完成!路径:${output_path}"
