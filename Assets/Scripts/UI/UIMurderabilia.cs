using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIMurderabilia :  MonoBehaviour
    {
        [Header("References")]
        public GameObject textBoxPrefab;
        public Transform contentParent;
        
        public void UpdateCollectionUI(List<(string itemName, bool collected)> items)
        {
            foreach (Transform child in contentParent)
            {
                if (child.gameObject.activeSelf)
                {
                    Destroy(child.gameObject);
                }
            }

            foreach (var item in items)
            {
                var newBox = Instantiate(textBoxPrefab, contentParent);
                var text = newBox.GetComponentInChildren<TMP_Text>();
                text.text = item.itemName;
                if (item.collected)
                {
                    text.fontStyle = FontStyles.Strikethrough;
                }

                newBox.gameObject.SetActive(true);
            }
        }
    }
}