#pragma once
#include "reference.h"
#include <list>
#include <unordered_map>
namespace fe_engine {
	template<typename T> class object_register : public ref_counted {
	public:
		size_t size() const;
		reference<T> get(size_t index) const;
		void add(reference<T> object);
		void remove(size_t index);
	private:
		std::list<reference<T>> m;
	};
	class object_registry {
	public:
		template<typename T> static void add_register();
		template<typename T> static reference<object_register<T>> get_register();
		template<typename T> static bool register_exists();
	private:
		static std::unordered_map<size_t, reference<ref_counted>> m;
	};
	template<typename T> inline size_t object_register<T>::size() const {
		return this->m.size();
	}
	template<typename T> inline reference<T> object_register<T>::get(size_t index) const {
		auto it = this->m.begin();
		std::advance(it, index);
		return *it;
	}
	template<typename T> inline void object_register<T>::add(reference<T> object) {
		this->m.push_back(object);
	}
	template<typename T> inline void object_register<T>::remove(size_t index) {
		auto element = this->get(index);
		this->m.remove_if([&](auto e) { return element.get() == e.get(); });
	}
	template<typename T> inline void object_registry::add_register() {
		m[typeid(T).hash_code()] = reference<object_register<T>>::create();
	}
	template<typename T> inline reference<object_register<T>> object_registry::get_register() {
		return m[typeid(T).hash_code()];
	}
	template<typename T> inline bool object_registry::register_exists() {
		return m[typeid(T).hash_code()];
	}
}