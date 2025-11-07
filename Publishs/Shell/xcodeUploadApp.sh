#!/bin/bash

rootpath=$1 # CodeSD_Running 的上一级目录，同时需要有ExportOptions.plist文件和 CodeSD_Running 同级存放
ipa_name=$2
build_output_dir=${rootpath}"/build"


echo "began validate app !"

# appstore 验证你的生成密钥key https://appstoreconnect.apple.com/access/api
xcrun altool --validate-app -f ${build_output_dir}/${ipa_name}.ipa -t ios -apiKey 2VUZ2VB8G9 -apiIssuer 65cf9fac-3fb1-42ed-bc0d-83eeab9f7cac

echo "began upload app !"

# 上传ipa
xcrun altool --upload-app -f ${build_output_dir}/${ipa_name}.ipa -t ios -apiKey 2VUZ2VB8G9 -apiIssuer 65cf9fac-3fb1-42ed-bc0d-83eeab9f7cac


echo "ipa upload completed!"