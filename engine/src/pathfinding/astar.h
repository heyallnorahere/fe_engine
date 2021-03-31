#pragma once
#include "pathfinding/algorithm.h"
namespace fe_engine {
    namespace pathfinding {
        namespace algorithms {
            class astar : public algorithm_ {
            public:
                astar(reference<graph> g);
                virtual size_t execute(s8vec2 start, s8vec2 end, reference<node>* end_node = NULL) override;
            };
        }
    }
}