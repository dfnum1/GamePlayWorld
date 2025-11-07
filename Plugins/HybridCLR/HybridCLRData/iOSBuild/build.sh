#!/bin/bash

export HUATUO_IL2CPP_SOURCE_DIR=$(pushd ../LocalIl2CppData-OSXEditor/il2cpp > /dev/null && pwd && popd > /dev/null)
export IPHONESIMULATOR_VERSION=

rm -rf build

mkdir build
cd build
/Applications/CMake.app/Contents/bin/cmake -DCMAKE_SYSTEM_PROCESSOR=arm64 -DCMAKE_OSX_ARCHITECTURES=arm64 .. 
make -j24