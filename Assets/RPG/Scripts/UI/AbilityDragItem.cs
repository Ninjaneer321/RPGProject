using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Core.UI.Dragging;
using GameDevTV.Inventories;
using RPG.Abilities;
using GameDevTV.UI.Inventories;
using UnityEngine.EventSystems;

/// <summary>
/// To be placed on icons representing the item in a slot. Allows the item
/// to be dragged into other slots.
/// </summary>
public class AbilityDragItem : DragItem<Ability>, IBeginDragHandler
{

}

