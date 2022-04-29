#version 330 core

in vec3 Normal;
in vec2 tex;
in vec4 col;
in mat4 proj;
in mat4 mod;
in mat4 vie;


out vec4 FragColor;
out vec2 tes;
out vec3 norms;
out vec4 cols;

uniform sampler2D pic;

void main()
{ 
    norms = Normal;  
    FragColor = texture2D(pic, tex);
    tes = tex;
	cols=col;
}