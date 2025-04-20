#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0; /// what is this

void main()
{
    outputColor = texture(texture0, texCoord);
}