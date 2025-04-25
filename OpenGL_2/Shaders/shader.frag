#version 330 core

in vec3 worldPos;      
in vec2 texCoord;
out vec4 FragColor;    

uniform sampler2D textr; 

void main()
{
    
    float height = worldPos.y / 10.0 / 10.0; // 10 - is max hill height and 10 is just coef 

    vec3 lowColor = vec3(0.1, 0.3, 0.1); 
    vec3 highColor = vec3(0.8, 0.6, 0.2); 
    vec3 finalColor = mix(lowColor, highColor, height);
	
	
	//			mix(lowColor,                 highColor,             height); - last is iterpolate argument
	FragColor = mix(texture(textr, texCoord), vec4(0.8,0.6,0.2,1.0), height);
}