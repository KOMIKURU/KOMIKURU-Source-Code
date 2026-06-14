using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MagicRingController : MonoBehaviour
{
    private SelectedElement selectedElement;
    private MagicElement magicElement;
    [SerializeField] private GameObject RingObjectParent; //属性のSpriteを入れるための親オブジェクト。これを回転させる。
    [SerializeField] private RectTransform FirstMagicRectTransform; //最初の属性の位置。これを使って計算することで、1つのTrasformで複数の位置を指定できる。
    private Vector3 targetPosition; //属性のSpriteを生成する位置
    private Vector3 ObjectScale = new Vector3(228.99f, 222.53f, 0); //Spriteの大きさ
    private float targetRotZ = 0f; // 目標角度 

    public void Init(SelectedElement selectedElement, MagicElement magicElement)
    {
        this.selectedElement = selectedElement;
        this.magicElement = magicElement;
        // 初期角度を取得しておく
        if (RingObjectParent.transform != null)
        {
            targetRotZ = RingObjectParent.transform.localEulerAngles.z;
        }
        
    }

    public void SetElementRing()
    {

        RingObjectParent.transform.DOKill();
        //更新用の操作
        //子オブジェクト(前の選択属性のSprite)を全部削除する。
        foreach (Transform child in RingObjectParent.transform)
        {
            Destroy(child.gameObject);
        }
        RingObjectParent.transform.eulerAngles = new Vector3(0f,0f,0f);
        targetRotZ = 0f; //回転を初期化

        if (selectedElement.selectedElementList.Count > 0)
        {
            //生成の操作
            for (int i = 0; i < selectedElement.selectedElementList.Count; i++)
            {
                if (selectedElement.selectedElementList[i] != null)
                {
                    GameObject obj = new GameObject("ElementUI");
                    obj.transform.SetParent(RingObjectParent.transform, false);
                    Image image = obj.AddComponent<Image>();
                    RectTransform rectTransform = obj.GetComponent<RectTransform>();

                    //objの初期化(サイズと位置)
                    //位置については、FirstMagicRectTransformの位置と必要な回転量から計算して、求めている
                    rectTransform.sizeDelta = ObjectScale;
                    float angle = (360f / selectedElement.selectedElementList.Count) * i;
                    Vector3 FirstMagicVector = FirstMagicRectTransform.anchoredPosition;
                    targetPosition = Quaternion.Euler(0, 0, angle) * FirstMagicVector;
                    rectTransform.anchoredPosition = targetPosition;

                    rectTransform.rotation = Quaternion.Euler(0,0,i * (360f/selectedElement.selectedElementList.Count));
                    
                    //objのspriteを入れる操作
                    image.sprite = selectedElement.selectedElementList[i].slotElement;
                }
            }
        }
    }

    public void RightRotateZ()
    {
        if (DOTween.IsTweening(RingObjectParent.transform) || selectedElement.selectedElementList.Count <= 0) return;
        float temp = (targetRotZ + (360f / selectedElement.selectedElementList.Count)) % 360f;
        targetRotZ = temp - 0.01f;
        RingObjectParent.transform.DORotate(new Vector3(0, 0, targetRotZ), 0.5f, RotateMode.Fast).OnStart(() =>
        {
            magicElement.RightChangeElement();

        }).OnComplete(() =>
        {
            targetRotZ = temp;
            RingObjectParent.transform.DORotate(new Vector3(0, 0, targetRotZ), 0.01f, RotateMode.Fast);
        });
    }

    public void LeftRotateZ()
    {
        if (DOTween.IsTweening(RingObjectParent.transform) || selectedElement.selectedElementList.Count <= 0) return;
        float temp = (targetRotZ - (360f / selectedElement.selectedElementList.Count)) % 360f;
        targetRotZ = temp + 0.01f;
        RingObjectParent.transform.DORotate(new Vector3(0, 0, targetRotZ), 0.5f, RotateMode.Fast).OnStart(() =>
        {
            magicElement.LeftChangeElement();
        }).OnComplete(() =>
        {
            targetRotZ = temp;
            RingObjectParent.transform.DORotate(new Vector3(0, 0, targetRotZ), 0.01f, RotateMode.Fast);
        });
    }



}