#version 450

in vec3 vPosition;
in vec2 vUV;

uniform mat4 proj;

out vec2 uv;

void
main() {
    gl_Position = vec4(vPosition, 1.0) * proj;
    uv = vUV;
}