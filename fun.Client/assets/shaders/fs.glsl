#version 330

in vec3 position;
in vec2 uv;
//in vec3 normal;
// please send help

uniform vec3 light_position;
//uniform float range; not needed currently
uniform sampler2D texture;
uniform vec2 resolution;

out vec4 fragment;

void
main (){
	// pixel position as a decimal (position would be anything from 0, 0 to 1920, 1080 in our case)
	vec3 actualPosition = vec3(position.xy / resolution.xy, position.z);
	// seriously screw normals
	//vec3 norm = normal;

	// distance determines wether our current pixel is affected by light affects
	// we used position instead of actual position because light position will have similar values (ranging from 0, 0 to 1920, 1080, in our case)
	float distance = distance(position.xy, light_position.xy);

	// our current pixel mapped to the texture
	vec4 color = vec4(texture2D(texture, uv).xyz, 1.0);

	// apply gradient, using distance as our factor
	// this works because distance is the distance in pixels between the current pixel and the light source. The term (light_position.z * resolution.x)
	// is the radius length. https://cms-assets.tutsplus.com/uploads/users/728/posts/24351/image/equation.png
	fragment = color * (1.0 - (distance / (light_position.z * resolution.x)) * 2.0); 
}
