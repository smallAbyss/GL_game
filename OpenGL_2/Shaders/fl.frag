#version 330 core

in vec3 cord;

in vec2 texCoord;
out vec4 FragColor;    

uniform sampler2D texture0; 

void main()
{
	vec3 nc = vec3((sin(cord.x / 5) + 2.01) / 3.01, sin(cord.y), (sin(cord.z / 10) + 1.01) / 2.02);
    FragColor =  texture(texture0, texCoord) * vec4(nc, 1.0);
}