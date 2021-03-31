#include "astar.h"
#include <iostream>
#include <vector>
#include <string>
namespace fe_engine {
    namespace pathfinding {
        namespace algorithms {
            template<typename T> void print(const T& val) {
                std::cout << val << std::endl;
            }
            template<typename T> void print(const std::vector<T>& v) {
                std::cout << "vector size: " << v.size() << std::endl;
                for (size_t i = 0; i < v.size(); i++) {
                    std::cout << "element " << std::to_string(i) << ": ";
                    print(v[i]);
                }
            }
            astar::astar(reference<graph> g) : algorithm_(g) { }
            size_t astar::execute(s8vec2 start, s8vec2 end, reference<node>* end_node) {
                // todo: implement astar
                return 0;
            }
        }
    }
}