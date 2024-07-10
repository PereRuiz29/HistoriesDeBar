Shader "Unlit/FluidShader"
{
    Properties
    {
        [NoScaleOffset] _MainTex("MainTex", 2D) = "white" {}
        _Intensity("Intensity", Float) = 0.2
        _BlurAmount("BlurAmount", Float) = 0.48
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
            "DisableBatching" = "False"
            "ShaderGraphShader" = "true"
            "ShaderGraphTargetId" = "UniversalUnlitSubTarget"
        }

        Pass
        {
            Name "Universal Forward"
            Tags
            {
            // LightMode: <None>
        }

        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag

        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>

        // Defines

        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        #define _ALPHATEST_ON 1


        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float3 positionWS : INTERP1;
             float3 normalWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

        PackedVaryings PackVaryings(Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

        Varyings UnpackVaryings(PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }


        // --------------------------------------------------
        // Graph

        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Intensity;
        float _BlurAmount;
        CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Graph Includes
            // GraphIncludes: <None>

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }

            void Unity_Negate_float2(float2 In, out float2 Out)
            {
                Out = -1 * In;
            }

            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }

            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }

            void Unity_Negate_float(float In, out float Out)
            {
                Out = -1 * In;
            }

            void Unity_Divide_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A / B;
            }

            void Unity_Step_float(float Edge, float In, out float Out)
            {
                Out = step(Edge, In);
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
                float AlphaClipThreshold;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                UnityTexture2D _Property_235d49feb144473fb704288a7fc6a286_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float _Property_7803b9889f694b7daea3ee068aad8413_Out_0_Float = _BlurAmount;
                float _Multiply_0cc5921c9738468a898042b9f4433ed9_Out_2_Float;
                Unity_Multiply_float_float(_Property_7803b9889f694b7daea3ee068aad8413_Out_0_Float, 0.001, _Multiply_0cc5921c9738468a898042b9f4433ed9_Out_2_Float);
                float4 _Combine_18686f10e2c944d3bac8bfc858ea20bf_RGBA_4_Vector4;
                float3 _Combine_18686f10e2c944d3bac8bfc858ea20bf_RGB_5_Vector3;
                float2 _Combine_18686f10e2c944d3bac8bfc858ea20bf_RG_6_Vector2;
                Unity_Combine_float(_Multiply_0cc5921c9738468a898042b9f4433ed9_Out_2_Float, 0, 0, 0, _Combine_18686f10e2c944d3bac8bfc858ea20bf_RGBA_4_Vector4, _Combine_18686f10e2c944d3bac8bfc858ea20bf_RGB_5_Vector3, _Combine_18686f10e2c944d3bac8bfc858ea20bf_RG_6_Vector2);
                float2 _Negate_ad2c9950820c4b8f91f8f12fbfd50781_Out_1_Vector2;
                Unity_Negate_float2(_Combine_18686f10e2c944d3bac8bfc858ea20bf_RG_6_Vector2, _Negate_ad2c9950820c4b8f91f8f12fbfd50781_Out_1_Vector2);
                float2 _TilingAndOffset_9083aa5bfd16494d8ca785b23edd480f_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Negate_ad2c9950820c4b8f91f8f12fbfd50781_Out_1_Vector2, _TilingAndOffset_9083aa5bfd16494d8ca785b23edd480f_Out_3_Vector2);
                float4 _SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_235d49feb144473fb704288a7fc6a286_Out_0_Texture2D.tex, _Property_235d49feb144473fb704288a7fc6a286_Out_0_Texture2D.samplerstate, _Property_235d49feb144473fb704288a7fc6a286_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_9083aa5bfd16494d8ca785b23edd480f_Out_3_Vector2));
                float _SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_R_4_Float = _SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_RGBA_0_Vector4.r;
                float _SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_G_5_Float = _SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_RGBA_0_Vector4.g;
                float _SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_B_6_Float = _SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_RGBA_0_Vector4.b;
                float _SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_A_7_Float = _SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_RGBA_0_Vector4.a;
                UnityTexture2D _Property_a3c1a4f3894f4509960805e96ab60b94_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float2 _TilingAndOffset_3bb7cc94e9674fc38e54b8491a6013da_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Combine_18686f10e2c944d3bac8bfc858ea20bf_RG_6_Vector2, _TilingAndOffset_3bb7cc94e9674fc38e54b8491a6013da_Out_3_Vector2);
                float4 _SampleTexture2D_981483030cd845fbb920bb32147be48e_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_a3c1a4f3894f4509960805e96ab60b94_Out_0_Texture2D.tex, _Property_a3c1a4f3894f4509960805e96ab60b94_Out_0_Texture2D.samplerstate, _Property_a3c1a4f3894f4509960805e96ab60b94_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_3bb7cc94e9674fc38e54b8491a6013da_Out_3_Vector2));
                float _SampleTexture2D_981483030cd845fbb920bb32147be48e_R_4_Float = _SampleTexture2D_981483030cd845fbb920bb32147be48e_RGBA_0_Vector4.r;
                float _SampleTexture2D_981483030cd845fbb920bb32147be48e_G_5_Float = _SampleTexture2D_981483030cd845fbb920bb32147be48e_RGBA_0_Vector4.g;
                float _SampleTexture2D_981483030cd845fbb920bb32147be48e_B_6_Float = _SampleTexture2D_981483030cd845fbb920bb32147be48e_RGBA_0_Vector4.b;
                float _SampleTexture2D_981483030cd845fbb920bb32147be48e_A_7_Float = _SampleTexture2D_981483030cd845fbb920bb32147be48e_RGBA_0_Vector4.a;
                float4 _Add_ca78ed20ea1d4860a5bf21342e43374f_Out_2_Vector4;
                Unity_Add_float4(_SampleTexture2D_188aec95cb6045b39071d07d5023f2e2_RGBA_0_Vector4, _SampleTexture2D_981483030cd845fbb920bb32147be48e_RGBA_0_Vector4, _Add_ca78ed20ea1d4860a5bf21342e43374f_Out_2_Vector4);
                UnityTexture2D _Property_34013744208c49b38249b396c0f810a4_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float4 _Combine_143b2529a55c4d1c874b1ab344e8b5d1_RGBA_4_Vector4;
                float3 _Combine_143b2529a55c4d1c874b1ab344e8b5d1_RGB_5_Vector3;
                float2 _Combine_143b2529a55c4d1c874b1ab344e8b5d1_RG_6_Vector2;
                Unity_Combine_float(0, _Multiply_0cc5921c9738468a898042b9f4433ed9_Out_2_Float, 0, 0, _Combine_143b2529a55c4d1c874b1ab344e8b5d1_RGBA_4_Vector4, _Combine_143b2529a55c4d1c874b1ab344e8b5d1_RGB_5_Vector3, _Combine_143b2529a55c4d1c874b1ab344e8b5d1_RG_6_Vector2);
                float2 _Negate_c82a13584a4e4b19be0938d78df9a9fa_Out_1_Vector2;
                Unity_Negate_float2(_Combine_143b2529a55c4d1c874b1ab344e8b5d1_RG_6_Vector2, _Negate_c82a13584a4e4b19be0938d78df9a9fa_Out_1_Vector2);
                float2 _TilingAndOffset_b04dcc7207324c9ca3334a9cd8a96ffc_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Negate_c82a13584a4e4b19be0938d78df9a9fa_Out_1_Vector2, _TilingAndOffset_b04dcc7207324c9ca3334a9cd8a96ffc_Out_3_Vector2);
                float4 _SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_34013744208c49b38249b396c0f810a4_Out_0_Texture2D.tex, _Property_34013744208c49b38249b396c0f810a4_Out_0_Texture2D.samplerstate, _Property_34013744208c49b38249b396c0f810a4_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_b04dcc7207324c9ca3334a9cd8a96ffc_Out_3_Vector2));
                float _SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_R_4_Float = _SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_RGBA_0_Vector4.r;
                float _SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_G_5_Float = _SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_RGBA_0_Vector4.g;
                float _SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_B_6_Float = _SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_RGBA_0_Vector4.b;
                float _SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_A_7_Float = _SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_RGBA_0_Vector4.a;
                UnityTexture2D _Property_6f97448f27f84c6990bab0614d8cd555_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float2 _TilingAndOffset_2eee2ca565c8405bb0b714d8c7bf747a_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Combine_143b2529a55c4d1c874b1ab344e8b5d1_RG_6_Vector2, _TilingAndOffset_2eee2ca565c8405bb0b714d8c7bf747a_Out_3_Vector2);
                float4 _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_6f97448f27f84c6990bab0614d8cd555_Out_0_Texture2D.tex, _Property_6f97448f27f84c6990bab0614d8cd555_Out_0_Texture2D.samplerstate, _Property_6f97448f27f84c6990bab0614d8cd555_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_2eee2ca565c8405bb0b714d8c7bf747a_Out_3_Vector2));
                float _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_R_4_Float = _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_RGBA_0_Vector4.r;
                float _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_G_5_Float = _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_RGBA_0_Vector4.g;
                float _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_B_6_Float = _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_RGBA_0_Vector4.b;
                float _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_A_7_Float = _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_RGBA_0_Vector4.a;
                float4 _Add_e7700aa9deaa47379fa7257806f4feb4_Out_2_Vector4;
                Unity_Add_float4(_SampleTexture2D_07272b0b7c134c28a01e9ccc2196f318_RGBA_0_Vector4, _SampleTexture2D_95e6540625044b708f62b2ebb415f4c7_RGBA_0_Vector4, _Add_e7700aa9deaa47379fa7257806f4feb4_Out_2_Vector4);
                float4 _Add_a516b3c81824490aa17a86228d13750d_Out_2_Vector4;
                Unity_Add_float4(_Add_ca78ed20ea1d4860a5bf21342e43374f_Out_2_Vector4, _Add_e7700aa9deaa47379fa7257806f4feb4_Out_2_Vector4, _Add_a516b3c81824490aa17a86228d13750d_Out_2_Vector4);
                UnityTexture2D _Property_99b3f6c2258f45a0a6acc31b363dd2fd_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float _Property_6de35be7c9d4496baffdca0d153d29f5_Out_0_Float = _BlurAmount;
                float _Multiply_5bd99b465c374f66a7cbe38cf9056c43_Out_2_Float;
                Unity_Multiply_float_float(_Property_6de35be7c9d4496baffdca0d153d29f5_Out_0_Float, 0.001, _Multiply_5bd99b465c374f66a7cbe38cf9056c43_Out_2_Float);
                float _Negate_8c9695d4a5174f0b9e92c6dc338f1a66_Out_1_Float;
                Unity_Negate_float(_Multiply_5bd99b465c374f66a7cbe38cf9056c43_Out_2_Float, _Negate_8c9695d4a5174f0b9e92c6dc338f1a66_Out_1_Float);
                float4 _Combine_b7667f23680540e2a315ef57ab995455_RGBA_4_Vector4;
                float3 _Combine_b7667f23680540e2a315ef57ab995455_RGB_5_Vector3;
                float2 _Combine_b7667f23680540e2a315ef57ab995455_RG_6_Vector2;
                Unity_Combine_float(_Negate_8c9695d4a5174f0b9e92c6dc338f1a66_Out_1_Float, _Multiply_5bd99b465c374f66a7cbe38cf9056c43_Out_2_Float, 0, 0, _Combine_b7667f23680540e2a315ef57ab995455_RGBA_4_Vector4, _Combine_b7667f23680540e2a315ef57ab995455_RGB_5_Vector3, _Combine_b7667f23680540e2a315ef57ab995455_RG_6_Vector2);
                float2 _Negate_424e3f18a9cc4be49bc8389ee0ff8e0c_Out_1_Vector2;
                Unity_Negate_float2(_Combine_b7667f23680540e2a315ef57ab995455_RG_6_Vector2, _Negate_424e3f18a9cc4be49bc8389ee0ff8e0c_Out_1_Vector2);
                float2 _TilingAndOffset_d8b5ae6ade0046aba613de9e71b58606_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Negate_424e3f18a9cc4be49bc8389ee0ff8e0c_Out_1_Vector2, _TilingAndOffset_d8b5ae6ade0046aba613de9e71b58606_Out_3_Vector2);
                float4 _SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_99b3f6c2258f45a0a6acc31b363dd2fd_Out_0_Texture2D.tex, _Property_99b3f6c2258f45a0a6acc31b363dd2fd_Out_0_Texture2D.samplerstate, _Property_99b3f6c2258f45a0a6acc31b363dd2fd_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_d8b5ae6ade0046aba613de9e71b58606_Out_3_Vector2));
                float _SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_R_4_Float = _SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_RGBA_0_Vector4.r;
                float _SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_G_5_Float = _SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_RGBA_0_Vector4.g;
                float _SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_B_6_Float = _SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_RGBA_0_Vector4.b;
                float _SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_A_7_Float = _SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_RGBA_0_Vector4.a;
                UnityTexture2D _Property_088f2fc8b0884281a202e4d167c4667a_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float2 _TilingAndOffset_bdd15c4a10234ecb992064978f56a8cb_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Combine_b7667f23680540e2a315ef57ab995455_RG_6_Vector2, _TilingAndOffset_bdd15c4a10234ecb992064978f56a8cb_Out_3_Vector2);
                float4 _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_088f2fc8b0884281a202e4d167c4667a_Out_0_Texture2D.tex, _Property_088f2fc8b0884281a202e4d167c4667a_Out_0_Texture2D.samplerstate, _Property_088f2fc8b0884281a202e4d167c4667a_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_bdd15c4a10234ecb992064978f56a8cb_Out_3_Vector2));
                float _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_R_4_Float = _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_RGBA_0_Vector4.r;
                float _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_G_5_Float = _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_RGBA_0_Vector4.g;
                float _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_B_6_Float = _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_RGBA_0_Vector4.b;
                float _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_A_7_Float = _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_RGBA_0_Vector4.a;
                float4 _Add_a3bf4724085348c5924f3e36ef9be040_Out_2_Vector4;
                Unity_Add_float4(_SampleTexture2D_ffdc9499018d4b158cfeffc4ff4f0672_RGBA_0_Vector4, _SampleTexture2D_443075eab48b4a5488a75432ee84dcdf_RGBA_0_Vector4, _Add_a3bf4724085348c5924f3e36ef9be040_Out_2_Vector4);
                UnityTexture2D _Property_9a82e8d280224152b6894cfabfaeaa8c_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float4 _Combine_7e453a5cb9f94974a9a2f63c5f69c34b_RGBA_4_Vector4;
                float3 _Combine_7e453a5cb9f94974a9a2f63c5f69c34b_RGB_5_Vector3;
                float2 _Combine_7e453a5cb9f94974a9a2f63c5f69c34b_RG_6_Vector2;
                Unity_Combine_float(_Multiply_5bd99b465c374f66a7cbe38cf9056c43_Out_2_Float, _Multiply_5bd99b465c374f66a7cbe38cf9056c43_Out_2_Float, 0, 0, _Combine_7e453a5cb9f94974a9a2f63c5f69c34b_RGBA_4_Vector4, _Combine_7e453a5cb9f94974a9a2f63c5f69c34b_RGB_5_Vector3, _Combine_7e453a5cb9f94974a9a2f63c5f69c34b_RG_6_Vector2);
                float2 _Negate_e4b829ec99414a26a5d13f39cd01d900_Out_1_Vector2;
                Unity_Negate_float2(_Combine_7e453a5cb9f94974a9a2f63c5f69c34b_RG_6_Vector2, _Negate_e4b829ec99414a26a5d13f39cd01d900_Out_1_Vector2);
                float2 _TilingAndOffset_566b48c77b0c45ff8e10e3c86486fcaa_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Negate_e4b829ec99414a26a5d13f39cd01d900_Out_1_Vector2, _TilingAndOffset_566b48c77b0c45ff8e10e3c86486fcaa_Out_3_Vector2);
                float4 _SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_9a82e8d280224152b6894cfabfaeaa8c_Out_0_Texture2D.tex, _Property_9a82e8d280224152b6894cfabfaeaa8c_Out_0_Texture2D.samplerstate, _Property_9a82e8d280224152b6894cfabfaeaa8c_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_566b48c77b0c45ff8e10e3c86486fcaa_Out_3_Vector2));
                float _SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_R_4_Float = _SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_RGBA_0_Vector4.r;
                float _SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_G_5_Float = _SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_RGBA_0_Vector4.g;
                float _SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_B_6_Float = _SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_RGBA_0_Vector4.b;
                float _SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_A_7_Float = _SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_RGBA_0_Vector4.a;
                UnityTexture2D _Property_160693d810164b35be649404397a4725_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float2 _TilingAndOffset_eb00f1184b6448d1ad6f77a6a394ad6e_Out_3_Vector2;
                Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Combine_7e453a5cb9f94974a9a2f63c5f69c34b_RG_6_Vector2, _TilingAndOffset_eb00f1184b6448d1ad6f77a6a394ad6e_Out_3_Vector2);
                float4 _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_160693d810164b35be649404397a4725_Out_0_Texture2D.tex, _Property_160693d810164b35be649404397a4725_Out_0_Texture2D.samplerstate, _Property_160693d810164b35be649404397a4725_Out_0_Texture2D.GetTransformedUV(_TilingAndOffset_eb00f1184b6448d1ad6f77a6a394ad6e_Out_3_Vector2));
                float _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_R_4_Float = _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_RGBA_0_Vector4.r;
                float _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_G_5_Float = _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_RGBA_0_Vector4.g;
                float _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_B_6_Float = _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_RGBA_0_Vector4.b;
                float _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_A_7_Float = _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_RGBA_0_Vector4.a;
                float4 _Add_1347952107a84a4a8b48db2141b58eb8_Out_2_Vector4;
                Unity_Add_float4(_SampleTexture2D_2e8ca5c8264e4cd6acdc9c641fd99e15_RGBA_0_Vector4, _SampleTexture2D_5101d476fb634b99ae37b3f1eaa3eedf_RGBA_0_Vector4, _Add_1347952107a84a4a8b48db2141b58eb8_Out_2_Vector4);
                float4 _Add_85ac976465554fc59386db022e0508a3_Out_2_Vector4;
                Unity_Add_float4(_Add_a3bf4724085348c5924f3e36ef9be040_Out_2_Vector4, _Add_1347952107a84a4a8b48db2141b58eb8_Out_2_Vector4, _Add_85ac976465554fc59386db022e0508a3_Out_2_Vector4);
                float4 _Add_fa656b5728454b2e817981052c1dd29b_Out_2_Vector4;
                Unity_Add_float4(_Add_a516b3c81824490aa17a86228d13750d_Out_2_Vector4, _Add_85ac976465554fc59386db022e0508a3_Out_2_Vector4, _Add_fa656b5728454b2e817981052c1dd29b_Out_2_Vector4);
                float _Float_aaf94dfbae2540389f8558bcb7bc50d5_Out_0_Float = 8;
                float4 _Divide_791c46a1d544489883f94e6a6fd5e383_Out_2_Vector4;
                Unity_Divide_float4(_Add_fa656b5728454b2e817981052c1dd29b_Out_2_Vector4, (_Float_aaf94dfbae2540389f8558bcb7bc50d5_Out_0_Float.xxxx), _Divide_791c46a1d544489883f94e6a6fd5e383_Out_2_Vector4);
                float _Property_9f865c38ba98411ebb6f51ac79634c3b_Out_0_Float = _Intensity;
                UnityTexture2D _Property_3c79f4d078884c5e8f568e3bc69a8974_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float4 _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_3c79f4d078884c5e8f568e3bc69a8974_Out_0_Texture2D.tex, _Property_3c79f4d078884c5e8f568e3bc69a8974_Out_0_Texture2D.samplerstate, _Property_3c79f4d078884c5e8f568e3bc69a8974_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                float _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_R_4_Float = _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_RGBA_0_Vector4.r;
                float _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_G_5_Float = _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_RGBA_0_Vector4.g;
                float _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_B_6_Float = _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_RGBA_0_Vector4.b;
                float _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_A_7_Float = _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_RGBA_0_Vector4.a;
                float _Step_b7ff2c989afe467b9c74a324ae1959dd_Out_2_Float;
                Unity_Step_float(_Property_9f865c38ba98411ebb6f51ac79634c3b_Out_0_Float, _SampleTexture2D_1147efecd2ec4f83bc8d71ef314ebe8a_A_7_Float, _Step_b7ff2c989afe467b9c74a324ae1959dd_Out_2_Float);
                surface.BaseColor = (_Divide_791c46a1d544489883f94e6a6fd5e383_Out_2_Vector4.xyz);
                surface.Alpha = _Step_b7ff2c989afe467b9c74a324ae1959dd_Out_2_Float;
                surface.AlphaClipThreshold = 0.09;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

            #ifdef HAVE_VFX_MODIFICATION
            #if VFX_USE_GRAPH_VALUES
                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
            #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

            #endif








                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                output.uv0 = input.texCoord0;
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                    return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif

            ENDHLSL
            }

    }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
                                                CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
                                                FallBack "Hidden/Shader Graph/FallbackError"
}