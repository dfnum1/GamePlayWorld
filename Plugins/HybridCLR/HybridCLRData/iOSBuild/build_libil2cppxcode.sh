#!/bin/bash

export HUATUO_IL2CPP_SOURCE_DIR=$(pushd ../LocalIl2CppData-OSXEditor/il2cpp > /dev/null && pwd && popd > /dev/null)
export IPHONESIMULATOR_VERSION=15.0

rm -rf iosbuild

mkdir iosbuild
cd iosbuild
/Applications/CMake.app/Contents/bin/cmake .. -G "Xcode"
