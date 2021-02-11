using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{

    public static BackgroundController instance;
    public  Layer background = new Layer();

    private void Awake()
    {
        instance = this;
    }

    [System.Serializable]
    public class Layer
    {
        public GameObject root;
        public RawImage activeRawImage;
        public GameObject newRawImageObject;
        public List<RawImage> allRawImages = new List<RawImage>();

        public void SetTexture(Texture texture)
        {
            if (texture != null)
            {
                if (activeRawImage == null)
                    CreateNewactiveRawImage();
                activeRawImage.texture = texture;
                activeRawImage.color = GlobalF.SetAlpha(activeRawImage.color, 1f);
            }
            else
            {
                if (activeRawImage != null)
                {
                    allRawImages.Remove(activeRawImage);
                    GameObject.DestroyImmediate(activeRawImage.gameObject);
                    activeRawImage = null;
                }
            }
        }
		public void TransitionToTexture(Texture texture, float speed = 1f, bool smooth = false)
		{
			if (activeRawImage != null && activeRawImage.texture == texture)
				return;

			StopTransitioning();
			transitioning = BackgroundController.instance.StartCoroutine(Transitioning(texture, speed, smooth));
		}

		void StopTransitioning()
		{
			if (isTransitioning)
				BackgroundController.instance.StopCoroutine(transitioning);

			transitioning = null;
		}

		public bool isTransitioning { get { return transitioning != null; } }
		Coroutine transitioning = null;
		IEnumerator Transitioning(Texture texture, float speed, bool smooth)
		{
			if (texture != null)
			{
				for (int i = 0; i < allRawImages.Count; i++)
				{
					RawImage image = allRawImages[i];
					if (image.texture == texture)
					{
						activeRawImage = image;
						break;
					}
				}

				if (activeRawImage == null || activeRawImage.texture != texture)
				{
					CreateNewactiveRawImage();
					activeRawImage.texture = texture;
					activeRawImage.color = GlobalF.SetAlpha(activeRawImage.color, 0f);
				}
			}
			else
				activeRawImage = null;

			while (GlobalF.TransitionRawImages(ref activeRawImage, ref allRawImages, speed, smooth))
				yield return new WaitForEndOfFrame();

			StopTransitioning();
		}

		public void CreateNewactiveRawImage()
        {
            GameObject ob = Instantiate(newRawImageObject, root.transform) as GameObject;
            ob.SetActive(true);
            RawImage im = ob.GetComponent<RawImage>();
            activeRawImage = im;
            allRawImages.Add(im);
        }
    }
}
