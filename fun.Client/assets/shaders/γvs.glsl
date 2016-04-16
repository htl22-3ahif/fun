#version 330

in vec3 vPosition;
in vec2 vUV;
in vec3 vNormal;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;

out vec3 position;
out vec2 uv;
out vec3 normal;
out float random;

void
main(){
	vec4 realPosition = world * vec4(vPosition, 1.0);
	gl_Position = projection * view * realPosition;

	position = realPosition.xyz;
	uv = vUV;
	normal = normalize((world * vec4(vNormal, 0)).xyz);
	random = sin(vPosition.x * normal.z);
}
