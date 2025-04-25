﻿#version 330


in vec3 fragPos;
out vec4 fragColor;

void main()
{
    float height = fragPos.y / 10.0; 
    fragColor = mix(vec4(0.1, 0.3, 0.1, 1.0),  
                   vec4(0.8, 0.6, 0.2, 1.0),  
                   height);
}