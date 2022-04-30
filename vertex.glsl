#version 330 core

in vec3 position;
in vec3 normals;
in vec2 texCoord;
in vec4 colorr;

uniform mat4 projection; 
uniform mat4 view; 
uniform mat4 models;


out vec3 Normal;
out vec2 tex;
out vec4 col;
out mat4 proj;
out mat4 mod;
out mat4 vie;


void main()
{
    Normal = normalize(normals);  
    gl_Position = projection * vec4(position, 50);//savethis shit
	tex = texCoord;
	col = colorr;
	proj = projection;
	mod = models;
	vie = view;
}