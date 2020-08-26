using System;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    [AddComponentMenu ("Image Effects/Bloom and Glow/Bloom (Optimized)")]
    public class BloomOptimized : PostEffectsBase
    {

        public enum Resolution
		{
            Low = 0,
            High = 1,
        }

        public enum BlurType
		{
            Standard = 0,
            Sgx = 1,
        }

        [Range(0.0f, 1.5f)]
        public float threshold = 0.25f;
        [Range(0.0f, 2.5f)]
        public float intensity = 0.75f;

        [Range(0.25f, 5.5f)]
        public float blurSize = 1.0f;

        Resolution resolution = Resolution.Low;
        [Range(1, 4)]
        public int blurIterations = 1;

        public BlurType blurType= BlurType.Standard;

        public Shader fastBloomShader = null;
        private Material fastBloomMaterial = null;

        public RenderTexture BloomedTexture;
        [SerializeField]
        RenderTexture rawParticleTexture = null;
        RenderTexture src;
        RenderTexture dest;
        RenderTexture _filteredMap = null;

        [NonSerialized]
        Material velFilter;
        public Shader velFilterShader;
        public override bool CheckResources ()
		{
            CheckSupport (false);

            fastBloomMaterial = CheckShaderAndCreateMaterial (fastBloomShader, fastBloomMaterial);

            if (!isSupported)
                ReportAutoDisable ();
            return isSupported;
        }


        private void Start()
        {
            CreateOrReCreateMap(ref _filteredMap, 4096, 4096, RenderTextureFormat.ARGBFloat, FilterMode.Bilinear, TextureWrapMode.Clamp, false);

            if (velFilter == null)
            {
                velFilter = new Material(velFilterShader);
                velFilter.hideFlags = HideFlags.HideAndDontSave;
            }
        }
        private void Update()
        {
            Graphics.Blit(rawParticleTexture, _filteredMap, velFilter);
            //source = _filteredMap;
            src = this.gameObject.GetComponent<Camera>().targetTexture;//rawParticleTexture;// _filteredMap;//
            dest = BloomedTexture;

            if (CheckResources() == false)
            {
                Graphics.Blit(src, dest);
                return;
            }

            int divider = resolution == Resolution.Low ? 4 : 2;
            float widthMod = resolution == Resolution.Low ? 0.5f : 1.0f;

            fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod, 0.0f, threshold, intensity));
            src.filterMode = FilterMode.Bilinear;

            var rtW = src.width / divider;
            var rtH = src.height / divider;

            // downsample
            RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, src.format);
            rt.filterMode = FilterMode.Bilinear;
            Graphics.Blit(_filteredMap, rt, fastBloomMaterial, 1);
            //Graphics.Blit(src, rt, fastBloomMaterial, 1);

            var passOffs = blurType == BlurType.Standard ? 0 : 2;

            for (int i = 0; i < blurIterations; i++)
            {
                fastBloomMaterial.SetVector("_Parameter", new Vector4(blurSize * widthMod + (i * 1.0f), 0.0f, threshold, intensity));

                // vertical blur
                RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, src.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, fastBloomMaterial, 2 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;

                // horizontal blur
                rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, src.format);
                rt2.filterMode = FilterMode.Bilinear;
                Graphics.Blit(rt, rt2, fastBloomMaterial, 3 + passOffs);
                RenderTexture.ReleaseTemporary(rt);
                rt = rt2;
            }

            fastBloomMaterial.SetTexture("_Bloom", rt);

            Graphics.Blit(src, dest, fastBloomMaterial, 0);

            RenderTexture.ReleaseTemporary(rt);
        }

  //      void OnRenderImage (RenderTexture source, RenderTexture destination)
		//{
  //          if (CheckResources() == false)
		//	{
  //              Graphics.Blit (source, destination);
  //              return;
  //          }

  //          int divider = resolution == Resolution.Low ? 4 : 2;
  //          float widthMod = resolution == Resolution.Low ? 0.5f : 1.0f;

  //          fastBloomMaterial.SetVector ("_Parameter", new Vector4 (blurSize * widthMod, 0.0f, threshold, intensity));
  //          source.filterMode = FilterMode.Bilinear;

  //          var rtW= source.width/divider;
  //          var rtH= source.height/divider;

  //          // downsample
  //          RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
  //          rt.filterMode = FilterMode.Bilinear;
  //          Graphics.Blit (source, rt, fastBloomMaterial, 1);

  //          var passOffs= blurType == BlurType.Standard ? 0 : 2;

  //          for(int i = 0; i < blurIterations; i++)
		//	{
  //              fastBloomMaterial.SetVector ("_Parameter", new Vector4 (blurSize * widthMod + (i*1.0f), 0.0f, threshold, intensity));

  //              // vertical blur
  //              RenderTexture rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
  //              rt2.filterMode = FilterMode.Bilinear;
  //              Graphics.Blit (rt, rt2, fastBloomMaterial, 2 + passOffs);
  //              RenderTexture.ReleaseTemporary (rt);
  //              rt = rt2;

  //              // horizontal blur
  //              rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
  //              rt2.filterMode = FilterMode.Bilinear;
  //              Graphics.Blit (rt, rt2, fastBloomMaterial, 3 + passOffs);
  //              RenderTexture.ReleaseTemporary (rt);
  //              rt = rt2;
  //          }

  //          fastBloomMaterial.SetTexture ("_Bloom", rt);

  //          Graphics.Blit (source, destination, fastBloomMaterial, 0);

  //          RenderTexture.ReleaseTemporary (rt);
  //      }

        void OnEnable()
        {
            // リソースを削除
            DeleteResources();
        }

        void OnDisable()
        {
                if (fastBloomMaterial)
                    DestroyImmediate(fastBloomMaterial);
                // リソースを削除
                DeleteResources();
        }

        void OnDestroy()
        {
            // リソースを削除
            DeleteResources();
        }

        void DeleteResources()
        {
            // --- RenderTexture -------
            DeleteRenderTexture(ref _filteredMap);

        }
        /// <summary>
        /// RenderBufferを作成
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="format"></param>
        /// <param name="filter"></param>
        /// <param name="wrap"></param>
        /// <param name="useInComputeshader"></param>
        void CreateRenderTexture(ref RenderTexture rt, int w, int h, RenderTextureFormat format, FilterMode filter, TextureWrapMode wrap, bool useInComputeshader)
        {
            rt = new RenderTexture(w, h, 0, format);
            rt.hideFlags = HideFlags.DontSave;
            rt.filterMode = filter;
            rt.wrapMode = wrap;
            if (useInComputeshader)
            {
                rt.enableRandomWrite = true;
                rt.Create();
            }
        }

        /// <summary>
        /// RenderTextureを削除
        /// </summary>
        /// <param name="rt"></param>
        void DeleteRenderTexture(ref RenderTexture rt)
        {
            if (rt != null)
            {
                if (RenderTexture.active == rt)
                    Graphics.SetRenderTarget(null);
                rt.Release();
                if (Application.isEditor)
                    RenderTexture.DestroyImmediate(rt);
                else
                    RenderTexture.Destroy(rt);
                rt = null;
            }
        }

        /// <summary>
        /// RenderTextureをクリア
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="color"></param>
        void ClearRenderTexture(ref RenderTexture rt, Color? color = null)
        {
            RenderTexture store = RenderTexture.active;
            Graphics.SetRenderTarget(rt);
            GL.Clear(false, true, color ?? Color.clear);
            Graphics.SetRenderTarget(store);
        }
        public void CreateOrReCreateMap(ref RenderTexture rt, int w, int h, RenderTextureFormat format, FilterMode filter, TextureWrapMode wrap, bool useInComputeshader)
        {
            if (rt == null)
            {
                CreateRenderTexture(ref rt, w, h, format, filter, wrap, useInComputeshader);
                ClearRenderTexture(ref rt);
            }
            else
            {
                if (w != rt.width || h != rt.height)
                {
                    DeleteRenderTexture(ref rt);
                    CreateRenderTexture(ref rt, w, h, format, filter, wrap, useInComputeshader);
                    ClearRenderTexture(ref rt);
                }
            }
        }

    }
}
