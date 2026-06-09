using UnityEngine;

public class Fruit : MonoBehaviour
{
    public int level;

    [HideInInspector]
    public bool isMerged = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (!isMerged && collision.gameObject.CompareTag("Fruit"))
        {
            Fruit other = collision.gameObject.GetComponent<Fruit>();
            if (other != null && level == other.level)
            {
                // ID가 큰 쪽이 대표로 매니저에게 주문을 넣음
                if (gameObject.GetInstanceID() > collision.gameObject.GetInstanceID())
                {
                    FruitManager.instance.MergeFruits(this, other);
                }
            }
        }
    }
}