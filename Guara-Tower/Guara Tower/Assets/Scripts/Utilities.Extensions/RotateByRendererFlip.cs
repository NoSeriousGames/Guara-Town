using UnityEngine;

public class RotateByRendererFlip : MonoBehaviour {

    public SpriteRenderer m_SpriteRendererToCopy;

    private void Update() {

        transform.localScale = new Vector3(m_SpriteRendererToCopy.flipX ? -1:1 , m_SpriteRendererToCopy.flipY ? -1 : 1, 1);

    }

}
