
#include <stdio.h>
#include <stdlib.h>
#include <proj.h>

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
    double Z;
} V3d;

DllExport(PJ_CONTEXT*) pCreateContext();
DllExport(void) pDestroyContext(PJ_CONTEXT* context);
DllExport(PJ*) pCreateTransformation(PJ_CONTEXT* context, const char* src, const char* dst);
DllExport(void) pDestroyTransformation(PJ* transformation);
DllExport(bool) pTransform(PJ* transformation, int count, V3d* points, V3d* outPoints);
