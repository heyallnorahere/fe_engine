#include "pathfinding/graph.h"
#include "object_register.h"
#include <cassert>
namespace fe_engine {
    namespace pathfinding {
        std::map<direction, s8vec2> direction_map;
        static reference<graph> g_graph;
        bool graph_initialized = false;
        static bool is_border(s8vec2 pos, reference<map> m) {
            bool west = pos.x < 0;
            bool east = pos.x > m->get_width() - 1;
            bool south = pos.y < 0;
            bool north = pos.y > m->get_height() - 1;
            return west || east || south || north;
        }
        cell::cell(s8vec2 pos, reference<map> m) {
            reference<tile> t = m->get_tile(pos);
            this->m_position = pos;
            this->m_properties = t->get_properties();
            for (direction d = (direction)0; (int)d < 4; ((int&)d)++) {
                if (!is_border(this->m_position + direction_map[d], m)) {
                    this->m_edges[d] = edge();
                    this->m_edges[d].state = edge_state::edge_open;
                    this->m_edges[d].destination = (size_t)-1;
                }
            }
        }
        void cell::set_neighbor(direction d, size_t c) {
            if (this->m_edges.find(d) == this->m_edges.end()) {
                return;
            }
            this->m_edges[d].destination = c;
        }
        edge cell::get_edge(direction d) {
            if (this->m_edges.find(d) == this->m_edges.end()) {
                edge e;
                e.state = edge_state::edge_closed;
                return e;
            }
            return this->m_edges[d];
        }
        tile::passing_properties cell::get_passing_properties() {
            return this->m_properties;
        }
        s8vec2 cell::get_position() {
            return this->m_position;
        }
        node::node(reference<cell> c, reference<node> parent) {
            this->m = c;
            this->m_parent = parent;
        }
        reference<cell> node::get() {
            return this->m;
        }
        reference<node> node::get_parent() {
            return this->m_parent;
        }
        std::vector<node::child_struct> node::get_children() {
            return this->m_children;
        }
        void node::add_child(direction d, reference<node> n) {
            this->m_children.push_back({ d, n });
        }
        void graph::init_graph(reference<map> m) {
            if (!object_registry::register_exists<cell>()) {
                object_registry::add_register<cell>();
            }
            direction_map = {
                { direction::north, { 0, 1 } },
                { direction::east, { 1, 0 } },
                { direction::south, { 0, -1 } },
                { direction::west, { -1, 0 } },
            };
            graph_initialized = true;
            g_graph = reference<graph>::create(m);
        }
        reference<graph> graph::get_graph() {
            return g_graph;
        }
        std::unordered_map<s8vec2, size_t, hash_vec2t<int8_t>> graph::get_cells() {
            return this->m_cells;
        }
        reference<map> graph::get_map() {
            return this->m_map;
        }
        graph::graph(reference<map> m) {
            assert(graph_initialized);
            this->m_map = m;
            for (int8_t x = 0; x < m->get_width(); x++) {
                for (int8_t y = 0; y < m->get_height(); y++) {
                    s8vec2 pos = { x, y };
                    auto index = object_registry::get_register<cell>()->add(reference<cell>::create(pos, m));
                    this->m_cells[pos] = index;
                }
            }
        }
    }
}