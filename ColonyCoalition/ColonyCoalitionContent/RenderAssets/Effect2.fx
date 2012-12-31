float4x4 World;
float4x4 View;
float4x4 Projection;
Texture xTexture;
float3 LightDirection;





//TECHNIQUE for rendering a Textured Model

//For ModelTexture
struct VertexPositionNormalTexture
{
    float4 Position : POSITION;
	 float2 TextureCoords: TEXCOORD0;
	float LightingFactor: TEXCOORD1;
};

//For ModelLight
struct VertexPositionTexture
{
    float4 Position : POSITION;
	 float2 TextureCoords: TEXCOORD0;
};

struct PixelToFrame
{
    float4 Color : COLOR0;
};

// texture sampler
sampler Sampler = sampler_state
{
    Texture = (xTexture);
};


VertexPositionNormalTexture TextureVertexShader( float4 inPosition : POSITION, float4 inNormal: NORMAL,float2 inTextureCoords: TEXCOORD0)
{
    VertexPositionNormalTexture output;

    float4 worldPosition = mul(inPosition, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TextureCoords = inTextureCoords;

	float3 Normal = normalize(mul(normalize(inNormal), World));	

	output.LightingFactor = dot(Normal, -normalize(LightDirection));
    return output;
}

float4 TexturePixelShader(VertexPositionNormalTexture input) : COLOR0
{
    float4 texel;
    float4 pixel;

    texel = tex2D(Sampler, input.TextureCoords);
	pixel.rgb=texel.rgb;
	  

	if(texel.a<1)
		pixel.rgb+=texel.a;
		
	//else
	//{
		pixel.a=1;
		pixel.rgb *= saturate(input.LightingFactor)+.3;
	//}

    return pixel;

} 


technique ModelTexture
{
    pass Pass1
    {
        
        VertexShader = compile vs_2_0 TextureVertexShader();
        PixelShader = compile ps_2_0 TexturePixelShader();
    }
}




//TECHNIQUE for rendering Light defined by Texture Alpha Values
VertexPositionTexture LightVertexShader( float4 inPosition : POSITION, float2 inTextureCoords: TEXCOORD0)
{
    VertexPositionTexture output;

    float4 worldPosition = mul(inPosition, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TextureCoords = inTextureCoords;
    return output;
}



float4 LightPixelShader(VertexPositionTexture input) : COLOR0
{
    float4 texel;
    float4 pixel;

    texel = tex2D(Sampler, input.TextureCoords);
	pixel.rgb=texel.rgb;
	  

	if(texel.a<1)
		pixel= float4(1,1,.7,1);
	else
		pixel= float4(0,0,0,1);



    return pixel;

}

technique ModelLight
{
    pass Pass1
    {
      
        VertexShader = compile vs_2_0 LightVertexShader();
        PixelShader = compile ps_2_0 LightPixelShader();
    }
}





