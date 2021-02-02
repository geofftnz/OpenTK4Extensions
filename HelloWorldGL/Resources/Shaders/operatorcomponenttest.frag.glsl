#version 450
precision highp float;
layout (location = 0) in vec2 texcoord;
layout (location = 0) out vec4 out_Colour;

void main(void)
{
	vec2 t = texcoord.xy;

	vec4 col = vec4(t,1-length(t),1.0);
	//vec4 col = vec4(texcoord.x,texcoord.y,0.,1.);
	
	out_Colour = col;	
}