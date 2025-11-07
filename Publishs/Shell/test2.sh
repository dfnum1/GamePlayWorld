#!/bin/bash

echo 'hello '$1''
# source /Users/a2020026/Desktop/work_sd/JSJ/Client/Shell/svnUpdate.sh
echo "hello world $2"
status=$?
echo '执行状态: '$status''
if [${status} -eq 0];then
	echo "执行成功"
else
	echo "执行失败"
	exit 1
fi
# ls -l $0 | awk '{ print $5,$9 }' #打印文件大小和位置,$0表示自身文件
# cp -vf $1 $2 #复制一个文件到领一个位置
#size=$(du -ha ${1}) #将指令赋予变量中进行执行
#echo $size