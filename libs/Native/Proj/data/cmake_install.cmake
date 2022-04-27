# Install script for directory: /Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data

# Set the install prefix
if(NOT DEFINED CMAKE_INSTALL_PREFIX)
  set(CMAKE_INSTALL_PREFIX "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/packages/proj_arm64-osx")
endif()
string(REGEX REPLACE "/$" "" CMAKE_INSTALL_PREFIX "${CMAKE_INSTALL_PREFIX}")

# Set the install configuration name.
if(NOT DEFINED CMAKE_INSTALL_CONFIG_NAME)
  if(BUILD_TYPE)
    string(REGEX REPLACE "^[^A-Za-z0-9_]+" ""
           CMAKE_INSTALL_CONFIG_NAME "${BUILD_TYPE}")
  else()
    set(CMAKE_INSTALL_CONFIG_NAME "Release")
  endif()
  message(STATUS "Install configuration: \"${CMAKE_INSTALL_CONFIG_NAME}\"")
endif()

# Set the component getting installed.
if(NOT CMAKE_INSTALL_COMPONENT)
  if(COMPONENT)
    message(STATUS "Install component: \"${COMPONENT}\"")
    set(CMAKE_INSTALL_COMPONENT "${COMPONENT}")
  else()
    set(CMAKE_INSTALL_COMPONENT)
  endif()
endif()

# Is this installation the result of a crosscompile?
if(NOT DEFINED CMAKE_CROSSCOMPILING)
  set(CMAKE_CROSSCOMPILING "OFF")
endif()

# Set default install directory permissions.
if(NOT DEFINED CMAKE_OBJDUMP)
  set(CMAKE_OBJDUMP "/Applications/Xcode.app/Contents/Developer/Toolchains/XcodeDefault.xctoolchain/usr/bin/objdump")
endif()

if("x${CMAKE_INSTALL_COMPONENT}x" STREQUAL "xUnspecifiedx" OR NOT CMAKE_INSTALL_COMPONENT)
  file(INSTALL DESTINATION "${CMAKE_INSTALL_PREFIX}/share/proj/data" TYPE FILE FILES
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/proj.ini"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/world"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/other.extra"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/nad27"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/GL27"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/nad83"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/nad.lst"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/CH"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/ITRF2000"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/ITRF2008"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/ITRF2014"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/arm64-osx-rel/data/proj.db"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/deformation_model.schema.json"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/projjson.schema.json"
    "/Users/schorsch/Development/ProjSharp/.vcpkg/vcpkg/buildtrees/proj/src/9.0.0-faedbe11fd.clean/data/triangulation.schema.json"
    )
endif()

