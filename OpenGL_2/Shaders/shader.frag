#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D textr; /// what is this

void main()
{
    outputColor = texture(textr, texCoord);
}