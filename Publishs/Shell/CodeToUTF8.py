import os
import sys
def main(filePath, option):
    for root, dirs, files in os.walk(filePath):
        for file in files:
            if file.endswith(option):
                notepad.open(root+"\\"+file)
                notepad.runMenuCommand("Encoding", "Convert to UTF-8")
                notepad.save()
                notepad.close()
                
if __name__ == '__main__':
    # 将notepad 转英文版
    path = r"D:\work\Project[2022-01-24]\Programs\Client\Assets\Scripts"
    option = ".cs"
    main(path,option)