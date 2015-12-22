#version 330

in vec3 vPosition;
in vec2 vUV;
in vec3 vNormal;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;

out vec2 uv;
out vec3 normal;

void
main(){
	gl_Position = projection * view * world * vec4(vPosition, 1.0);

	uv = vUV;
	normal = normalize((world * vec4(vNormal, 0)).xyz);
}
