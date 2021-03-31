#pragma once
#include "reference.h"
#include "map.h"
#include <vector>
#include <map>
#include <unordered_map>
namespace fe_engine {
    namespace pathfinding {
        enum class direction {
            north,
            east,
            south,
            west,
        };
        enum class edge_state {
            edge_open,
            edge_closed,
        };
        struct edge {
            edge_state state;
            size_t destination;
        };
        class cell : public ref_counted {
        public:
            cell(s8vec2 pos, reference<map> m);
            void set_neighbor(direction d, size_t c);
            edge get_edge(direction d);
            tile::passing_properties get_passing_properties();
        private:
            std::map<direction, edge> m_edges;
            s8vec2 m_position;
            tile::passing_properties m_properties;
        };
        class node : public ref_counted {
        public:
            node(reference<cell> c, reference<node> parent = reference<node>());
            reference<cell> get();
            reference<node> get_parent();
        private:
            reference<cell> m;
            reference<node> m_parent;
        };
        class graph : public ref_counted {
        public:
            static void init_graph(reference<map> m);
            static reference<graph> get_graph();
        private:
            graph(reference<map> m);
            size_t m_width, m_height;
            std::unordered_map<s8vec2, size_t, hash_vec2t<int8_t>> m_cells;
            friend class reference<graph>;
        };
    }
}