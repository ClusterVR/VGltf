using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using VGltf.Unity;

namespace VGltfExamples.VRMExample
{
    public sealed class VRM0ImporterBridge : VGltf.Ext.Vrm0.Unity.Bridge.IImporterBridge
    {
        readonly VGltf.Ext.Vrm0.Unity.DefaultImporterBridge _defaultBridge = new VGltf.Ext.Vrm0.Unity.DefaultImporterBridge();

        public void ImportMeta(IImporterContext context, VGltf.Ext.Vrm0.Types.Meta vrmMeta, GameObject go)
        {
            _defaultBridge.ImportMeta(context, vrmMeta, go);
        }

        public void ImportFirstPerson(IImporterContext context, VGltf.Ext.Vrm0.Types.FirstPerson vrmFirstPerson, GameObject go)
        {
            _defaultBridge.ImportFirstPerson(context, vrmFirstPerson, go);
        }

        public void ImportBlendShapeMaster(IImporterContext context, VGltf.Ext.Vrm0.Types.BlendShape vrmBlendShape, GameObject go)
        {
            _defaultBridge.ImportBlendShapeMaster(context, vrmBlendShape, go);
        }

        public void ImportSecondaryAnimation(IImporterContext context, VGltf.Ext.Vrm0.Types.SecondaryAnimation vrmSecondaryAnimation, GameObject go)
        {
            _defaultBridge.ImportSecondaryAnimation(context, vrmSecondaryAnimation, go);
        }

        public async Task ReplaceMaterialByMtoon(IImporterContext context, VGltf.Ext.Vrm0.Types.Material matProp, Material mat, CancellationToken ct)
        {
            if (matProp.Shader == VGltf.Ext.Vrm0.Types.Material.VRM_USE_GLTFSHADER)
            {
                // DO nothing!
                return;
            }

            var shader = Shader.Find(MToon.Utils.ShaderName);
            if (shader == null)
            {
                throw new Exception("VRM0 shader is not found");
            }

            mat.shader = shader;
            mat.renderQueue = matProp.RenderQueue;
            foreach (var kv in matProp.FloatProperties)
            {
                if (!mat.HasProperty(kv.Key))
                {
                    continue;
                }
                if (!MToonProps.TryGetPropKind(kv.Key, out var kind) || kind != MToonProps.PropKind.Float)
                {
                    continue;
                }

                mat.SetFloat(kv.Key, kv.Value);
            }
            foreach (var kv in matProp.KeywordMap)
            {
                if (kv.Value)
                {
                    mat.EnableKeyword(kv.Key);
                }
                else
                {
                    mat.DisableKeyword(kv.Key);
                }
            }
            foreach (var kv in matProp.TagMap)
            {
                mat.SetOverrideTag(kv.Key, kv.Value);
            }
            foreach (var kv in matProp.TextureProperties)
            {
                if (!mat.HasProperty(kv.Key))
                {
                    continue;
                }
                if (!MToonProps.TryGetPropKind(kv.Key, out var kind) || kind != MToonProps.PropKind.Tex)
                {
                    continue;
                }

                if (!context.Resources.Textures.TryGetValue(kv.Value, out var texRes))
                {
                    texRes = await context.Importers.Textures.Import(kv.Value, false, ct);
                    await context.TimeSlicer.Slice(ct);
                }
                mat.SetTexture(kv.Key, texRes.Value);
            }
            foreach (var kv in matProp.VectorProperties)
            {
                if (!mat.HasProperty(kv.Key))
                {
                    continue;
                }
                if (!MToonProps.TryGetPropKind(kv.Key, out var kind) || kind != MToonProps.PropKind.Color)
                {
                    continue;
                }

                mat.SetVector(kv.Key, new Vector4(kv.Value[0], kv.Value[1], kv.Value[2], kv.Value[3]));
            }
        }
    }
}
