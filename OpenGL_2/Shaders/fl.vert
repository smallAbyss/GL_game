#version 330 core

layout (location = 0) in vec3 aPos;       
layout (location = 1) in vec3 instancePos;
layout (location = 2) in vec3 instanceColor;
layout (location = 3) in float instanceSize;

out vec3 fColor;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    vec3 scaledPos = aPos * instanceSize;
    gl_Position = vec4(scaledPos + instancePos, 1.0) * model * view * projection;
    //gl_Position = vec4(aPos, 1.0) * model * view * projection;
    fColor = vec3(1.0, 0.8, 0.2);
}
