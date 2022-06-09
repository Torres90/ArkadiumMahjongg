using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType tileType;
    public Vector3Int boardPosition;

    [SerializeField] List<Material> tileTypeMaterial; //already in the same order as the enum
    [SerializeField] BoxCollider boxCollider;
    [SerializeField] AnimationCurve animationCurve;
    IEnumerator spinAndShrink;
    Renderer[] quadRenderers;

    private void Start()
    {
        spinAndShrink = SpinAndShrink();
    }

    public void Init(TileType type)
    {
        if (spinAndShrink != null)
            StopCoroutine(spinAndShrink);

        if (quadRenderers == null || quadRenderers.Length == 0)
        {
            quadRenderers = new Renderer[6];
            for (int i = 0; i < 6; i++)
            {
                quadRenderers[i] = transform.GetChild(i).GetComponent<Renderer>();
            }
        }

        tileType = type;
        for (int i = 0; i < quadRenderers.Length; i++)
        {
            quadRenderers[i].material = tileTypeMaterial[(int)type];
        }
        boxCollider.enabled = true;
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;

        gameObject.SetActive(true);
    }

    public void SelfDisable()
    {
        boxCollider.enabled = false;

        StopCoroutine(spinAndShrink);
        spinAndShrink = SpinAndShrink();
        StartCoroutine(spinAndShrink);
        //Debug.Log($"Self Disabling from {this.name}");
    }

    IEnumerator SpinAndShrink()
    {
        float t = 0.0f;
        float startRotation = transform.eulerAngles.y;
        float endRotation = startRotation + 180.0f;

        Vector3 initialScale = transform.localScale;
        float duration = 0.25f;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.zero;
            float progress = animationCurve.Evaluate( t / duration);

            float yRotation = Mathf.LerpUnclamped(startRotation, endRotation, progress) % 360.0f;
            transform.localScale = new Vector3(
                Mathf.LerpUnclamped(initialScale.x, 0, progress),
                Mathf.LerpUnclamped(initialScale.y, 0, progress),
                Mathf.LerpUnclamped(initialScale.z, 0, progress));

            transform.eulerAngles = new Vector3(
                transform.eulerAngles.x,
                yRotation,
                transform.eulerAngles.z);

            yield return null;
        }
        //Disable at the end of the anim
        gameObject.SetActive(false);
    }
}
