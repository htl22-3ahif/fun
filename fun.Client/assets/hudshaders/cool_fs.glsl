#version 450

in vec2 uv;

uniform sampler2D texture;
uniform vec2 screen;

out vec4 fragment;

void
main (){
	vec2 xy = gl_FragCoord.xy / screen;

	if (xy.y < 0.5){
		fragment = texture2D(texture, uv);
	} else {
		fragment = vec4(0,0,0,0);
	}
}