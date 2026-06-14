using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;



public class SelectedElement : MonoBehaviour
{
    public List<MagicData> selectedElementList; //選択した魔法属性のリスト
    public int maxSelectedNum = 5; //最大のリスト内の魔法属性の個数(現在は使っていない)
    public List<MagicData> PreSelectedElementList; //現在のリストと比較するためのリスト
    public event Action DifferentList; //リストが更新されたことを知らせるイベント
    [SerializeField] public List<MagicData> AllElementDataList; //全ての属性のデータが入ったもの
    [SerializeField] public List<GameObject> AllElementSpriteList; //設定画面にある全ての属性のUIが入ったもの
    [SerializeField] public List<GameObject> AllElementButtonList; //設定画面にある属性のボタンオブジェクトが入ったもの
    [SerializeField] private GameObject RingObjectParent; //属性のSpriteを入れるための親オブジェクト。
    [SerializeField] private RectTransform FirstMagicRT; //最初の属性の位置。これを使って計算することで、1つのTrasformで複数の位置を指定できる。
    [SerializeField] private RectTransform RingObjectRT; //MagicRingの位置。
    private Vector3 ObjectScale = new Vector3(228.99f, 222.53f, 0); //スロットのSpriteの大きさ
    [SerializeField] private Sprite SlotSprite;//
    [SerializeField] private TextMeshProUGUI SlotButtonText;

    private void Update()
    {
        if (!selectedElementList.SequenceEqual(PreSelectedElementList)) //リストを更新したときに使う
        {
            PreSelectedElementList = new List<MagicData>(selectedElementList);
            DifferentList?.Invoke();
        }
    }


    public void Init()
    {
        selectedElementList = new List<MagicData>();
        PreSelectedElementList = new List<MagicData>();
        ChangeSlot();
    }

    public void ChangeSlot()
    {
        //PushData中はSlotを増やせないようにする
        if (DOTween.IsTweening("ElementTween")) return;

        //更新用の操作
        //子オブジェクト(前の選択属性のSprite)を全部削除する。



        if (selectedElementList.Count < maxSelectedNum)
        {
            foreach (Transform child in RingObjectParent.transform)
            {
                Destroy(child.gameObject);
            }
            Vector2 relativeVector = FirstMagicRT.anchoredPosition - RingObjectRT.anchoredPosition;

            selectedElementList.Add(null);

            //生成の操作
            for (int i = 0; i < selectedElementList.Count; i++)
            {
                //Slotのオブジェクトを作成
                GameObject SlotObj = new GameObject("SlotUI");
                SlotObj.transform.SetParent(RingObjectParent.transform, false);
                Image image = SlotObj.AddComponent<Image>();
                RectTransform rectTransform = SlotObj.GetComponent<RectTransform>();

                //SlotObj(サイズと位置)
                //位置については、relativeVectorと必要な回転量から計算して、求めている
                rectTransform.sizeDelta = ObjectScale;
                float angle = (360f / selectedElementList.Count) * i;
                Vector2 targetPosition = Quaternion.Euler(0, 0, angle) * relativeVector;
                rectTransform.anchoredPosition = targetPosition + new Vector2(-13, 10);

                //objのspriteを入れる操作
                image.sprite = SlotSprite;

                //現在入ってる属性のSpriteを枠に入れる操作
                if (selectedElementList[i] != null)
                {
                    GameObject ElementObj = AllElementSpriteList.Find(e => e.GetComponent<Image>().sprite == selectedElementList[i].sprite);
                    RectTransform ElementObjRT = ElementObj.GetComponent<RectTransform>();
                    ElementObjRT.anchoredPosition = targetPosition + RingObjectRT.anchoredPosition;
                }
            }
        }
        else //selectedElementListをnullにする操作
        {

            for (int i = selectedElementList.Count - 1; i >= 1; i--)
            {
                if (selectedElementList[i] != null)
                {
                    PopData(selectedElementList[i]);
                }
                Destroy(RingObjectParent.transform.GetChild(i).gameObject);
                selectedElementList.RemoveAt(i);
            }
        }
        RestSlotText();
    }

    public void ToggleElement(string elementDataName)
    {
        MagicData data = AllElementDataList.Find(e => e.name == elementDataName);
        if (data == null) return;

        if (selectedElementList.Contains(data))
        {
            PopData(data);
        }
        else
        {
            PushData(data);
        }
    }

    public void PushData(MagicData data)
    {
        Vector2 relativeVector = FirstMagicRT.anchoredPosition - RingObjectRT.anchoredPosition;

        for (int i = 0; i < selectedElementList.Count; i++)
        {
            if (selectedElementList[i] == null)
            {
                float angle = (360f / selectedElementList.Count) * i;
                Vector2 targetPosition = Quaternion.Euler(0, 0, angle) * relativeVector;
                targetPosition += RingObjectRT.anchoredPosition;
                GameObject SpriteObj = AllElementSpriteList.Find(e => e.GetComponent<Image>().sprite == data.sprite);
                RectTransform rectTransform = SpriteObj.GetComponent<RectTransform>();
                rectTransform.DOAnchorPos(targetPosition,1.0f).SetUpdate(true).SetId("ElementTween");
                
                selectedElementList[i] = data;
                return;
            }
        }
    }

    public void PopData(MagicData data)
    {
        for (int i = 0; i < selectedElementList.Count; i++)
        {
            if (selectedElementList[i] == data)
            {
                GameObject ElementObj = AllElementSpriteList.Find(e => e.GetComponent<Image>().sprite == data.sprite);
                GameObject ElementButton = AllElementButtonList.Find(e => e.GetComponent<Image>().sprite == data.sprite);
                ElementObj.GetComponent<RectTransform>().DOAnchorPos(ElementButton.GetComponent<RectTransform>().anchoredPosition,1.0f).SetUpdate(true);

                selectedElementList[i] = null;
                return;
            }
        }
    }

    public void RestSlotText()
    {
        SlotButtonText.text = "×" + (maxSelectedNum - selectedElementList.Count);
    }

    public void OnFireClick() => ToggleElement("FireData");
    public void OnWaterClick() => ToggleElement("WaterData");
    public void OnThunderClick() => ToggleElement("ThunderData");
    public void OnSoilClick() => ToggleElement("SoilData");
    public void OnWindClick() => ToggleElement("WindData");
}
