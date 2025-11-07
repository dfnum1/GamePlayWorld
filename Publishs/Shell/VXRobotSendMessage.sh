#!/bin/bash
#测试机器人地址
send_url='https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=cf813814-dced-4264-8776-b140b2679db7'
#研发群机器人地址
send_url2='https://qyapi.weixin.qq.com/cgi-bin/webhook/send?key=36037f4d-1294-4842-b9af-985741931ee4'

#打印文件大小
if [ -f ${2} ];then
	content=$(cat ${2})
	echo "存在info文件"
	else
	content=""
	echo "不存在info文件"
fi

#echo $size

curl $send_url2 \
   -H 'Content-Type: application/json' \
   -d "
   {
        \"msgtype\": \"text\",
        \"text\": {
            \"content\": \"$1 \n $content\"
        }
   }"