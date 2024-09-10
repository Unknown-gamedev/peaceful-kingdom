shader_type canvas_item;
varying vec2 WorldPos;
uniform vec4 tint : source_color;
uniform float textureScaler :hint_range(0, 1) = 0.02;
void vertex() {
	WorldPos =  (MODEL_MATRIX * vec4(VERTEX,0,1)).xy ;
}

void fragment() {
	vec2 uv = UV + WorldPos * textureScaler;
	vec2 repeatUV = fract(WorldPos * 0.02);
	vec4 text = texture(TEXTURE,uv).xyzw;
	COLOR = vec4(text) * tint;
}
