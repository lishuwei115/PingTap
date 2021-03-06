using Fralle.Core.Pooling;
using UnityEngine;

namespace Fralle.PingTap
{
  public class ExplosionShockwave : MonoBehaviour
  {
    static readonly int Alpha = Shader.PropertyToID("_Alpha");
    static readonly int Size = Shader.PropertyToID("_Size");

    [SerializeField] float dissipateSpeed = 0.25f;

    float dissipationTime;
    float radius;
    float DissipationPercentage => Mathf.Max(dissipateSpeed - dissipationTime, 0) / dissipateSpeed;

    new Renderer renderer;
    MaterialPropertyBlock propBlock;

    public void Setup(float explosionRadius)
    {
      radius = explosionRadius;
    }

    void Awake()
    {
      propBlock = new MaterialPropertyBlock();
      renderer = GetComponentInChildren<Renderer>();
      renderer.GetPropertyBlock(propBlock);
    }

    void OnEnable()
    {
      dissipationTime = dissipateSpeed;
    }

    void Update()
    {
      if (dissipationTime > 0)
      {
        dissipationTime -= Time.deltaTime;
        propBlock.SetFloat(Size, radius * DissipationPercentage);
        propBlock.SetFloat(Alpha, 1f - DissipationPercentage);
        renderer.SetPropertyBlock(propBlock);
      }
      else
        Despawn();
    }

    void Despawn()
    {
      ObjectPool.Despawn(gameObject);
    }
  }
}
