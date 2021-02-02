#version 450
precision highp float;
layout (location = 0) in vec3 vertex;
layout (location = 0) out vec2 texcoord;
uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 viewMatrix;

void main() 
{
	gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex.xy,0.0,1.0);
	texcoord = vertex.xy * vec2(0.5,-0.5) + vec2(0.5);  // coordinates to 0-1 range, flip vertical
}
