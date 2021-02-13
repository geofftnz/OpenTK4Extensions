#version 450
precision highp float;
layout (location = 0) in vec2 texcoord;
layout (location = 0) out vec4 out_Colour;
uniform sampler2D frameBuffer;

void main() 
{
	out_Colour = vec4(texture2D(frameBuffer,texcoord).rgb,1.0);
	//out_Colour = vec4(texcoord.y*0.3,0.3+texcoord.x*0.3,0.5,1.0);
}
