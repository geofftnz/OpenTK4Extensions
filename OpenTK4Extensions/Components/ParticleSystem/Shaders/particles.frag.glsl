#version 450
precision highp float;
//layout (location = 0) in vec2 vertex;
layout (location = 0) in vec3 pos;
layout (location = 1) in float size;
layout (location = 0) out vec4 out_Colour;

void main(void)
{
	vec3 col = vec3(0.1) + vec3(pos);

	vec2 pc = (gl_PointCoord.st - vec2(0.5)) * 2.0;
	float rsq = dot(pc,pc);

	// smooth dots with falloff
	//float a = (1.0 - smoothstep(0.5,1.0,rsq)) * (1.0 / (1.0 + rsq * 10.0));
	
	// 
	float a = (1.0 - smoothstep(0.1,1.0,rsq));


	a *= min(1.0,size*0.1);
	out_Colour = vec4(col,a);
}