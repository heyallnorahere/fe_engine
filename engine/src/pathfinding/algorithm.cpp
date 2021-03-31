#include "pathfinding/algorithm.h"
#include <cassert>
#include "astar.h"
namespace fe_engine {
    namespace pathfinding {
        extern bool graph_initialized;
        algorithm_::~algorithm_() { }
        algorithm_::algorithm_(reference<graph> g) {
            this->m_graph = g;
        }
        size_t algorithm::operator()(s8vec2 start, s8vec2 end, reference<node>* end_node) {
            if (!this->m) {
                return (size_t)-1;
            }
            return this->m->execute(start, end, end_node);
        }
        algorithm algorithm::create_algorithm(reference<graph> g) {
            assert(graph_initialized);
            algorithm alg;
#ifdef FEENGINE_PATHFINDING_ALGORITHM_ASTAR
            alg.m = new algorithms::astar(g);
#endif
            return alg;
        }
    }
}