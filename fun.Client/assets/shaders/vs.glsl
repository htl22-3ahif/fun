#version 330

in vec3 vPosition;
in vec3 vNormal;
in vec4 vColor;

uniform mat4 world;
uniform mat4 view;
uniform mat4 projection;

out vec4 color;
out vec3 normal;

void
main(){
	gl_Position = projection * view * world * vec4(vPosition, 1.0);

	color = vColor;
	normal = normalize((world * vec4(vNormal, 0)).xyz);
}
