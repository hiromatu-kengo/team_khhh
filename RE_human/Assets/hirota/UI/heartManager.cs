using UnityEngine;
using UnityEngine.UI;
public class heartManager : MonoBehaviour
{
    [SerializeField] private Image[] heartImages;
    [SerializeField] private Sprite fullHeartSprite;
    [SerializeField] private Sprite emptyHeartSprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
        public void UpdateHearts(int currentHp)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            // 現在のインデックスがHP未満なら「フル」、以上なら「空」にする
            if (i < currentHp)
            {
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }
}
