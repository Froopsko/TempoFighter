using Spine.Unity;
using Spine; // Ajoutez ceci pour accéder aux classes de Spine
using UnityEngine;

public class OpacityController : MonoBehaviour
{
    public SkeletonAnimation skeletonAnimation;
    public float opacity = 0.5f; // Valeur d'opacité (0 = transparent, 1 = opaque)

    void Start()
    {
        SetOpacity(opacity);
    }

    public void SetOpacity(float alpha)
    {
        // Utilisez UnityEngine.Color à la place de Spine.Color
        UnityEngine.Color color = new UnityEngine.Color(1f, 1f, 1f, alpha);
        skeletonAnimation.skeleton.SetColor(color);
    }
}
