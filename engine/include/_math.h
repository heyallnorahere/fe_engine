#pragma once
#include <cmath>
#include <cstdint>
#include <typeinfo>
#include <functional>
namespace fe_engine {
	// basic vec2 struct, good enough for our purposes
	template<typename T> struct vec2t {
		T x, y;
		bool operator==(const vec2t<T>& other) const {
			return (this->x == other.x) && (this->y == other.y);
		}
		template<typename U> bool operator==(const vec2t<U>& other) const {
			return (this->x == (T)other.x) && (this->y == (T)other.y);
		}
		vec2t<T> operator+(const vec2t<T>& other) {
			T x = this->x + other.x;
			T y = this->y + other.y;
			return { x, y };
		}
		const vec2t<T>& operator+=(const vec2t<T>& other) {
			*this = *this + other;
			return *this;
		}
		vec2t<T> operator-(const vec2t<T>& other) {
			T x = this->x - other.x;
			T y = this->y - other.y;
			return { x, y };
		}
		const vec2t<T>& operator-=(const vec2t<T>& other) {
			*this = *this - other;
			return *this;
		}
		vec2t<T> operator*(T other) {
			T x = this->x * other;
			T y = this->y * other;
			return { x, y };
		}
		const vec2t<T>& operator*=(T other) {
			*this = *this * other;
			return *this;
		}
		vec2t<T> operator/(T other) {
			return { this->x / other, this->y / other };
		}
		const vec2t<T>& operator/=(T other) {
			*this = *this / other;
			return *this;
		}
		template<typename U> operator vec2t<U>() const {
			return { (U)this->x, (U)this->y };
		}
		double length() {
			double x = this->x;
			double y = this->y;
			return sqrt(pow(x, 2) + pow(y, 2));
		}
		vec2t<double> normalize() {
			vec2t<double> normal = static_cast<vec2t<double>>(*this) / this->length();
			return normal;
		}
		T taxicab() {
			auto _abs = [](T x) -> T {
				if (typeid(T).hash_code() == typeid(float).hash_code() || typeid(T).hash_code() == typeid(double).hash_code()) {
					return fabs(x);
				} else {
					return abs(x);
				}
			};
			return _abs(this->x) + _abs(this->y);
		}
	};
	template<typename T> struct hash_vec2t {
		using vec2 = vec2t<T>;
		size_t operator()(const vec2& key) const {
			std::hash<T> hasher;
			return hasher(key.x) ^ (hasher(key.y) << 1);
		}
	};
	using u8vec2 = vec2t<uint8_t>;
	using s8vec2 = vec2t<int8_t>;
	using u32vec2 = vec2t<uint32_t>;
	using s32vec2 = vec2t<int32_t>;
	using vec2 = vec2t<double>;
}