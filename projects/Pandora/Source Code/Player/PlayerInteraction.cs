using UnityEngine;

public interface IInteractable
{
    void Interact();
    void SetInteractUI(bool value);
}

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentTarget;
    [SerializeField] private PlayerInputHandler inputHandler;
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EnvironmentContext env;


    void Update()
    {
        if (DialogueManager.Instance.isDialogueActive)
        {
            return;
        }
        DetectInteractable();
    }

    void OnEnable()
    {
        if (inputHandler != null)
        {
            inputHandler.OnInteractAction += TryInteract;
        }
    }

    void OnDisable()
    {
        if (inputHandler != null)
        {
            inputHandler.OnInteractAction -= TryInteract;
        }
    }

    void TryInteract()
    {
        // ?? 修正点: イベント駆動だが、競合防止のガードは必要
        // DialogueManagerがアクティブな間は、このイベント処理は無視される
        if (DialogueManager.Instance.isDialogueActive)
        {
            return;
        }

        // ?? 修正点: InteractPressedフラグのチェックは不要になった
        if (currentTarget != null&&env.IsGrounded)
        {
            currentTarget.Interact();
        }
    }

    void DetectInteractable()
    {
        Collider2D hit = Physics2D.OverlapCircle(spriteRenderer.bounds.center, interactRadius, LayerMask.GetMask("NPC"));
        if (hit)
        {
            //Debug.Log("NPCと接触");
            var interactable = hit.GetComponent<IInteractable>();
            if (interactable != null && interactable != currentTarget)
            {
                //Debug.Log("NPC範囲内");
                currentTarget?.SetInteractUI(false);
                currentTarget = interactable;
                currentTarget.SetInteractUI(true);
            }
        }
        else if (currentTarget != null)
        {
            //Debug.Log("NPC範囲外");
            currentTarget.SetInteractUI(false);
            currentTarget = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spriteRenderer.bounds.center, interactRadius);
    }
}


