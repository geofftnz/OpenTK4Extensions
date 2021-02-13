#version 450
precision highp float;
layout (location = 0) in vec2 vertex;
layout (location = 0) out vec3 pos;
layout (location = 1) out float size;
//layout (location = 1) out vec2 pos;
uniform float screenFactor;
uniform mat4 projectionMatrix;
uniform mat4 modelMatrix;
uniform mat4 viewMatrix;
uniform sampler2D particlePositionTexture;

void main() 
{
	//gl_Position = projectionMatrix * viewMatrix * modelMatrix * vec4(vertex.x,0.0,vertex.y,1.0);
	//pos = gl_Position.xyz;
	//texcoord = vertex.xy;

	// https://stackoverflow.com/questions/8608844/resizing-point-sprites-based-on-distance-from-the-camera
	//float s = 2.0 * (sqrt(screenWidth) / 28.0);

	//vec4 p = vec4(vertex.x,vertex.y,mod(vertex.x*vertex.y*50.0,1.0),1.0);  //TODO read from texture.

	vec4 ptex = texture2D(particlePositionTexture,vertex.xy);

	vec4 p = vec4(ptex.xyz,1.0);
	float s = ptex.a * screenFactor;

	vec4 eyePos = viewMatrix * modelMatrix * p;
	vec4 corner = projectionMatrix * vec4(s,s,eyePos.z,eyePos.w);
	size = corner.x / corner.w;
	gl_Position = projectionMatrix * eyePos;
	gl_PointSize = size;
	pos = p.xyz;
}
