#version 330

in vec4 color;
in vec3 normal;

uniform vec3 light_direction;

out vec4 fragment;

void
main (){
	float diffuse = max(dot(normal, light_direction), 0);
	float ambient = 0.3;
	float lighting = max(diffuse, ambient);

	fragment = vec4(color.xyz * lighting, color.a);
}