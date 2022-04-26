#include "ProjNative.h"

DllExport(PJ_CONTEXT*) pCreateContext() {
    return proj_context_create();
}

DllExport(void) pDestroyContext(PJ_CONTEXT* context) {
    proj_context_destroy(context);
}

DllExport(PJ*) pCreateTransformation(PJ_CONTEXT* context, const char* src, const char* dst) {
    auto p = proj_create_crs_to_crs(context, src, dst, NULL);
    proj_normalize_for_visualization(context, p);
    return p;
}

DllExport(void) pDestroyTransformation(PJ* transformation) {
    proj_destroy(transformation);
}

DllExport(bool) pTransform(PJ* transformation, int count, V3d* points, V3d* outPoints) {
    
    PJ_COORD* result = new PJ_COORD[count];
    for(int i = 0; i < count; i++) {
        result[i].xyz.x = points[i].X;
        result[i].xyz.y = points[i].Y;
        result[i].xyz.z = points[i].Z;
    }

    auto err = proj_trans_array(transformation, PJ_FWD, count, result);
    for(int i = 0; i < count; i++) {
        outPoints[i].X = result[i].xyz.x;
        outPoints[i].Y = result[i].xyz.y;
        outPoints[i].Z = result[i].xyz.z;
    }

    return err == 0;
}