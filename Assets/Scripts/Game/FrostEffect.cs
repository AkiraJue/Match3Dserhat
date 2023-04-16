using UnityEngine;
public class FrostEffect : MonoBehaviour
{
    public static float FrostAmount; //0-1 (0=minimum Frost, 1=maximum frost)
    private float EdgeSharpness; //>=1
    private float minFrost; //0-1
    private float maxFrost; //0-1
    private float seeThroughness; //blends between 2 ways of applying the frost effect: 0=normal blend mode, 1="overlay" blend mode
    private float distortion; //how much the original image is distorted through the frost (value depends on normal map)
    [SerializeField]private Texture2D Frost; //RGBA
    [SerializeField]private Texture2D FrostNormals; //normalmap
    [SerializeField]private Shader Shader; //ImageBlendEffect.shader
	
	private Material material;

	private void Awake()
	{
        material = new Material(Shader);
        material.SetTexture("_BlendTex", Frost);
        material.SetTexture("_BumpMap", FrostNormals);
	}
    private void Start()
    {
        EdgeSharpness = 10f;
        minFrost = 0.2f;
        maxFrost = 0.8f;
        seeThroughness = 0.6f;
        distortion = 0.5f;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!Application.isPlaying)
        {
            material.SetTexture("_BlendTex", Frost);
            material.SetTexture("_BumpMap", FrostNormals);
            EdgeSharpness = Mathf.Max(1, EdgeSharpness);
        }
        material.SetFloat("_BlendAmount", Mathf.Clamp01(Mathf.Clamp01(FrostAmount) * (maxFrost - minFrost) + minFrost));
        material.SetFloat("_EdgeSharpness", EdgeSharpness);
        material.SetFloat("_SeeThroughness", seeThroughness);
        material.SetFloat("_Distortion", distortion);
		Graphics.Blit(source, destination, material);
	}
}