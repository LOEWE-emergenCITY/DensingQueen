using System.Collections.Generic;
using UnityEngine;

namespace Flashlight.codeTestScene
{
    [RequireComponent(typeof(Projector))]
    public class TSProjectorWrapper : MonoBehaviour
    {
        public Camera inputCamera;
        public RenderTexture inputFeed;
        private Texture2D outputImage;
        private RenderTexture old;

        private readonly Color blank = new Color(0, 0, 0, 0);
        private Color alphaShift;
        private Rect rect;

        // Start is called before the first frame update
        private void Start()
        {
            if (inputCamera != null)
            {
                GetComponent<Projector>().aspectRatio = inputCamera.aspect;
                var width = inputFeed.width + 2;
                var height = inputFeed.height + 2;

                var widthList = new List<Color>();
                var heightList = new List<Color>();

                for (var i = 0; i < width; i++)
                {
                    widthList.Add(blank);
                }

                for (var i = 0; i < height; i++)
                {
                    heightList.Add(blank);
                }

                outputImage = new Texture2D(width, height, TextureFormat.ARGB32, false)
                {
                    wrapMode = TextureWrapMode.Clamp
                };

                var textureFrameWidth = widthList.ToArray();
                var textureFrameHeight = heightList.ToArray();

                outputImage.SetPixels(0, 0, width, 1, textureFrameWidth);
                outputImage.SetPixels(0, 0, 1, height, textureFrameHeight);
                outputImage.SetPixels(0, height - 1, width, 1, textureFrameWidth);
                outputImage.SetPixels(width - 1, 0, 1, height, textureFrameHeight);

                rect = new Rect(0, 0, inputFeed.width, inputFeed.height);
            }
        }
    }
}