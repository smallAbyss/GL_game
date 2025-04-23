#version 330 core
layout (location = 0) in vec3 aPos;   // the position variable has attribute position 0
layout (location = 1) in vec2 aTexCoord; // the color variable has attribute position 1
  
out vec2 texCoord;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

// uniform mat4 transform;

void main()
{
	texCoord = vec2(aTexCoord.x, aTexCoord.y);

    gl_Position = vec4(aPos, 1.0) * model * view * projection;
}   
