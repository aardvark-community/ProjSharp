cmake_minimum_required(VERSION 3.9)
include("cmake/Aardvark.cmake")

project(ProjNative)

find_package(PROJ CONFIG REQUIRED)

set(CMAKE_BUILD_TYPE, "Release")
if(UNIX)
    set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -O3 -ffunction-sections -fdata-sections -fvisibility=hidden")
    set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -O3 -ffunction-sections -fdata-sections -fvisibility=hidden")
endif()



set(CMAKE_CXX_STANDARD 14)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
if(APPLE)
    set(CMAKE_SHARED_LINKER_FLAGS "-O3 -flto=thin -dead_strip")
elseif(UNIX)
    set(CMAKE_SHARED_LINKER_FLAGS "-O3 -Wl,--gc-sections -Wl,--strip-all")
elseif(WIN32)
    add_compile_definitions(NOMINMAX)
endif()


add_library(ProjNative SHARED ProjNative.h ProjNative.cpp)
target_link_libraries(ProjNative PRIVATE PROJ::proj)

SET(CMAKE_INSTALL_PREFIX "${CMAKE_CURRENT_SOURCE_DIR}/../../libs/Native/Proj/") 
install(TARGETS ProjNative DESTINATION ${OS}/${ARCH})