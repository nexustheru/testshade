#version 440 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec2 uv;
layout (location = 3) in vec4 colors;

out vec2 uvIn;
out vec4 col;
out vec3 norms;
out mat4 mod;
out mat4 vie;
out mat4 proj;

uniform mat4 projection; 
uniform mat4 view; 
uniform mat4 models;

void main()
{

	uvIn = uv;
	col = colors;
	norms = normal;
	mod = models;
	vie =view;
	proj = projection;
	gl_Position = projection * vec4(position, 50);
}

