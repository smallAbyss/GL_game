#version 330 core

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec4 instanceColumn0;
layout (location = 2) in vec4 instanceColumn1;
layout (location = 3) in vec4 instanceColumn2;
layout (location = 4) in vec4 instanceColumn3;
layout (location = 5) in vec3 instanceColor;

out vec3 fColor;

uniform mat4 scale;
uniform mat4 view;
uniform mat4 projection;

void main()
{

    mat4 model = scale * mat4(instanceColumn0, instanceColumn1, instanceColumn2, instanceColumn3) ;
   gl_Position = vec4(aPos, 1.0) * model * view * projection;


    fColor = instanceColor;
}