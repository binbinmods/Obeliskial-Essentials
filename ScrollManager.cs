using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static Obeliskial_Essentials.Essentials;

namespace Obeliskial_Essentials
{

    public class ScrollButtonCreator : MonoBehaviour
    {
        [Header("Button Settings")]
        public Sprite leftArrowSprite;
        public Sprite rightArrowSprite;


        public GameObject CreateLeftButton()
        {
            return CreateButton("LeftScrollButton", leftArrowSprite);
        }

        public GameObject CreateRightButton()
        {
            return CreateButton("RightScrollButton", rightArrowSprite);
        }

        private GameObject CreateButton(string buttonName, Sprite arrowSprite)
        {
            // Create button object
            GameObject buttonObj = new GameObject(buttonName);
            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(0.5f, 0.5f);

            // Add image component for visual
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.sprite = arrowSprite;
            buttonImage.color = new Color(1, 0.5f, 0.5f, 0.8f);

            // Add button component
            Button button = buttonObj.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(1, 1, 1, 0.8f);
            colors.highlightedColor = Color.white;
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1);
            button.colors = colors;

            // Add box collider for interaction
            BoxCollider2D collider = buttonObj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.5f, 0.5f);

            return buttonObj;
        }
    }



    public class ScrollController : MonoBehaviour
    {
        [Header("Scroll Settings")]
        public int visibleItemCount = 5;
        public float scrollSpeed = 0.5f;
        public Transform itemContainer;
        public GameObject leftButton;
        public GameObject rightButton;

        [Header("Optional Settings")]
        public bool wrapAround = false;
        // public float scrollSensitivity = 0.1f;

        private List<GameObject> itemsList = new List<GameObject>();
        private int startIndex = 0;
        private int totalItems = 0;
        private float itemWidth = 0;

        [Header("Scroll Settings")]
        public float scrollThreshold = 0.3f;
        public float scrollCooldown = 0.05f;

        private float scrollAccumulator = 0f;
        private float lastScrollTime = 0f;

        // private bool isScrolling = false;
        // private float scrollTarget = 0;

        void Start()
        {
            // Collect all child objects in the container
            RefreshItemsList();

            // Set initial visibility states
            UpdateVisibility();

            // Add button listeners
            if (leftButton != null)
            {
                Button leftBtn = leftButton.GetComponent<Button>();
                if (leftBtn != null)
                    leftBtn.onClick.AddListener(ScrollLeft);
            }

            if (rightButton != null)
            {
                Button rightBtn = rightButton.GetComponent<Button>();
                if (rightBtn != null)
                    rightBtn.onClick.AddListener(ScrollRight);
            }
        }




        void Update()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.01f && IsMouseOverContainer())
            {
                // Add to accumulator
                scrollAccumulator += scrollInput;

                bool cooldownElapsed = (Time.time - lastScrollTime) >= scrollCooldown;

                if (cooldownElapsed && Mathf.Abs(scrollAccumulator) >= scrollThreshold)
                {
                    if (scrollAccumulator > 0)
                        ScrollLeft();
                    else
                        ScrollRight();

                    scrollAccumulator = 0f;
                    lastScrollTime = Time.time;
                }
            }
            else
            {
                scrollAccumulator = Mathf.Lerp(scrollAccumulator, 0, Time.deltaTime * 2);
            }

            UpdateButtonStates();
        }

        private void ScrollToCurrentIndex()
        {
            float targetPos = -startIndex * itemWidth;

            itemContainer.localPosition = new Vector3(targetPos, itemContainer.localPosition.y, itemContainer.localPosition.z);
        }

        public void RefreshItemsList()
        {
            itemsList.Clear();

            for (int i = 0; i < itemContainer.childCount; i++)
            {
                GameObject item = itemContainer.GetChild(i).gameObject;
                itemsList.Add(item);

                // Calculate item width based on first item
                if (i == 0)
                {
                    RectTransform rect = item.GetComponent<RectTransform>();
                    if (rect != null)
                        itemWidth = rect.rect.width * item.transform.localScale.x;
                    else
                        itemWidth = 1.75f; // Fallback based on the original code
                }
            }

            totalItems = itemsList.Count;
            UpdateButtonStates();
        }

        public void ScrollLeft()
        {
            if (startIndex > 0 || wrapAround)
            {
                startIndex--;
                if (startIndex < 0 && wrapAround)
                    startIndex = totalItems - 1;
                else if (startIndex < 0)
                    startIndex = 0;

                UpdateVisibility();
                ScrollToCurrentIndex();
            }
        }

        public void ScrollRight()
        {
            if (startIndex < totalItems - visibleItemCount || wrapAround)
            {
                startIndex++;
                if (startIndex > totalItems - 1 && wrapAround)
                    startIndex = 0;
                else if (startIndex > totalItems - visibleItemCount)
                    startIndex = totalItems - visibleItemCount;

                UpdateVisibility();
                ScrollToCurrentIndex();
            }
        }

        private void UpdateVisibility()
        {
            // If we have fewer items than visible count, show all
            if (totalItems <= visibleItemCount)
            {
                foreach (GameObject item in itemsList)
                {
                    item.SetActive(true);
                }
                return;
            }

            // Otherwise update visibility based on the current index
            for (int i = 0; i < totalItems; i++)
            {
                bool isVisible = (i >= startIndex && i < startIndex + visibleItemCount);
                itemsList[i].SetActive(isVisible);
            }
        }

        private bool IsMouseOverContainer()
        {
            return true;
            // // Check if mouse is over this container
            // RectTransform rect = GetComponent<RectTransform>();
            // if (rect != null)
            // {
            //     Vector2 localMousePosition = rect.InverseTransformPoint(Input.mousePosition);
            //     return rect.rect.Contains(localMousePosition);
            // }
            // return false;
        }

        private void UpdateButtonStates()
        {
            leftButton?.SetActive(wrapAround || startIndex > 0);
            rightButton?.SetActive(wrapAround || startIndex < totalItems - visibleItemCount);
        }

        // Call this when new items are added or removed
        public void RefreshController()
        {
            RefreshItemsList();
            UpdateVisibility();
        }
    }


    public class HeroSelectionScrollSystem : MonoBehaviour
    {
        [Header("Scroll Containers")]
        public GameObject warriorScrollContainer;
        public GameObject scoutScrollContainer;
        public GameObject mageScrollContainer;
        public GameObject healerScrollContainer;
        public GameObject dlcScrollContainer;

        [Header("Button Prefabs")]
        public GameObject scrollLeftButtonPrefab;
        public GameObject scrollRightButtonPrefab;

        private Dictionary<string, ScrollController> scrollControllers = new Dictionary<string, ScrollController>();

        void Awake()
        {
            SetupScrollingForCategory("warrior", HeroSelectionManager.Instance.warriorsGO);
            SetupScrollingForCategory("scout", HeroSelectionManager.Instance.scoutsGO);
            SetupScrollingForCategory("mage", HeroSelectionManager.Instance.magesGO);
            SetupScrollingForCategory("healer", HeroSelectionManager.Instance.healersGO);
            SetupScrollingForCategory("dlc", HeroSelectionManager.Instance.dlcsGO);

        }

        private void SetupScrollingForCategory(string categoryName, GameObject categoryContainer)
        {
            if (categoryContainer == null)
                return;

            LogDebug($"SetupScrollingForCategory {categoryName}");

            GameObject scrollContainer = new GameObject(categoryName + "ScrollContainer");
            scrollContainer.transform.SetParent(categoryContainer.transform, false);
            scrollContainer.transform.localPosition = Vector3.zero;

            GameObject itemsContainer = new GameObject(categoryName + "Items");
            itemsContainer.transform.SetParent(scrollContainer.transform, false);
            itemsContainer.transform.localPosition = Vector3.zero;

            GameObject leftButton;
            GameObject rightButton;

            if (scrollLeftButtonPrefab != null)
            {
                LogDebug("Scrolling - Using Button Prefabs");
                leftButton = Instantiate(scrollLeftButtonPrefab, scrollContainer.transform);
            }
            else
                leftButton = CreateSimpleButton("LeftButton", true);

            if (scrollRightButtonPrefab != null)
                rightButton = Instantiate(scrollRightButtonPrefab, scrollContainer.transform);
            else
                rightButton = CreateSimpleButton("RightButton", false);

            float num4 = 1.75f;
            float y = -0.65f;

            float scaleX = 0.7f;
            float scaleY = 1.0f;

            leftButton.transform.SetParent(scrollContainer.transform);
            leftButton.transform.localPosition = new Vector3(num4 * -0.3f, y * 1.25f, 0);
            leftButton.transform.localScale = new Vector3(scaleX, scaleY, 1);
            leftButton.name = "LeftButton";

            LogDebug($"SetupScrollingForCategory - LeftButton Position and World Position {leftButton.transform.localPosition}, {leftButton.transform.position}");


            rightButton.transform.SetParent(scrollContainer.transform);
            rightButton.transform.localPosition = new Vector3(num4 * 5f, y * 1.25f, 0);
            rightButton.transform.localScale = new Vector3(scaleX, scaleY, 1);
            rightButton.name = "RightButton";

            LogDebug($"SetupScrollingForCategory - RightButton Position and World Position {rightButton.transform.localPosition}, {rightButton.transform.position}");

            ScrollController scrollController = scrollContainer.AddComponent<ScrollController>();
            scrollController.itemContainer = itemsContainer.transform;
            scrollController.leftButton = leftButton;
            scrollController.rightButton = rightButton;
            scrollController.visibleItemCount = 5;

            scrollControllers[categoryName] = scrollController;

            StoreContainerReference(categoryName, itemsContainer);
            scrollContainer.SetActive(true);
            itemsContainer.SetActive(true);

            BoxCollider2D leftCollider = leftButton.GetComponent<BoxCollider2D>() ?? leftButton.AddComponent<BoxCollider2D>();
            // leftCollider.size = new Vector2(0.5f, 0.25f);
            leftCollider.size = new Vector2(1.1f/scaleX, 1.1f/scaleY);
            BoxCollider2D rightCollider = rightButton.GetComponent<BoxCollider2D>() ?? rightButton.AddComponent<BoxCollider2D>();
            // rightCollider.size = new Vector2(0.5f, 0.25f);
            rightCollider.size = new Vector2(1.1f/scaleX, 1.1f/scaleY);

            ButtonClickHandler leftClickHandler = leftButton.AddComponent<ButtonClickHandler>();
            leftClickHandler.scrollController = scrollContainer.GetComponent<ScrollController>();
            leftClickHandler.isLeftButton = true;

            ButtonClickHandler rightClickHandler = rightButton.AddComponent<ButtonClickHandler>();
            rightClickHandler.scrollController = scrollContainer.GetComponent<ScrollController>();
            rightClickHandler.isLeftButton = false;
        }

        private GameObject CreateSimpleButton(string name, bool isLeftButton)
        {
            GameObject buttonObj = new GameObject(name);

            SpriteRenderer renderer = buttonObj.AddComponent<SpriteRenderer>();

            Texture2D texture = new Texture2D(64, 64);
            Color[] colors = new Color[64 * 64];

            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.clear;
            }

            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    int index = y * 64 + x;
                    if (x <= 32 && y >= x && y <= 64 - x)
                    {
                        colors[index] = Color.green;
                    }
                    
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
            
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f));
            renderer.sprite = sprite;
            renderer.flipX = isLeftButton;

            BoxCollider2D collider = buttonObj.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(1.0f, 1.0f);

            return buttonObj;
        }

        private void StoreContainerReference(string categoryName, GameObject container)
        {
            LogDebug("StoreContainerReference");
            switch (categoryName)
            {
                case "warrior":
                    warriorScrollContainer = container;
                    break;
                case "scout":
                    scoutScrollContainer = container;
                    break;
                case "mage":
                    mageScrollContainer = container;
                    break;
                case "healer":
                    healerScrollContainer = container;
                    break;
                case "dlc":
                    dlcScrollContainer = container;
                    break;
            }
        }

        public void RefreshAllScrollControllers()
        {
            foreach (var controller in scrollControllers.Values)
            {
                controller.RefreshController();
            }
        }

        public Transform GetScrollContainerFor(string category)
        {
            LogDebug("GetScrollContainerFor");
            if (scrollControllers.TryGetValue(category.ToLower(), out ScrollController controller))
            {
                return controller.itemContainer;
            }
            return null;
        }
    }


    public class ButtonClickHandler : MonoBehaviour
    {
        public ScrollController scrollController;
        public bool isLeftButton;

        private void OnMouseDown()
        {
            Debug.Log($"Button clicked: {(isLeftButton ? "Left" : "Right")}");

            if (scrollController != null)
            {
                if (isLeftButton)
                {
                    scrollController.ScrollLeft();
                }
                else
                {
                    scrollController.ScrollRight();
                }
            }
            else
            {
                Debug.LogError("ButtonClickHandler: No scroll controller assigned!");
            }
        }
    }
}
