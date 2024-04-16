using UnityEngine;

namespace Game.FlappyEddie
{
	public class MoveBG : MonoBehaviour
	{
		[SerializeField] bool isBgMove;
		[SerializeField] MeshRenderer mesh;
		[SerializeField] float speed = 0.1f;
        public Texture bgTexture;

		bool isPause;
		float timePlay;

        private void Start()
        {
            if (bgTexture != null)
            {
                mesh.materials[0].mainTexture = bgTexture;
            }
        }

        // Update is called once per frame
        void Update()
		{
			if (isPause || !isBgMove) return;

			timePlay += Time.deltaTime;
			var offset = new Vector2(timePlay * speed, 0);
			mesh.material.mainTextureOffset = offset;
		}

		public void SetPause(bool isPause)
		{
			this.isPause = isPause;
		}
	}
}
