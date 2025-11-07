#!/bin/bash

rootpath=$1 # CodeSD_Running 的上一级目录，同时需要有ExportOptions.plist文件和 CodeSD_Running 同级存放
project_path=${rootpath}/IOS/$2 # 传递项目路径作为参数1 位置为.plist的上一层目录
scheme_name="Unity-iPhone"
archivePath=${project_path}/${scheme_name}.xcarchive
configuration="Release" # 发布版本Release
project_name="Unity-iPhone.xcodeproj"
build_output_dir=${rootpath}"/IOS/build"
export_plist=${rootpath}/Temps/ExportOptions.plist
if [ ! -f ${export_plist} ]
then
	export_plist=${rootpath}/IOS/ExportOptions.plist
fi

#清理工程
xcodebuild clean -configuration ${configuration}
echo "清理工程"

# 打包(编译,使用新构建系统)
xcodebuild archive -project "${project_path}/${project_name}" -scheme "${scheme_name}" -configuration "$configuration" -UseModernBuildSystem=YES -archivePath "${archivePath}"
echo "build 工程"

# 导出(生成ipa)
xcodebuild -exportArchive -archivePath "${archivePath}" -exportOptionsPlist "${export_plist}" -exportPath "${build_output_dir}"

open ${build_output_dir}

echo "ipa exported successfully"