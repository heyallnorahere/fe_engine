#pragma once
#include <utility>
#include <type_traits>
namespace fe_engine {
	class ref_counted {
	public:
		void increase_ref_count() const {
			this->m_ref_count++;
		}
		void decrease_ref_count() const {
			this->m_ref_count--;
		}
		size_t get_ref_count() const {
			return this->m_ref_count;
		}
	private:
		mutable size_t m_ref_count = 0;
	};
	template<typename T> class reference {
	public:
		reference() {
			this->m_instance = nullptr;
		}
		reference(std::nullptr_t) {
			this->m_instance = nullptr;
		}
		reference(T* instance) {
			static_assert(std::is_base_of<ref_counted, T>::value, "class is not a child of ref_counted");
			this->m_instance = instance;
			this->increase_ref_count();
		}
		reference(const reference<T>& other) {
			this->m_instance = other.m_instance;
			this->increase_ref_count();
		}
		template<typename U> reference(const reference<U>& other) {
			this->m_instance = (T*)other.m_instance;
			this->increase_ref_count();
		}
		template<typename U> reference(reference<U>&& other) {
			this->m_instance = (T*)other.m_instance;
			other.m_instance = nullptr;
		}
		~reference() {
			this->decrease_ref_count();
		}
		reference& operator=(std::nullptr_t) {
			this->decrease_ref_count();
			this->m_instance = nullptr;
			return *this;
		}
		reference& operator=(const reference<T>& other) {
			other.increase_ref_count();
			this->decrease_ref_count();
			this->m_instance = other.m_instance;
			return *this;
		}
		template<typename U> reference& operator=(const reference<U>& other) {
			other.increase_ref_count();
			this->decrease_ref_count();
			this->m_instance = (T*)other.m_instance;
			return *this;
		}
		template<typename U> reference& operator=(reference<U>&& other) {
			this->decrease_ref_count();
			this->m_instance = (T*)other.m_instance;
			other.m_instance = nullptr;
			return *this;
		}
		operator bool() {
			return this->m_instance;
		}
		operator bool() const {
			return this->m_instance;
		}
		T* operator->() {
			return this->m_instance;
		}
		const T* operator->() const {
			return this->m_instance;
		}
		T& operator*() {
			return *this->m_instance;
		}
		const T& operator*() const {
			return *this->m_instance;
		}
		T* get() {
			return this->m_instance;
		}
		const T* get() const {
			return this->m_instance;
		}
		void reset(T* instance = nullptr) {
			this->decrease_ref_count();
			this->m_instance = instance;
			this->increase_ref_count();
		}
		template<typename... A> static reference<T> create(A&&... args) {
			return reference<T>(new T(std::forward<A>(args)...));
		}
	private:
		void increase_ref_count() const {
			if (this->m_instance) {
				this->m_instance->increase_ref_count();
			}
		}
		void decrease_ref_count() const {
			if (this->m_instance) {
				this->m_instance->decrease_ref_count();
				if (this->m_instance->get_ref_count() == 0) {
					delete this->m_instance;
				}
			}
		}
		T* m_instance;
	};
}