using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float attackDelay = 1f;
    private bool isGrounded;
    private bool isAttacking = false;
    private bool isShowingTemporaryMessage = false;
    private Rigidbody2D rb;
    private Animator animator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (GameManager.Instance?.PlayerCharacter != null)
        {
            GameManager.Instance.PlayerCharacter.OnDamageTaken += HandleDamageTaken;
        }
        else 
        {
            Debug.LogError("GameManager or PlayerCharacter is missing");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal") * moveSpeed;
        rb.linearVelocity = new Vector2(horizontal, rb.linearVelocity.y);

        bool isRunning = horizontal != 0;
        animator.SetBool("isRunning", isRunning);
        animator.SetFloat("AirSpeed", rb.linearVelocity.y);

        if (isRunning && isGrounded)
        {
            animator.SetInteger("AnimState", 1);
        }
        else
        {
            animator.SetInteger("AnimState", 0);
        }

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            animator.SetTrigger("jump");
            animator.SetBool("Grounded", false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("Grounded", true);
            animator.SetBool("isFalling", false);
            // Debug.Log("Player landed.");
        }
    }
    private void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            // Debug.Log("Player attacked by mouse click.");
            animator.SetTrigger("Attack1");
            StartCoroutine(AttackCooldown());
        }
        // else
        // {
        //     Debug.Log("Player attacked.");
        // }
        // Debug.Log("Player attacked.");
    }
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
    }
      private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bread") && GameManager.Instance.canPickupBread)
        {
            Debug.Log("Bread detected by OnTriggerEnter2D");
            Sprite breadSprite = GameManager.Instance.Bread != null
                                ? GameManager.Instance.Bread.GetComponent<SpriteRenderer>().sprite
                                : null;
            if (breadSprite != null)
            {
                // Debug.Log("bread sprite found and will be added to inventory");
                // GameManager.Instance.AddtoInventory("Bread", breadSprite);
                GameManager.Instance.storyText.text = "You took the bread! It's been added to your inventory.";
                other.gameObject.SetActive(false);
                GameManager.Instance.storyProgress = 6;
                GameManager.Instance.UpdateStoryProgression();
            }
            else 
            {
                Debug.LogError("Bread sprite is null. Cannot add to inventory");
            }
        }
        else if (other.CompareTag("Bread") && !GameManager.Instance.canPickupBread)
        {
            if (!isShowingTemporaryMessage)
            {
                StartCoroutine(ShowTemporaryMessage("You need to decide to take the bread first."));

            }
        }
        else if (other.CompareTag("GoldenDagger") && GameManager.Instance.canPickupDagger)
        {
            GameManager.Instance.PlayerCharacter.Equip("Gold Dagger");
            GameManager.Instance.storyText.text = $"You picked up the Golden Dagger. It's now equipped! You also lost 10 gold. You now have {GameManager.Instance.PlayerCharacter.Gold} gold";
            other.gameObject.SetActive(false);
            GameManager.Instance.ProgressStory(6);
        }
        else if (other.CompareTag("GoldenDagger") && !GameManager.Instance.canPickupDagger)
        {
            GameManager.Instance.storyText.text = "You need to decide to leave tribute first";
        }
        else if (other.CompareTag("FlowerField"))
        {
            GameManager.Instance.storyText.text = "You arrive at a beautiful flower field amongst the forest. A cottage stands alone.";
        }
        else if (other.CompareTag("Cottage"))
        {
            GameManager.Instance.storyText.text = $"{GameManager.Instance.PlayerCharacter.Name}, will you enter the cottage?";
            GameManager.Instance.confirmNameButton.gameObject.SetActive(true);
            GameManager.Instance.confirmNameButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enter";
            GameManager.Instance.confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
            GameManager.Instance.confirmNameButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => 
                {
                    GameManager.Instance.OnEnterCottage();
                });
        }
        else if (other.CompareTag("HealthPotion") && GameManager.Instance.canPickupHealthPotion)
        {
            GameManager.Instance.PlayerCharacter.AddHealth(100);
            other.gameObject.SetActive(false);
            GameManager.Instance.UpdateHealthDisplay();

            GameManager.Instance.MemoryRestored();
        }
    }
    private IEnumerator ShowTemporaryMessage(string message)
    {
        isShowingTemporaryMessage = true;
        string previousText = GameManager.Instance.storyText.text;
        GameManager.Instance.storyText.text = message;
        yield return new WaitForSeconds(2f);
        GameManager.Instance.storyText.text = previousText;
        isShowingTemporaryMessage = false;
    }
    private void HandleDamageTaken(int damageAmount)
    {
        animator.SetTrigger("hurt");
    }
}
