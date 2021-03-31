#pragma once
#include "reference.h"
#include "graph.h"
namespace fe_engine {
    namespace pathfinding {
        class algorithm_ : public ref_counted {
        public:
            virtual size_t execute(s8vec2 start, s8vec2 end, reference<node>* end_node = NULL) = 0;
            virtual ~algorithm_();
        protected:
            algorithm_(reference<graph> g);
            reference<graph> m_graph;
        private:
            friend struct algorithm;
        };
        struct algorithm {
            reference<algorithm_> m;
            size_t operator()(s8vec2 start, s8vec2 end, reference<node>* end_node = NULL);
            static algorithm create_algorithm(reference<graph> g);
        };
    }
}