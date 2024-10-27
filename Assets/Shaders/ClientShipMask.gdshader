shader_type canvas_item;

const int size = 4;

uniform vec4 color_from[size] : source_color;
uniform vec4 color_to[size] : source_color;
uniform float tolerance = 0.05;
bool is_close (vec4 a, vec4 b) {
	return abs(a.r - b.r) < tolerance && abs(a.g - b.g) < tolerance && abs(a.b - b.b) < tolerance && abs(a.a - b.a) < tolerance;
}

void fragment() {
	vec4 color = texture(TEXTURE, UV);
	for (int i = 0; i < size; i++) {
		if (is_close(color, color_from[i])) {
			color = color_to[i];
		}
	}
	COLOR = color;
}
