#pragma once
#include <cmath>
namespace fe_engine {
	// basic vec2 struct, good enough for our purposes
	template<typename T> struct vec2t {
		T x, y;
		bool operator==(const vec2t<T>& other) {
			return (this->x == other.x) && (this->y == other.y);
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
		template<typename U> operator vec2t<U>() {
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
	};
	using u8vec2 = vec2t<unsigned char>;
	using s8vec2 = vec2t<char>;
}