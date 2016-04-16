#version 450

in vec2 uv;

uniform sampler2D texture;

out vec4 fragment;

void
main (){
    fragment = texture2D(texture, uv);
}