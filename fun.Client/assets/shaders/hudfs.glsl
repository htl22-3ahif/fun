#version 330

in vec2 uv;

uniform sampler2D texture;

out vec4 fragment;

void
main (){
	fragment = vec4(texture2D(texture, uv).xyz, 1);
}