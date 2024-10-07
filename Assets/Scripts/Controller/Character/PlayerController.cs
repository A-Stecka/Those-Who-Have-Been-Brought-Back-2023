using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public bool hasBackpack, understandsGuardians, canReadGravestone;

    [Header("Dialogue display")]
    public GameObject dialogueCanvas;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueLineText;
    public Sprite hintSprite, interactSprite;
    public SpriteRenderer spriteInDialogue;
    public string characterName;
    public string hint;

    [Header("Backpack display")]
    public GameObject backpackCanvas;
    public GameObject[] backpackItems;
    public GameObject backpackIcon;
    public GameObject backpackHint;
    public AudioSource audioSource;

    [Header("Shadow display")]
    public GameObject shadow;

    [Header("Pause display")]
    public GameObject pauseCanvas;

    public static InputActions inputActions;
    
    private List<Sprite> _backpack;
    private int _backpackIndex;
    private bool _movementEnabled;
    
    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    private readonly string _dynamicTag = "DynamicallyAdded";

    void Awake()
    {
        if (GlobalController.Instance != null && !GlobalController.Instance.firstLoadOfScene)
        {
            if (GlobalController.Instance.nextScene == 1 && GlobalController.Instance.currentScene == 2)
            {
                GameObject fromArea2 = GameObject.Find("Area_2 start");
                this.transform.position = new Vector3(fromArea2.transform.position.x, fromArea2.transform.position.y, this.transform.position.z);
            }
            if (GlobalController.Instance.nextScene == 1 && GlobalController.Instance.currentScene == 3)
            {
                GameObject fromArea3 = GameObject.Find("Area_3 start");
                this.transform.position = new Vector3(fromArea3.transform.position.x, fromArea3.transform.position.y, this.transform.position.z);
            }
            if (GlobalController.Instance.nextScene == 2 && GlobalController.Instance.currentScene == 1)
            {
                GameObject fromArea1 = GameObject.Find("Area_1 start");
                this.transform.position = new Vector3(fromArea1.transform.position.x, fromArea1.transform.position.y, this.transform.position.z);
            }
            if (GlobalController.Instance.nextScene == 2 && GlobalController.Instance.currentScene == 5)
            {
                GameObject fromArea5 = GameObject.Find("Area_5 start");
                this.transform.position = new Vector3(fromArea5.transform.position.x, fromArea5.transform.position.y, this.transform.position.z);
            }
            if (GlobalController.Instance.nextScene == 3 && GlobalController.Instance.currentScene == 1)
            {
                GameObject fromArea1 = GameObject.Find("Area_1 start");
                this.transform.position = new Vector3(fromArea1.transform.position.x, fromArea1.transform.position.y, this.transform.position.z);
            }
            if (GlobalController.Instance.nextScene == 3 && GlobalController.Instance.currentScene == 4)
            {
                GameObject fromArea4 = GameObject.Find("Area_4 start");
                this.transform.position = new Vector3(fromArea4.transform.position.x, fromArea4.transform.position.y, this.transform.position.z);
            }
            if (GlobalController.Instance.nextScene == 4 && GlobalController.Instance.currentScene == 3)
            {
                GameObject fromArea3 = GameObject.Find("Area_3 start");
                this.transform.position = new Vector3(fromArea3.transform.position.x, fromArea3.transform.position.y, this.transform.position.z);
            }
            if (GlobalController.Instance.nextScene == 5 && GlobalController.Instance.currentScene == 2)
            {
                GameObject fromArea2 = GameObject.Find("Area_2 start");
                this.transform.position = new Vector3(fromArea2.transform.position.x, fromArea2.transform.position.y, this.transform.position.z);
            }

            if (GlobalController.Instance.loadingFromSave)
            {
                this.transform.position = new Vector3(GlobalController.Instance.playerData.x, GlobalController.Instance.playerData.y, this.transform.position.z);
                GlobalController.Instance.loadingFromSave = false;
            }

            _backpack = GlobalController.Instance.playerData.backpack;
            hint = GlobalController.Instance.playerData.hint;
            _backpackIndex = GlobalController.Instance.playerData.backpackIndex;
            hasBackpack = GlobalController.Instance.playerData.hasBackpack;
            understandsGuardians = GlobalController.Instance.playerData.understandsGuardians;
            canReadGravestone = GlobalController.Instance.playerData.canReadGravestone;
            speed = GlobalController.Instance.playerData.speed;
        }
        else
        {
            if (GlobalController.Instance == null)
            {
                _backpack = new List<Sprite>();
                _backpackIndex = 0;
            }
            else
            {
                _backpack = GlobalController.Instance.playerData.backpack;
                _backpackIndex = GlobalController.Instance.playerData.backpackIndex;
            }

            if (GlobalController.Instance != null && GlobalController.Instance.nextScene != 0)
            {
                speed = GlobalController.Instance.playerData.speed;
            }
        }

        _movementEnabled = true;

        inputActions = new InputActions();

        inputActions.SinglePlayer.Pause.performed += Pause;
        inputActions.SinglePlayer.Hint.performed += Hint;

        if (hasBackpack)
        {
            inputActions.SinglePlayer.Next.performed += BackpackNext;
            inputActions.SinglePlayer.Previous.performed += BackpackPrevious;
            EnableBackpack();
            DisplayBackpackContents();
        }

        _rigidbody2D = this.GetComponent<Rigidbody2D>();
        _animator = this.GetComponentInChildren<Animator>();
        _spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();

        AnimateForward();
        SavePlayer();
    }

    private void OnEnable()
    {
        inputActions.SinglePlayer.Enable();
    }

    private void OnDisable()
    {
        inputActions.SinglePlayer.Disable();
    }

    private void OnDestroy()
    {
        SavePlayer();
    }

    void Update()
    {
        if (_movementEnabled)
        {
            Move();
        }
        SavePosition();
    }

    private void Move()
    {
        Vector2 moveInput = inputActions.SinglePlayer.Move.ReadValue<Vector2>();
        _rigidbody2D.velocity = moveInput * speed;
        if (moveInput.x < 0)
        {
            _animator.SetInteger("Direction", 1);
            _spriteRenderer.flipX = false;
        } 
        if (moveInput.x > 0)
        {
            _animator.SetInteger("Direction", 1);
            _spriteRenderer.flipX = true;
        }
        if (moveInput.y < 0)
        {
            _animator.SetInteger("Direction", 0);
            _spriteRenderer.flipX = false;
        }
        if (moveInput.y > 0)
        {
            _animator.SetInteger("Direction", 2);
            _spriteRenderer.flipX = false;
        }
    }

    private void DisplayBackpackContents()
    {
        var previous = GameObject.FindGameObjectsWithTag(_dynamicTag);
        for (int i = 0; i < previous.Length; i++)
        {
            Destroy(previous[i]);
        }


        for (int i = 0; i < _backpack.Count; i++)
        {
            var temp = Instantiate(backpackIcon, new Vector3(0, 20, 0), Quaternion.identity);
            RectTransform tempTransform = temp.GetComponent<RectTransform>();

            temp.tag = _dynamicTag;
            tempTransform.localScale = new Vector3(0.6f, 0.6f, 1);
            tempTransform.anchorMin = new Vector2(0.5f, 0);
            tempTransform.anchorMax = new Vector2(0.5f, 0);
            tempTransform.SetParent(backpackItems[i].transform, false);

            SpriteRenderer tempSpriteRenderer = temp.GetComponentInChildren<SpriteRenderer>();

            if (i != _backpackIndex)
            {
                tempSpriteRenderer.color = new Color(tempSpriteRenderer.color.r, tempSpriteRenderer.color.g, tempSpriteRenderer.color.b, 0.5f);
            }
            else
            {
                tempSpriteRenderer.color = new Color(tempSpriteRenderer.color.r, tempSpriteRenderer.color.g, tempSpriteRenderer.color.b, 1.0f);
            }
            tempSpriteRenderer.sprite = _backpack[i];
        }
        DisplayBackpackUI();
    }

    private void DisplayBackpackUI()
    {
        for (int i = 0; i < backpackItems.Length; i++)
        {
            if (i != _backpackIndex)
            {
                BackpackItemTransparency(0.5f, i);
            }
            else
            {
                BackpackItemTransparency(1.0f, i);
            }
        }
    }

    private void BackpackItemTransparency(float alpha, int index)
    {
        SpriteRenderer[] _spriteRenderers = backpackItems[index].GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < _spriteRenderers.Length; i++)
        {
            _spriteRenderers[i].color = new Color(_spriteRenderers[i].color.r, _spriteRenderers[i].color.g, _spriteRenderers[i].color.b, alpha);
        }
    }

    void Pause(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        this.PlayerMovement(false);
        pauseCanvas.SetActive(true);
    }

    void Hint(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!dialogueCanvas.activeInHierarchy)
        {
            inputActions.SinglePlayer.Hint.performed -= Hint;

            this.PlayerMovement(false);
            nameText.text = characterName;
            dialogueLineText.text = hint;
            spriteInDialogue.sprite = hintSprite;
            dialogueCanvas.SetActive(true);

            inputActions.SinglePlayer.Hint.performed += CloseHint;
        }
    }

    void CloseHint(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        dialogueCanvas.SetActive(false);
        spriteInDialogue.sprite = interactSprite;
        this.PlayerMovement(true);
        inputActions.SinglePlayer.Hint.performed -= CloseHint;
        inputActions.SinglePlayer.Hint.performed += Hint;
    }

    void BackpackPrevious(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!dialogueCanvas.activeInHierarchy && _backpackIndex > 0)
        {
            BackpackItemTransparency(0.5f, _backpackIndex);
            _backpackIndex--;
            BackpackItemTransparency(1.0f, _backpackIndex);
        }
        SavePlayer();
    }

    void BackpackNext(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!dialogueCanvas.activeInHierarchy && _backpackIndex < backpackItems.Length - 1)
        {
            BackpackItemTransparency(0.5f, _backpackIndex);
            _backpackIndex++;
            BackpackItemTransparency(1.0f, _backpackIndex);
        }
        SavePlayer();
    }

    public void PlayerMovement(bool movementEnabled)
    {
        _movementEnabled = movementEnabled;
        if (_rigidbody2D != null)
        {
            _rigidbody2D.velocity = new Vector2(0, 0);
        }
    }

    public void ChangeHint(string hint)
    {
        this.hint = hint;
        SavePlayer();
    }

    public void EnableBackpack()
    {
        hasBackpack = true;
        backpackCanvas.SetActive(true);
        DisplayBackpackContents();
        backpackHint.SetActive(true);

        inputActions.SinglePlayer.Previous.performed += BackpackPrevious;
        inputActions.SinglePlayer.Next.performed += BackpackNext;
        SavePlayer();
    }

    public void AddToBackpack(Sprite sprite)
    {
        audioSource.Play();
        _backpack.Add(sprite);
        DisplayBackpackContents();
        SavePlayer();
    }

    public void RemoveFromBackpack(Sprite sprite)
    {
        audioSource.Play();
        _backpack.Remove(sprite);
        DisplayBackpackContents();
        SavePlayer();
    }

    public bool HasInBackpack(Sprite sprite)
    {
        return _backpack.Contains(sprite);
    }

    public Sprite SelectedItem()
    {
        if (_backpackIndex < _backpack.Count)
        {
            return _backpack[_backpackIndex];
        }
        return null;
    }

    public void AnimateForward()
    {
        _animator.SetInteger("Direction", 0);
    }

    public void SetAnimation(int value)
    {
        _animator.SetInteger("Direction", value);
        shadow.GetComponent<Animator>().SetInteger("Direction", value);
    }

    public void HideShadow()
    {
        shadow.SetActive(false);
    }

    public void End()
    {
        StartCoroutine(Wait());
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(12);
        GlobalController.Instance.currentScene = 0;
        GlobalController.Instance.nextScene = 6;
        GlobalController.Instance.SaveGame();
        SceneManager.LoadScene("Menu");
    }

    public void DisplayCutscene()
    {
        DialogueController controller = gameObject.GetComponent<DialogueController>();
        if (controller != null)
        {
            PlayerMovement(false);
            controller.DisplayText();
        }
    }

    private void SavePlayer()
    {
        if (GlobalController.Instance != null)
        {
            SavePosition();
            GlobalController.Instance.playerData.hasBackpack = hasBackpack;
            GlobalController.Instance.playerData.understandsGuardians = understandsGuardians;
            GlobalController.Instance.playerData.canReadGravestone = canReadGravestone;
            GlobalController.Instance.playerData.backpack = _backpack;
            GlobalController.Instance.playerData.hint = hint;
            GlobalController.Instance.playerData.backpackIndex = _backpackIndex;
            GlobalController.Instance.playerData.speed = speed;
        }
    }

    private void SavePosition()
    {
        if (GlobalController.Instance != null)
        {
            GlobalController.Instance.playerData.x = this.transform.position.x;
            GlobalController.Instance.playerData.y = this.transform.position.y;
        }
    }

}
