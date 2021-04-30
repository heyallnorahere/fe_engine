#pragma once
#include "reference.h"
#include "map.h"
#include <vector>
#include <map>
#include <unordered_map>
#include <list>
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
            s8vec2 get_position();
        private:
            std::map<direction, edge> m_edges;
            s8vec2 m_position;
            tile::passing_properties m_properties;
        };
        class node : public ref_counted {
        public:
            struct child_struct {
                direction d;
                reference<node> n;
            };
            node(reference<cell> c, reference<node> parent = reference<node>());
            reference<cell> get();
            reference<node> get_parent();
            std::vector<child_struct> get_children();
            void add_child(direction d, reference<node> n);
        private:
            reference<cell> m;
            reference<node> m_parent;
            std::vector<child_struct> m_children;
        };
        class graph : public ref_counted {
        public:
            static void init_graph(reference<map> m);
            static reference<graph> get_graph();
            std::unordered_map<s8vec2, size_t, hash_vec2t<int8_t>> get_cells();
            reference<map> get_map();
        private:
            graph(reference<map> m);
            size_t m_width, m_height;
            std::unordered_map<s8vec2, size_t, hash_vec2t<int8_t>> m_cells;
            reference<map> m_map;
            friend class reference<graph>;
        };
    }
}