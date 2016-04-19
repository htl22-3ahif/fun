#version 400

in vec2 uv;

uniform sampler2D texture;
uniform float time;
uniform vec2 resolution;

out vec4 fragment;

float 
rand () {
    return fract(sin(time)*1e4);
}

void 
main()
{
	vec2 _uv = uv;//gl_FragCoord.xy / resolution.xy;

    vec2 uvR = _uv;
    vec2 uvB = _uv;

    uvR.x = _uv.x * 1.0 - rand() * 0.02 * 0.8;
    uvB.y = _uv.y * 1.0 + rand() * 0.02 * 0.8;
    
    // 
    if(_uv.y < rand() && _uv.y > rand() -0.1 && sin(time) < 0.0)
    {
    	_uv.x = (uv + 0.02 * rand()).x;
    }
    
    //
    vec3 c;
    c.r = texture2D(texture, uvR).r;
    c.g = texture2D(texture, _uv).g;
    c.b = texture2D(texture, uvB).b;
	
    //
    float scanline = sin( _uv.y * 800.0 * rand())/30.0; 
	c *= 1.0 - scanline; 
    
    //vignette
    float vegDist = length(( 0.5 , 0.5 ) - _uv);
    c *= 1.0 - vegDist * 0.6;

    fragment = vec4(c, 1.0);
}