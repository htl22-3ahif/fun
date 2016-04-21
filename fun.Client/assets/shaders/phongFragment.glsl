#version 400

in vec3 position;
in vec2 uv;
in vec3 normal;

uniform vec3 light;
uniform sampler2D texture;
uniform float range;

const float ambient = 0.3;

out vec4 fragment;

void
main (){
	vec3 offset = position - light;
	vec3 lightVector = normalize(offset);

	float lambertian = max(dot(lightVector, -normal), 0.0);	
	lambertian *= max(((-1/range) * length(offset) + 1), 0);
	float specular = 0.0;

	if(lambertian > 0.0){
		vec3 viewVector = normalize(-position);

		vec3 halfVector = normalize(lightVector + viewVector);
		float specAngle = max(dot(halfVector, normal), 0.0);
		specular = pow(specAngle, 16.0);
	}

	vec3 samplue = texture2D(texture, uv).xyz;

	fragment = vec4(ambient * samplue.xyz + lambertian * samplue.xyz + specular, 1.0);
}