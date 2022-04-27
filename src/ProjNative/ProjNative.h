
#include <stdio.h>
#include <stdlib.h>
#include <proj.h>
#include <string.h>

#ifdef __APPLE__
#define DllExport(t) extern "C" __attribute__((visibility("default"))) t
#elif __GNUC__
#define DllExport(t) extern "C" __attribute__((visibility("default"))) t
#else
#define DllExport(t) extern "C"  __declspec( dllexport ) t __cdecl
#endif

typedef struct {
    double X;
    double Y; 
} V2d;

typedef struct {
    double X;
    double Y; 
    double Z;
} V3d;

DllExport(void) pInit(const char* searchPath);
DllExport(PJ_CONTEXT*) pCreateContext();
DllExport(void) pDestroyContext(PJ_CONTEXT* context);

DllExport(PJ*) pCreateTransformation(PJ_CONTEXT* context, const char* src, const char* dst);

DllExport(PJ*) pCreateCoordinateSystem(PJ_CONTEXT* context, const char* definition);
DllExport(void) pDestroyCoordinateSystem(PJ* coordinateSystem);
DllExport(PJ*) pCreateCoordinateTransformation(PJ_CONTEXT* context, PJ* src, PJ* dst, const char * const options[]);
DllExport(void) pDestroyTransformation(PJ* transformation);

DllExport(bool) pTransformArray(PJ* transformation, int count, V3d* points, V3d* outPoints);
DllExport(bool) pTransformArray2d(PJ* transformation, int count, V2d* points, V2d* outPoints);