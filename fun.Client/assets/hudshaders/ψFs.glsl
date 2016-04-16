#version 450

in vec2 uv;

uniform sampler2D texture;
uniform float time;

out vec4 fragment;

float
rand (){
	return fract(sin(time)*1e4);
}

void
main (){
	//source: https://www.shadertoy.com/view/ls3Xzf

	vec2 uvCopy = uv;

	vec2 uvR = uvCopy;
	vec2 uvB = uvCopy;

	uvR.x = uvCopy.x * 1.0 - rand() * 0.02 * 0.8;
	uvB.y = uvCopy.y * 1.0 + rand() * 0.02 * 0.8;

	if(uvCopy.y < rand() && uvCopy.y > rand() * -0.1 && sin(time) < 0.0)
	{
		uvCopy.x = (uvCopy + 0.02 * rand()).x;
	}
	
	vec3 color = vec3(texture2D(texture, uvR).r, texture2D(texture, uvCopy).g, texture2D(texture, uvB).b);

	float scanline = sin(uvCopy.y * 800.0 * rand())/30.0;
	color *= 1.0 - scanline;

	float vegDist = length((0.5, 0.5) - uvCopy);
	color *= 1.0 - vegDist * 0.6;

    fragment = vec4(color, 1);
}