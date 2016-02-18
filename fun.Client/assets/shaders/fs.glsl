#version 330

in vec3 position;
in vec2 uv;
in vec3 normal;

uniform vec3 light_position;
//uniform float range;
uniform sampler2D texture;
uniform vec2 resolution;

out vec4 fragment;

void
main (){
	float distance = distance(position.xy, light_position.xy);

	vec3 norm = normal;

	if(light_position.z * resolution.x > distance)
	{
		fragment = vec4(texture2D(texture, uv).xyz, 1);
	}
	else
	{
		fragment = vec4(normal, 0.0); //screw normals
	}
}
