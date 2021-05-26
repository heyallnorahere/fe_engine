#include "pch.h"
#include "scripthost.h"
int main(int argc, const char** argv) {
    cxxopts::Options options("FEEngine");
    options.add_options()("f,file", "Path of DLL to load and run", cxxopts::value<std::string>()->default_value("Game.dll"))("e,entrypoint", "The entrypoint class to call on", cxxopts::value<std::string>()->default_value("Game.Entrypoint"))("d,debug", "Debug mode")("a,arg", "Pass an argument to the game", cxxopts::value<std::vector<std::string>>());
    auto result = options.parse(argc, argv);
    std::string path = result["f"].as<std::string>();
    std::string entrypoint = result["e"].as<std::string>();
    std::vector<std::string> args;
    if (result["a"].count() > 0) {
        args = result["a"].as<std::vector<std::string>>();
    }
    bool debug = result["d"].count() > 0;
    std::cout << "Running " << path << " as the entrypoint." << std::endl;
    std::cout << "Calling " << entrypoint << " as the entrypoint class." << std::endl;
    if (args.size() > 0) {
        std::cout << "Game arguments:" << std::endl;
        for (size_t i = 0; i < args.size(); i++) {
            std::cout << "\t" << args[i] << std::endl;
        }
    }
    auto host = std::make_shared<scripthost>();
    auto game_assembly = host->load_assembly(path);
    auto entrypoint_class = game_assembly->get_class(entrypoint);
    auto main_method = entrypoint_class->get_method(":Main()");
    // we dont need the return value
    delete managed_method::call_function(main_method);
    return 0;
}