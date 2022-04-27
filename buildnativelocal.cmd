@echo off

git clone https://github.com/Microsoft/vcpkg.git ./.vcpkg/vcpkg --depth 1
copy .vcpkg\vcpkg\triplets\community\x64-windows-static-md.cmake .vcpkg\vcpkg\triplets\community\x64-windows-static-md-rel.cmake
echo set(VCPKG_BUILD_TYPE release) >> .vcpkg\vcpkg\triplets\community\x64-windows-static-md-rel.cmake

cmd /C ".vcpkg\vcpkg\bootstrap-vcpkg.bat -disableMetrics"

.vcpkg\vcpkg\vcpkg.exe install proj --triplet x64-windows-static-md-rel

cmake -S src\ProjNative -B src\ProjNative\build -DCMAKE_TOOLCHAIN_FILE="%~dp0\.vcpkg\vcpkg\scripts\buildsystems\vcpkg.cmake" -DVCPKG_TARGET_TRIPLET=x64-windows-static-md-rel -DCMAKE_BUILD_TYPE=Release

cmake --build src\ProjNative\build --config Release --target install