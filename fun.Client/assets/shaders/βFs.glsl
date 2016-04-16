#version 450

in vec3 position;
in vec2 uv;
in vec3 normal;
in float time;

uniform vec3 light;
uniform float range;
uniform sampler2D texture;

out vec4 fragment;

void
main (){
	vec3 offset = position - light;
	vec3 lightVector = normalize(offset);
	vec3 color;

	float diffuse = max(dot(-normal, lightVector), 0);
	diffuse = diffuse * max(((-1/range) * length(offset) + 1), 0);
	float ambient = 0.3;
	float lighting = max(diffuse, ambient);

	if (lighting > 0.95)
	{
		color = vec3(0.95, 0.95, 0.95);
	}
	else
	{
		if (lighting > 0.5)
		{
			color = vec3(0.5, 0.5, 0.5);
		}
		else
		{
			if (lighting > 0.25)
			{
				color = vec3(0.25, 0.25, 0.25);
			}
			else
			{
				color = vec3(0.1, 0.1, 0.1);
			}
		}
	}

	fragment = vec4(color * texture2D(texture, uv).xyz, 1);
}