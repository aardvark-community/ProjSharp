#include "ProjNative.h"

DllExport(void) pInit(const char* searchPath) {
    proj_context_set_search_paths(PJ_DEFAULT_CTX, 1, &searchPath);
}

DllExport(PJ_CONTEXT*) pCreateContext() {
    auto ctx = proj_context_create();
    return ctx;
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
DllExport(PJ*) pCreateCoordinateSystem(PJ_CONTEXT* context, const char* definition) {
    auto h = proj_create(context, definition);
    if(proj_is_crs(h)) {
        return h;
    }
    else {
        proj_destroy(h);
        char* def = new char[strlen(definition) + 50];
        sprintf(def, "%s +type=crs", definition);
        h = proj_create(context, def);
        delete[] def;
        return h;
    }
}

DllExport(void) pDestroyCoordinateSystem(PJ* coordinateSystem) {
    proj_destroy(coordinateSystem);
}

DllExport(PJ*) pCreateCoordinateTransformation(PJ_CONTEXT* context, PJ* src, PJ* dst, const char * const options[]) {
    return proj_create_crs_to_crs_from_pj(context, src, dst, NULL, options);
}

DllExport(bool) pTransformArray(PJ* transformation, int count, V3d* points, V3d* outPoints) {
    
    PJ_XYZT* result = new PJ_XYZT[count];
    for(int i = 0; i < count; i++) {
        result[i].x = points[i].X;
        result[i].y = points[i].Y;
        result[i].z = points[i].Z;
        result[i].t = 0.0;
    }

    auto err = proj_trans_array(transformation, PJ_FWD, count, (PJ_COORD*)result);
    for(int i = 0; i < count; i++) {
        outPoints[i].X = result[i].x;
        outPoints[i].Y = result[i].y;
        outPoints[i].Z = result[i].z;
    }
    delete[] result;
    return err == 0;
}

DllExport(bool) pTransformArray2d(PJ* transformation, int count, V2d* points, V2d* outPoints) {
    
    PJ_XYZT* result = new PJ_XYZT[count];
    for(int i = 0; i < count; i++) {
        result[i].x = points[i].X;
        result[i].y = points[i].Y;
        result[i].z = 0.0;
        result[i].t = 0.0;
    }

    auto err = proj_trans_array(transformation, PJ_FWD, count, (PJ_COORD*)result);
    for(int i = 0; i < count; i++) {
        outPoints[i].X = result[i].x;
        outPoints[i].Y = result[i].y;
    }
    delete[] result;
    
    return err == 0;
}