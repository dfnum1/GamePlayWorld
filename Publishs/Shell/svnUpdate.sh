#!/bin/bash
svn_root=$2
is_revert=$3
if ($3 eq "true")
then
	echo "start svn revert"
	${svn_root}svn revert -R $1
else
	echo "skip svn revert"
fi

echo "start svn resolve!"
#解决冲突,使用文件夹递归所有文件
${svn_root}svn resolve -R --accept theirs-full $1

echo "start svn update"

#更新代码,如果冲突使用他们的
${svn_root}svn update --accept theirs-full $1

echo "start svn resolve!"
#解决冲突,使用文件夹递归所有文件
${svn_root}svn resolve -R --accept theirs-full $1

echo "end svn update"
