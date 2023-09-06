using System.Collections;
using System.Collections.Generic;
using GameDevTV.UI.Inventories;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameDevTV.Core.UI.Dragging
{
    /// <summary>
    /// Allows a UI element to be dragged and dropped from and to a container.
    /// 
    /// Create a subclass for the type you want to be draggable. Then place on
    /// the UI element you want to make draggable.
    /// 
    /// During dragging, the item is reparented to the parent canvas.
    /// 
    /// After the item is dropped it will be automatically return to the
    /// original UI parent. It is the job of components implementing `IDragContainer`,
    /// `IDragDestination and `IDragSource` to update the interface after a drag
    /// has occurred.
    /// </summary>
    /// <typeparam name="T">The type that represents the item being dragged.</typeparam>
    public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
        where T : class
    {
        // PRIVATE STATE
        [SerializeField] Vector3 startPosition;
        [SerializeField] Transform originalParent;
        //[SerializeField] GameObject duplicateObjectForAbilityDragging;
        IDragSource<T> source;

        // CACHED REFERENCES
        Canvas parentCanvas;

        // LIFECYCLE METHODS
        private void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            source = GetComponentInParent<IDragSource<T>>();
        }

        // PRIVATE
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            startPosition = transform.position;
            originalParent = transform.parent;
            if (transform.GetComponent<AbilityDragItem>())
            {
                var duplicateObject = GameObject.Instantiate(this.gameObject, startPosition, transform.rotation, originalParent);
                duplicateObject.tag = "DuplicatedAbility";
            }
            if (transform.GetComponent<InventoryDragItem>())
            {
                gameObject.tag = "DuplicatedActionItem";
            }
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            transform.SetParent(parentCanvas.transform, true);
 
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            transform.position = startPosition;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            transform.SetParent(originalParent, true);
            if (transform.GetComponent<AbilityDragItem>())
            {
                //Deletes duplicated object forcefully. Ideally we only want this to happen if the ability is dropped without a container.
                var duplicatedObject = GameObject.FindWithTag("DuplicatedAbility");
                Destroy(duplicatedObject);
            }

            IDragDestination<T> container;
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                container = parentCanvas.GetComponent<IDragDestination<T>>();
            }
            else
            {
                container = GetContainer(eventData);
            }

            if (container != null)
            {
                DropItemIntoContainer(container);
            }
            else
            {
                Debug.Log("No container");
            }
            
        }

        private IDragDestination<T> GetContainer(PointerEventData eventData)
        {
            if (eventData.pointerEnter)
            {
                var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();

                return container;
            }
            return null;
        }

        protected void DropItemIntoContainer(IDragDestination<T> destination)
        {
            if (object.ReferenceEquals(destination, source)) return;

            var destinationContainer = destination as IDragContainer<T>;
            var sourceContainer = source as IDragContainer<T>;
            // Swap won't be possible
            if (destinationContainer == null || sourceContainer == null ||
                destinationContainer.GetItem() == null ||
                object.ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
            {            
                AttemptSimpleTransfer(destination);
                return;
            }

            AttemptSwap(destinationContainer, sourceContainer);
        }

        private void AttemptSwap(IDragContainer<T> destination, IDragContainer<T> source)
        {
            // Provisionally remove item from both sides.
            Debug.Log("AttemptSwap");
            var removedSourceNumber = source.GetNumber();
            var removedSourceItem = source.GetItem();
            var removedDestinationNumber = destination.GetNumber();
            var removedDestinationItem = destination.GetItem();

            source.RemoveItems(removedSourceNumber);
            destination.RemoveItems(removedDestinationNumber);

            var sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, source, destination);
            var destinationTakeBackNumber = CalculateTakeBack(removedDestinationItem, removedDestinationNumber, destination, source);

            // Do take backs (if needed)
            if (sourceTakeBackNumber > 0)
            {
                source.AddItems(removedSourceItem, sourceTakeBackNumber);
                removedSourceNumber -= sourceTakeBackNumber;
            }
            if (destinationTakeBackNumber > 0)
            {
                destination.AddItems(removedDestinationItem, destinationTakeBackNumber);
                removedDestinationNumber -= destinationTakeBackNumber;
            }

            // Abort if we can't do a successful swap
            if (source.MaxAcceptable(removedDestinationItem) < removedDestinationNumber ||
                destination.MaxAcceptable(removedSourceItem) < removedSourceNumber ||
                removedSourceNumber == 0)
            {
                if (removedDestinationNumber > 0)
                {
                    destination.AddItems(removedDestinationItem, removedDestinationNumber);
                }
                if (removedSourceNumber > 0)
                {
                    source.AddItems(removedSourceItem, removedSourceNumber);
                }
                return;
            }

            // Do swaps
            if (removedDestinationNumber > 0)
            {
                source.AddItems(removedDestinationItem, removedDestinationNumber);
            }
            if (removedSourceNumber > 0)
            {
                destination.AddItems(removedSourceItem, removedSourceNumber);
            }
        }

        private bool AttemptSimpleTransfer(IDragDestination<T> destination)
        {
            //This gets called whenever we try to swap an Ability with an Item, and vice versa.
            Debug.Log("AttemptSimpleTransfer");
            var draggingItem = source.GetItem();
            var draggingNumber = source.GetNumber();
            var acceptable = destination.MaxAcceptable(draggingItem);
            var toTransfer = Mathf.Min(acceptable, draggingNumber);
            if (toTransfer > 0)
            {
                source.RemoveItems(toTransfer);
                destination.AddItems(draggingItem, toTransfer);
                return false;
            }

            return true;
        }

        private int CalculateTakeBack(T removedItem, int removedNumber, IDragContainer<T> removeSource, IDragContainer<T> destination)
        {
            var takeBackNumber = 0;
            var destinationMaxAcceptable = destination.MaxAcceptable(removedItem);

            if (destinationMaxAcceptable < removedNumber)
            {
                takeBackNumber = removedNumber - destinationMaxAcceptable;

                var sourceTakeBackAcceptable = removeSource.MaxAcceptable(removedItem);

                // Abort and reset
                if (sourceTakeBackAcceptable < takeBackNumber)
                {
                    return 0;
                }
            }
            return takeBackNumber;
        }
    }
}