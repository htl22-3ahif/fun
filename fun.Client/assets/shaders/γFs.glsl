#version 330

in vec3 position;
in vec2 uv;
in vec3 normal;
in float random;

uniform vec3 light;
uniform float range;
uniform sampler2D texture;
uniform float time;

out vec4 fragment;

float
correctTime (){
	return time;
}

float
rand (){
	return fract(sin(correctTime())*1e4);
}

void
main (){
	vec3 offset = position - light;
	vec3 lightVector = normalize(offset);

	float diffuse = max(dot(normal, lightVector), 0);
	diffuse = diffuse * max(((-1/range) * length(offset) + 1), 0);
	float ambient = 0.3;
	float lighting = max(diffuse, ambient);

	//source: https://www.shadertoy.com/view/ls3Xzf

	vec2 uvCopy = uv;

	vec2 uvR = uvCopy;
	vec2 uvB = uvCopy;

	uvR.x = uvCopy.x * 1.0 - rand() * 0.02 * 0.8;
	uvR.y = uvCopy.y * 1.0 + rand() * 0.02 * 0.8;

	if(uvCopy.y < rand() && uvCopy.y > rand() * -0.1 && sin(correctTime()) < 0.0)
	{
		uvCopy.x = (uvCopy + 0.02 * rand()).x;
	}
	
	vec3 color = vec3(texture2D(texture, uvR).r, texture2D(texture, uvCopy).g, texture2D(texture, uvB).b);

	float scanline = sin(uvCopy.y * 800.0 * rand())/30.0;
	color *= 1.0 - scanline;

	float vegDist = length((0.5, 0.5) - uvCopy);
	color *= 1.0 - vegDist * 0.6;

	fragment = vec4(lighting * color, 1);
}