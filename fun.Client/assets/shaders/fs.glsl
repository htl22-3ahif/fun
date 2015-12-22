#version 330

in vec2 uv;
in vec3 normal;

uniform vec3 light_direction;
uniform sampler2D texture;

out vec4 fragment;

void
main (){
	float diffuse = max(dot(normal, light_direction), 0);
	float ambient = 0.3;
	float lighting = max(diffuse, ambient);

	fragment = vec4(lighting * texture2D(texture, uv).xyz, 1);
}
