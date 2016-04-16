#version 330

in vec3 position;
in vec2 uv;
in vec3 normal;

uniform vec3 light;
uniform float range;
uniform sampler2D texture;

out vec4 fragment;

void
main (){
	vec3 offset = position - light;
	vec3 lightVector = normalize(offset);

	float diffuse = max(dot(-normal, lightVector), 0);
	diffuse = diffuse * max(((-1/range) * length(offset) + 1), 0);
	float ambient = 0.3;
	float lighting = max(diffuse, ambient);

	fragment = vec4(lighting * texture2D(texture, uv).xyz, 1);
}