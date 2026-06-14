using DG.Tweening;
using UnityEngine;

public class MagicGenerate : MonoBehaviour
{

    [SerializeField] private Transform magicShotPoint;
    [SerializeField] private Transform viewTransform;
    private MagicPoint magicPoint;
    private MagicElement magicElement;
    private GameObject currentMagic;
    private MagicData currentData;
    public void Init(MagicPoint magicPoint,MagicElement magicElement)
    {
        this.magicPoint = magicPoint;
        this.magicElement = magicElement;
    }
       
    public void magicGanerate()
    {
        currentData = magicElement.currentElement; //MagicDataを参照
        if (currentData != null)
        {
            magicPoint.setUseMPAmount(currentData.cost); //使う魔法量の設定
            currentMagic = currentData.prefab; //生成する魔法属性の設定

            if (magicPoint.currentMP >= magicPoint.useMPAmount) //現在のMPが使用する魔法のMPCostより高かったら、発動できるように
            {
                GameObject magicInstance = Instantiate(currentMagic, magicShotPoint.position, Quaternion.identity);
                MagicMove magicMove = magicInstance.GetComponent<MagicMove>();
                magicMove?.SetDirection(viewTransform.rotation.y == 0f ? 1 : -1); //移動方向の設定
                magicPoint?.UseMagic();
            }
        }
    }
}