#include "pch.h"
#include "scripthost.h"
#include "renderer.h"
static void* read_file(const std::string& path, size_t& buffer_size) {
    FILE* f = fopen(path.c_str(), "rb");
    assert(f);
    fseek(f, 0, SEEK_END);
    size_t file_length = (size_t)ftell(f);
    rewind(f);
    buffer_size = file_length * sizeof(char);
    void* file_data = malloc(buffer_size);
    assert(file_data);
    memset(file_data, 0, buffer_size);
    size_t read_bytes = fread(file_data, sizeof(char), file_length, f);
    assert(read_bytes == file_length);
    fclose(f);
    return file_data;
}
static MonoAssembly* load_assembly_from_file(const std::string& path) {
    size_t buffer_size;
    void* buffer = read_file(path, buffer_size);
    MonoImageOpenStatus status;
    MonoImage* image = mono_image_open_from_data_full((char*)buffer, (size_t)buffer_size, true, &status, false);
    if (status != MONO_IMAGE_OK) {
        return NULL;
    }
    MonoAssembly* assembly = mono_assembly_load_from_full(image, path.c_str(), &status, false);
    mono_image_close(image);
    free(buffer);
    return assembly;
}
scripthost::scripthost(bool debug) {
    this->m_debug = debug;
    this->init();
}
scripthost::~scripthost() {
    this->shutdown();
}
std::shared_ptr<managed_assembly> scripthost::get_core() {
    return this->m_core;
}
std::shared_ptr<managed_assembly> scripthost::load_assembly(const std::string& path) {
    MonoAssembly* assembly = load_assembly_from_file(path);
    return std::shared_ptr<managed_assembly>(new managed_assembly(assembly, this->m_domain));
}
static void register_functions(scripthost* host) {
    host->register_function("FEEngine.Renderer::WriteColoredChar_Native", Renderer_WriteColoredChar);
    host->register_function("FEEngine.Renderer::ClearNativeBuffer_Native", Renderer_ClearNativeBuffer);
    host->register_function("FEEngine.Renderer::Present_Native", Renderer_Present);
    host->register_function("FEEngine.Renderer::DisableCursor_Native", Renderer_DisableCursor);
}
void scripthost::init() {
    mono_set_assemblies_path(MONO_CS_LIBDIR);
    {
        // apparently were not using this????
        auto domain = mono_jit_init("FEEngine");
    }
    char buf[10];
    strcpy(buf, "FEEngine");
    this->m_domain = mono_domain_create_appdomain(buf, NULL);
    MonoDomain* domain = NULL;
    bool cleanup = false;
    if (this->m_domain) {
        domain = mono_domain_create_appdomain(buf, NULL);
        mono_domain_set(domain, false);
        cleanup = true;
    }
    this->m_core = std::shared_ptr<managed_assembly>(new managed_assembly(load_assembly_from_file("FEEngine.dll"), this->m_domain));
    register_functions(this);
    if (cleanup) {
#ifndef FEENGINE_LINUX
        mono_domain_unload(this->m_domain);
#endif
        this->m_domain = domain;
    }
}
void scripthost::shutdown() {
    mono_jit_cleanup(this->m_domain);
}