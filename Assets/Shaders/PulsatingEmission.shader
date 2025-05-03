Shader "Custom/HeartbeatPulsatingEmission"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _EmissionColor("Emission Color", Color) = (1,1,1,1)
        _Frequency("Pulse Frequency", Float) = 2
        _Amplitude("Pulse Amplitude", Float) = 1
        _BaseEmission("Base Emission", Float) = 0.5
        _PulseSpeed("Pulse Speed", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float4 _BaseColor;
            float4 _EmissionColor;
            float _Frequency;
            float _Amplitude;
            float _BaseEmission;
            float _PulseSpeed;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Heartbeat-like sine pulse that smoothly oscillates between 0 and 1
                float heartbeat = abs(sin(_Time.y * _Frequency)); // Get the sine wave
                heartbeat = pow(heartbeat, _PulseSpeed); // Make the pulse sharper or smoother
                float pulse = heartbeat * _Amplitude + _BaseEmission;
                
                // Apply emission color based on the pulse value
                float3 finalColor = _BaseColor.rgb + (_EmissionColor.rgb * pulse);
                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
