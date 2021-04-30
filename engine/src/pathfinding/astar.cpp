#include "astar.h"
#include <iostream>
#include <vector>
#include <list>
#include "object_register.h"
namespace fe_engine {
    namespace pathfinding {
        extern std::map<direction, s8vec2> direction_map;
        namespace algorithms {
            astar::astar(reference<graph> g) : algorithm_(g) { }
            void get_child_depth(size_t& count, reference<node> n) {
                if (n->get_parent()) {
                    count++;
                    get_child_depth(count, n->get_parent());
                }
            }
            struct node_struct {
                size_t f(s8vec2 end) {
                    return this->g() + this->h(end);
                }
                size_t g() {
                    size_t count = 0;
                    if (this->m) get_child_depth(count, this->m);
                    return count;
                }
                size_t h(s8vec2 end) {
                    return (size_t)((this->m->get()->get_position() - end).taxicab());
                }
                reference<node> m;
            };
            node_struct find_node_with_least_f(const std::list<node_struct>& list, s8vec2 end) {
                node_struct s;
                for (auto n : list) {
                    if (!s.m) {
                        s = n;
                        continue;
                    }
                    if (n.f(end) < s.f(end)) {
                        s = n;
                    }
                }
                return s;
            }
            bool is_out_of_bounds(s8vec2 pos, reference<map> m) {
                bool x = (pos.x < 0) || (pos.y > m->get_width() - 1);
                bool y = (pos.y < 0) || (pos.y > m->get_height() - 1);
                return x || y;
            }
            size_t astar::execute(s8vec2 start, s8vec2 end, reference<node>* end_node) {
                std::list<node_struct> open, closed;
                auto cells = this->m_graph->get_cells();
                reference<object_register<cell>> cell_register = object_registry::get_register<cell>();
                open.push_back({ reference<node>::create(cell_register->get(cells[start])) });
                while (open.size() > 0) {
                    node_struct q = find_node_with_least_f(open, end);
                    open.remove_if([&](node_struct n) {
                        return q.m.get() == n.m.get();
                    });
                    for (direction d = (direction)0; (int)d < 4; ((int&)d)++) {
                        s8vec2 pos = q.m->get()->get_position() - direction_map[d];
                        auto n = reference<node>::create(cell_register->get(cells[pos]), q.m);
                        q.m->add_child(d, n);
                    }
                    for (auto s : q.m->get_children()) {
                        node_struct n = { s.n };
                        if (s.n->get()->get_position() == end) {
                            if (end_node) {
                                *end_node = s.n;
                            }
                            return n.g();
                        }
                        bool skip = false;
                        for (auto& o : open) {
                            if (n.m->get()->get_position() == o.m->get()->get_position()) {
                                if (o.f(end) < n.f(end)) {
                                    skip = true;
                                    break;
                                }
                            }
                        }
                        for (auto& c : closed) {
                            if (n.m->get()->get_position() == c.m->get()->get_position()) {
                                if (c.f(end) < n.f(end)) {
                                    skip = true;
                                    break;
                                }
                            }
                        }
                        if (is_out_of_bounds(n.m->get()->get_position(), this->m_graph->get_map())) {
                            skip = true;
                        }
                        if (skip) {
                            continue;
                        }
                        open.push_back(n);
                    }
                    closed.push_back(q);
                }
                return (size_t)-1;
            }
        }
    }
}