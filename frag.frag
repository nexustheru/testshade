#version 440 core

in vec2 uvIn;
in vec4 col;
in vec3 norms;
in mat4 mod;
in mat4 vie;
in mat4 proj;

out vec4 FragColor; 

uniform sampler2D texturee;

void main()
{
	FragColor = texture(texturee, uvIn);
}