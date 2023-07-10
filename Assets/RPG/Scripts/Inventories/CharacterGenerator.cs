using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using GameDevTV.Saving;
using UnityEngine;
using RPG.Inventories;
using System.Linq;
using Unity.Netcode;

[DisallowMultipleComponent]
[RequireComponent(typeof(Equipment))]
public class CharacterGenerator : MonoBehaviour, ISaveable
{
    /// <summary>
    /// A dictionary containing all of the modular parts, organized by category.
    /// Use this catalogue, not the static one when actually customizing the character.
    /// </summary>
    Dictionary<string, List<GameObject>> characterGameObjects;

    /// <summary>
    /// A dictionary containing all of the modular parts, organized by category.
    /// Use this catalogue, not the static one when actually customizing the character.
    /// </summary>
    Dictionary<string, List<GameObject>> CharacterGameObjects
    {
        get
        {
            InitGameObjects(); //This will build the dictionary if it hasn't yet been initialized.
            return characterGameObjects;
        }
    }

    [SerializeField] private bool playerClone = false;
    //Character Customization
    [SerializeField] SyntyStatics.Gender gender = SyntyStatics.Gender.Male;
    [Range(-1, 37)] [SerializeField] int hair = 0;
    [Range(-1, 3)] [SerializeField] int ears = -1;
    [Range(-1, 21)] [SerializeField] int head = 0;
    [Range(-1, 6)] [SerializeField] int eyebrow = 0;
    [Range(-1, 17)] [SerializeField] int facialHair = 0;
    //Armor System
    [Range(-1, 27)] [SerializeField] int defaultTorso = 1;
    [Range(-1, 20)] [SerializeField] int defaultUpperArm = 0;
    [Range(-1, 17)] [SerializeField] int defaultLowerArm = 0;
    [Range(-1, 16)] [SerializeField] int defaultHand = 0;
    [Range(-1, 27)] [SerializeField] int defaultHips = 0;
    [Range(-1, 18)] [SerializeField] int defaultLeg = 0;

    public bool isMale => gender == SyntyStatics.Gender.Male;
    [SerializeField] private bool pickup = false;
    public bool isPickup => pickup;

    //initialize colors for customization
    int hairColor = 0;
    int skinColor = 0;
    int stubbleColor = 0;
    int scarColor = 0;
    int bodyArtColor = 0;
    int eyeColor = 0;

    Equipment equipment; //saving armor changes once the armor system is implemented
    SelectHairColor selectHairColor;

    void Awake()
    {
        //playerClone = (CompareTag("PlayerPreview"));

        equipment = playerClone
            ? GameObject.FindWithTag("Player").GetComponent<Equipment>()
            : GetComponent<Equipment>();
        equipment.equipmentUpdated += LoadArmor;
        LoadDefaultCharacter();

    }

    private void Start()
    {

        LoadDefaultCharacter();
        if (pickup)
        {
            Pickup pickup = GetComponent<Pickup>();
            InventoryItem item = pickup.GetItem();
            if (item is EquipableItem equipable)
                equipment.AddItem(equipable.GetAllowedEquipLocation(), equipable);
        }

    }

    private void GetConfiguration()
    {

        CharacterGenerator otherGenerator = equipment.GetComponent<CharacterGenerator>();
        gender = otherGenerator.gender;
        hair = otherGenerator.hair;
        head = otherGenerator.head;
        ears = otherGenerator.ears;
        eyebrow = otherGenerator.eyebrow;
        facialHair = otherGenerator.facialHair;
        defaultTorso = otherGenerator.defaultTorso;
        defaultHand = otherGenerator.defaultHand;
        defaultHips = otherGenerator.defaultHips;
        defaultLeg = otherGenerator.defaultLeg;
        defaultLowerArm = otherGenerator.defaultLowerArm;
        defaultUpperArm = otherGenerator.defaultUpperArm;

    }

    public void InitGameObjects()
    {

        if (characterGameObjects != null) return;
        characterGameObjects = new Dictionary<string, List<GameObject>>();
        BuildCharacterGameObjectFromCatalogue(SyntyStatics.AllGenderBodyParts);
        BuildCharacterGameObjectFromCatalogue(SyntyStatics.MaleBodyCategories);
        BuildCharacterGameObjectFromCatalogue(SyntyStatics.FemaleBodyCategories);
    }

    void BuildCharacterGameObjectFromCatalogue(string[] catalogue)
    {

        foreach (string category in catalogue)
        {
            List<GameObject> list = new List<GameObject>();
            Transform t = GetComponentsInChildren<Transform>().FirstOrDefault(x => x.gameObject.name == category);
            if (t)
            {
                for (int i = 0; i < t.childCount; i++)
                {
                    Transform tr = t.GetChild(i);
                    if (tr == t) continue;
                    {
                        list.Add(tr.gameObject);
                        tr.gameObject.SetActive(false);
                    }
                }

                characterGameObjects[category] = list;
            }
            else
            {
                Debug.Log($"BuildFromCatalogue - {name} - has no {category} category!");
            }

            if (characterGameObjects.ContainsKey(category))
            {
                //Debug.Log($"Category {category}, objects {characterGameObjects[category].Count()}");
            }
        }
    }

    #region Character Generation

    /// <summary>
    /// Should only be called when creating the character or from within RestoreState()
    /// </summary>
    /// <param name="female"></param>
    public void SetGender(bool female)
    {

        gender = female ? SyntyStatics.Gender.Female : SyntyStatics.Gender.Male;
        LoadDefaultCharacter();
    }

    /// <summary>
    /// Should only be called when creating the character or from within RestoreState()
    /// </summary>
    /// <param name="index"></param>
    public void SetHairColor(int index)
    {

        if (index >= 0 && index < SyntyStatics.hairColors.Length)
        {
            hairColor = index;
        }
        
        SetColorInCategory(SyntyStatics.All_01_Hair, SyntyStatics.HairColor, SyntyStatics.hairColors[hairColor]);
        SetColorInCategory(SyntyStatics.Male_FacialHair, SyntyStatics.HairColor,
        SyntyStatics.hairColors[hairColor]);
        SetColorInCategory(SyntyStatics.Female_Eyebrows, SyntyStatics.HairColor,
        SyntyStatics.hairColors[hairColor]);
        SetColorInCategory(SyntyStatics.Male_Eyebrows, SyntyStatics.HairColor, SyntyStatics.hairColors[hairColor]);
    }

    /// <summary>
    /// Should only be called when creating the character.
    /// </summary>
    /// <param name="index"></param>
    public void CycleHairColor(int index)
    {

        hairColor += index;
        if (hairColor < 0) hairColor = SyntyStatics.hairColors.Length - 1;
        hairColor = hairColor % SyntyStatics.hairColors.Length;
        SetHairColor(hairColor);
    }


    /// <summary>
    /// Should only be called when creating the character.
    /// </summary>
    /// <param name="index"></param>
    public void CycleSkinColor(int index)
    {

        skinColor += index;
        if (skinColor < 0) skinColor += SyntyStatics.skinColors.Length - 1;
        skinColor = skinColor % SyntyStatics.skinColors.Length;
        SetSkinColor(skinColor);
    }

    /// <summary>
    /// Should only be called when creating the character.
    /// </summary>
    /// <param name="index"></param>
    public void CycleHairStyle(int index)
    {

        hair += index;
        int maxHairStyles = CharacterGameObjects[SyntyStatics.All_01_Hair].Count;
        if (hair < -1) hair = maxHairStyles - 1;
        //hair %= CharacterGameObjects[SyntyStatics.All_01_Hair].Count;
        if (hair >= maxHairStyles) hair = -1;
        ActivateHair(hair);
    }
    public void SetFacialHairStyle(int index)
    {

        facialHair = index;
        ActivateFacialHair(facialHair);
    }


    public void SetHairStyle(int index)
    {
;
        hair = index;
         ActivateHair(hair);
        

    }

    /// <summary>
    /// Should only be called when creating the character.
    /// </summary>
    /// <param name="index"></param>
    public void CycleFacialHair(int index)
    {

        facialHair += index;
        int maxHair = CharacterGameObjects[SyntyStatics.Male_FacialHair].Count;
        if (facialHair < -1) facialHair = maxHair - 1;
        if (facialHair >= maxHair) facialHair = -1;
        ActivateFacialHair(facialHair);
    }

    /// <summary>
    /// Should only be called when creating the character.
    /// </summary>
    /// <param name="index"></param>
    public void CycleHead(int index)
    {

        head += index;
        if (head < 0) head += CharacterGameObjects[SyntyStatics.Female_Head_All_Elements].Count - 1;
        head %= CharacterGameObjects[SyntyStatics.Female_Head_All_Elements].Count;
        ActivateHead(head);
    }

    public void CycleEars(int index)
    {

        ears += index;
        int maxEars = CharacterGameObjects[SyntyStatics.Elf_Ear].Count;
        if (ears < -1) ears = maxEars - 1;
        if (ears >= maxEars) ears = -1;
        ActivateElfEars(ears);
    }

    /// <summary>
    /// Should only be called when creating the character.
    /// </summary>
    /// <param name="index"></param>
    public void CycleEyebrows(int index)
    {
        eyebrow += index;
        if (eyebrow < 0) eyebrow += CharacterGameObjects[SyntyStatics.Female_Eyebrows].Count - 1;
        eyebrow %= CharacterGameObjects[SyntyStatics.Female_Eyebrows].Count;
        ActivateEyebrows(eyebrow);
    }

    public void CycleBodyArtColor(int index)
    {
        bodyArtColor += index;
        if (bodyArtColor < 0) bodyArtColor = SyntyStatics.bodyArtColors.Length - 1;
        bodyArtColor = bodyArtColor % SyntyStatics.bodyArtColors.Length;
        SetBodyArtColor(bodyArtColor);
    }

    public void CycleEyeColor(int index)
    {
        eyeColor += index;
        if (eyeColor < 0) eyeColor = SyntyStatics.eyeColors.Length - 1;
        eyeColor = eyeColor % SyntyStatics.eyeColors.Length;
        SetEyeColor(eyeColor);
    }

    /// <summary>
    /// Should only be called when creating the character.
    /// </summary>
    /// <param name="category"></param>
    /// <param name="shaderVariable"></param>
    /// <param name="colorToSet"></param>
    void SetColorInCategory(string category, string shaderVariable, Color colorToSet)
    {
        if (!CharacterGameObjects.ContainsKey(category)) return;
        foreach (GameObject go in CharacterGameObjects[category])
        {
            Renderer rend = go.GetComponent<Renderer>();
            rend.material.SetColor(shaderVariable, colorToSet);
        }
    }

    /// <summary>
    /// Should only be called when creating the character or from RestoreState
    /// </summary>
    /// <param name="index"></param>
    public void SetSkinColor(int index)
    {
        if (index >= 0 && index < SyntyStatics.skinColors.Length)
        {
            skinColor = index;
        }
        if (index >= 0 && index < SyntyStatics.stubbleColors.Length)
        {
            stubbleColor = index;
        }
        if (index >= 0 && index < SyntyStatics.scarColors.Length)
        {
            scarColor = index;
        }

        foreach (var pair in CharacterGameObjects)
        {
            SetColorInCategory(pair.Key, "_Color_Skin", SyntyStatics.skinColors[skinColor]);
            SetColorInCategory(pair.Key, "_Color_Stubble", SyntyStatics.stubbleColors[stubbleColor]);
            SetColorInCategory(pair.Key, "_Color_Scar", SyntyStatics.scarColors[scarColor]);
        }
    }

    public void SetBodyArtColor(int index)
    {
        if (index >= 0 && index < SyntyStatics.bodyArtColors.Length)
        {
            bodyArtColor = index;
        }
        foreach (var pair in CharacterGameObjects)
        {
            SetColorInCategory(pair.Key, "_Color_BodyArt", SyntyStatics.bodyArtColors[bodyArtColor]);
        }
    }

    public void SetEyeColor(int index)
    {
        if (index >= 0 && index < SyntyStatics.eyeColors.Length)
        {
            eyeColor = index;
        }
        foreach (var pair in CharacterGameObjects)
        {
            SetColorInCategory(pair.Key, "_Color_Eyes", SyntyStatics.eyeColors[eyeColor]);
        }
    }

    #endregion

    #region CharacterActivation

    /// <summary>
    /// This sets the character to the default state, assuming no items in the EquipmentManager. 
    /// </summary>
    /// 

    public void LoadDefaultCharacter()
    {

        foreach (var pair in CharacterGameObjects)
        {
            foreach (var item in pair.Value)
            {
                item.SetActive(false);
            }
        }

        if (pickup) return;

        ActivateHair(hair);
        ActivateHead(head);
        ActivateElfEars(ears);
        ActivateEyebrows(eyebrow);
        ActivateFacialHair(facialHair);
        ActivateTorso(defaultTorso);
        ActivateUpperArm(defaultUpperArm);
        ActivateLowerArm(defaultLowerArm);
        ActivateHand(defaultHand);
        ActivateHips(defaultHips);
        ActivateLeg(defaultLeg);


    }


    public void LoadArmor()
    {
        if (equipment == null) equipment = GetComponent<Equipment>();
        LoadDefaultCharacter();
        foreach (var pair in equipment.EquippedItems) // add the line -> public Dictionary<EquipLocation, EquipableItem> EquippedItems => equippedItems; to Equipment.cs under STATE.
        {
            if (pair.Value is SyntyEquipableItem item)
            //Debug.Log(pair.Key.GetDisplayName());
            {
                foreach (string category in item.SlotsToDeactivate)
                {
                    DeactivateCategory(category);
                }

                var colorChanger = item.ColorChangers;
                if (gender == SyntyStatics.Gender.Male)
                {
                    foreach (SyntyEquipableItem.ItemPair itemPair in item.ObjectsToActivateM)
                    {
                        //Debug.Log($"{itemPair.category}-{itemPair.index}");
                        switch (itemPair.category)
                        {
                            case "Leg":
                                ActivateLeg(itemPair.index, colorChanger);
                                break;
                            case "Hips":
                                ActivateHips(itemPair.index, colorChanger);
                                break;
                            case "Torso":
                                ActivateTorso(itemPair.index, colorChanger);
                                break;
                            case "UpperArm":
                                ActivateUpperArm(itemPair.index, colorChanger);
                                break;
                            case "LowerArm":
                                ActivateLowerArm(itemPair.index, colorChanger);
                                break;
                            case "Hand":
                                ActivateHand(itemPair.index, colorChanger);
                                break;
                            default:
                                ActivatePart(itemPair.category, itemPair.index, colorChanger);
                                break;
                        }
                    }
                }
                else if (gender == SyntyStatics.Gender.Female)
                {
                    foreach (SyntyEquipableItem.ItemPair itemPair in item.ObjectsToActivateF)
                    {
                        //Debug.Log($"{itemPair.category}-{itemPair.index}");
                        switch (itemPair.category)
                        {
                            case "Leg":
                                ActivateLeg(itemPair.index, colorChanger);
                                break;
                            case "Hips":
                                ActivateHips(itemPair.index, colorChanger);
                                break;
                            case "Torso":
                                ActivateTorso(itemPair.index, colorChanger);
                                break;
                            case "UpperArm":
                                ActivateUpperArm(itemPair.index, colorChanger);
                                break;
                            case "LowerArm":
                                ActivateLowerArm(itemPair.index, colorChanger);
                                break;
                            case "Hand":
                                ActivateHand(itemPair.index, colorChanger);
                                break;
                            default:
                                ActivatePart(itemPair.category, itemPair.index, colorChanger);
                                break;
                        }
                    }
                }


            }
        }
    }
    void ActivateElfEars(int selector)
    {
        ActivatePart(SyntyStatics.Elf_Ear, selector);
    }

    void ActivateLeg(int selector, List<SyntyEquipableItem.ItemColor> colorChanges = null)
    {
        ActivatePart(gender == SyntyStatics.Gender.Male ? SyntyStatics.Male_Leg_Left : SyntyStatics.Female_Leg_Left,
            selector, colorChanges);
        ActivatePart(
            gender == SyntyStatics.Gender.Male ? SyntyStatics.Male_Leg_Right : SyntyStatics.Female_Leg_Right,
            selector, colorChanges);
        DeactivateCategory(isMale ? SyntyStatics.Female_Leg_Left : SyntyStatics.Male_Leg_Left);
        DeactivateCategory(isMale ? SyntyStatics.Female_Leg_Right : SyntyStatics.Male_Leg_Right);
    }

    void ActivateHips(int selector, List<SyntyEquipableItem.ItemColor> colorChanges = null)
    {
        ActivatePart(gender == SyntyStatics.Gender.Male ? SyntyStatics.Male_Hips : SyntyStatics.Female_Hips,
            selector, colorChanges);
        DeactivateCategory(isMale ? SyntyStatics.Female_Hips : SyntyStatics.Male_Hips);
    }

    void ActivateHand(int selector, List<SyntyEquipableItem.ItemColor> colorChanges = null)
    {
        ActivatePart(
            gender == SyntyStatics.Gender.Male ? SyntyStatics.Male_Hand_Right : SyntyStatics.Female_Hand_Right,
            selector, colorChanges);
        ActivatePart(
            gender == SyntyStatics.Gender.Male ? SyntyStatics.Male_Hand_Left : SyntyStatics.Female_Hand_Left,
            selector, colorChanges);
        DeactivateCategory(isMale ? SyntyStatics.Female_Hand_Right : SyntyStatics.Male_Hand_Right);
        DeactivateCategory(isMale ? SyntyStatics.Female_Hand_Left : SyntyStatics.Male_Hand_Left);
    }

    void ActivateLowerArm(int selector, List<SyntyEquipableItem.ItemColor> colorChanges = null)
    {
        ActivatePart(
            gender == SyntyStatics.Gender.Male
                ? SyntyStatics.Male_Arm_Lower_Right
                : SyntyStatics.Female_Arm_Lower_Right, selector,
            colorChanges);
        ActivatePart(
            gender == SyntyStatics.Gender.Male
                ? SyntyStatics.Male_Arm_Lower_Left
                : SyntyStatics.Female_Arm_Lower_Left, selector,
            colorChanges);
        DeactivateCategory(isMale ? SyntyStatics.Female_Arm_Lower_Right : SyntyStatics.Male_Arm_Lower_Right);
        DeactivateCategory(isMale ? SyntyStatics.Female_Arm_Lower_Left : SyntyStatics.Male_Arm_Lower_Left);
    }

    void ActivateUpperArm(int selector, List<SyntyEquipableItem.ItemColor> colorChanges = null)
    {
        ActivatePart(isMale ? SyntyStatics.Male_Arm_Upper_Right : SyntyStatics.Female_Arm_Upper_Right, selector,
            colorChanges);
        ActivatePart(isMale ? SyntyStatics.Male_Arm_Upper_Left : SyntyStatics.Female_Arm_Upper_Left, selector,
            colorChanges);
        DeactivateCategory(isMale ? SyntyStatics.Female_Arm_Upper_Right : SyntyStatics.Male_Arm_Upper_Right);
        DeactivateCategory(isMale ? SyntyStatics.Female_Arm_Upper_Left : SyntyStatics.Male_Arm_Upper_Left);
    }

    void ActivateTorso(int selector, List<SyntyEquipableItem.ItemColor> colorChanges = null)
    {
        ActivatePart(isMale ? SyntyStatics.Male_Torso : SyntyStatics.Female_Torso, selector, colorChanges);
        DeactivateCategory(isMale ? SyntyStatics.Female_Torso : SyntyStatics.Male_Torso);
    }

    void ActivateFacialHair(int selector)
    {
        if (!isMale)
        {
            DeactivateCategory(SyntyStatics.Male_FacialHair);
            return;
        }

        ActivatePart(SyntyStatics.Male_FacialHair, selector);
    }

    void ActivateEyebrows(int selector)
    {
        ActivatePart(isMale ? SyntyStatics.Male_Eyebrows : SyntyStatics.Female_Eyebrows, selector);
        DeactivateCategory(isMale ? SyntyStatics.Female_Eyebrows : SyntyStatics.Male_Eyebrows);
    }

    void ActivateHead(int selector)
    {
        ActivatePart(isMale ? SyntyStatics.Male_Head_All_Elements : SyntyStatics.Female_Head_All_Elements,
            selector);
        DeactivateCategory(isMale ? SyntyStatics.Female_Head_All_Elements : SyntyStatics.Male_Head_All_Elements);
    }


    void ActivateHair(int selector)
    {
        ActivatePart(SyntyStatics.All_01_Hair, selector);
    }


    private Dictionary<GameObject, Material> materialDict = new Dictionary<GameObject, Material>();

    void ActivatePart(string identifier, int selector, List<SyntyEquipableItem.ItemColor> colorChanges = null)
    {
        if (selector < 0)
        {
            DeactivateCategory(identifier);
            return;
        }

        if (!CharacterGameObjects.ContainsKey(identifier))
        {
            Debug.Log($"{name} - {identifier} not found in dictionary");
            return;
        }

        if ((CharacterGameObjects[identifier].Count < selector))
        {
            Debug.Log($"Index {selector}out of range for {identifier}");
            return;
        }

        DeactivateCategory(identifier);
        GameObject go = CharacterGameObjects[identifier][selector];
        go.SetActive(true);
        if (colorChanges == null) return;
        foreach (var pair in colorChanges)
        {
            SetColor(go, pair.category, pair.color);
        }
    }

    void DeactivateCategory(string identifier)
    {
        if (!CharacterGameObjects.ContainsKey(identifier))
        {
            Debug.LogError($"Category {identifier} not found in database!");
            return;
        }

        foreach (GameObject g in CharacterGameObjects[identifier])
        {
            g.SetActive(false);
        }
    }

    #endregion

    #region StaticDictionary

    /// <summary>
    /// This static dictionary is for a hook for the custom editors for EquipableItem and other Editor windows.
    /// Outside of this, it should not be used as a reference to get/set items on the character because it is
    /// terribly inefficient for this purpose.
    /// </summary>
    static Dictionary<string, List<string>> characterParts;

    public static Dictionary<string, List<string>> CharacterParts
    {
        get
        {
            InitCharacterParts();
            return characterParts;
        }
    }

    public static void InitCharacterParts()
    {
        if (characterParts != null) return;
        GameObject character = Resources.Load<GameObject>("PolyFantasyHeroBase");
        if (character == null) Debug.Log("Unable to find Character!");
        characterParts = new Dictionary<string, List<string>>();
        BuildCategory(SyntyStatics.AllGenderBodyParts, character);
        BuildCategory(SyntyStatics.FemaleBodyCategories, character);
        BuildCategory(SyntyStatics.MaleBodyCategories, character);
        character = null;
    }

    static void BuildCategory(IEnumerable<string> parts, GameObject source)
    {
        foreach (string category in parts)
        {
            List<string> items = new List<string>();
            if (source == null)
            {
                Debug.Log("Source Not Loaded?");
            }
            else
            {
                Debug.Log($"Source is {source.name}");
            }

            Debug.Log($"Testing {category}");
            Transform t = source.GetComponentsInChildren<Transform>().First(x => x.gameObject.name == category);
            if (t == null)
            {
                Debug.Log($"Unable to locate {category}");
            }
            else
            {
                Debug.Log($"Category {t.name}");
            }

            foreach (Transform tr in t.gameObject.GetComponentsInChildren<Transform>())
            {
                if (tr == t) continue;
                GameObject go = tr.gameObject;
                Debug.Log($"Adding {go.name}");
                items.Add(go.name);
            }

            characterParts[category] = items;
            Debug.Log(characterParts[category].Count);
        }
    }

    #endregion

    #region ISaveable

    [System.Serializable]
    public struct ModularData
    {
        public bool isMale;
        public int hair;
        public int facialHair;
        public int head;
        public int ears;
        public int eyebrow;
        public int skinColor;
        public int hairColor;
        public int bodyArtColor;
        public int eyeColor;

        public ModularData(bool _isMale, int _hair, int _facialHair, int _head, int _ears, int _eyebrow, int _skinColor,
                           int _hairColor, int _bodyArtColor, int _eyeColor)
        {
            isMale = _isMale;
            hair = _hair;
            facialHair = _facialHair;
            head = _head;
            ears = _ears;
            eyebrow = _eyebrow;
            skinColor = _skinColor;
            hairColor = _hairColor;
            bodyArtColor = _bodyArtColor;
            eyeColor = _eyeColor;
        }
    }

    public object CaptureState()
    {
        return (new ModularData(isMale, hair, facialHair, head, ears, eyebrow, skinColor, hairColor, bodyArtColor, eyeColor));
    }


    public void RestoreState(object state)
    {


        equipment.equipmentUpdated -= LoadArmor; //prevent issues
        ModularData data = (ModularData)state;

        gender = data.isMale ? SyntyStatics.Gender.Male : SyntyStatics.Gender.Female;
        hair = data.hair;
        facialHair = data.facialHair;
        head = data.head;
        ears = data.ears;
        eyebrow = data.eyebrow;
        skinColor = data.skinColor;
        hairColor = data.hairColor;
        bodyArtColor = data.bodyArtColor;
        eyeColor = data.eyeColor;

        SetHairColor(hairColor);
        SetSkinColor(skinColor);
        SetBodyArtColor(bodyArtColor);
        SetEyeColor(eyeColor);
        equipment.equipmentUpdated += LoadArmor;
        Invoke(nameof(LoadArmor), .1f);

    }

    #endregion

    /* This section is used by the EquipmentBuilder scene only */

    #region EquipmentBuilder

    public int SetParameter(string parameterString, int value, int i)
    {
        if (!CharacterGameObjects.ContainsKey(parameterString))
            return TryAlternateParameter(parameterString, value, i);
        int available = CharacterGameObjects[parameterString].Count;
        value += i;
        if (value >= available) value = -1;
        else if (value < -1) value = available - 1;
        ActivatePart(parameterString, value);
        return value;
    }

    int TryAlternateParameter(string parameterString, int value, int i)
    {
        switch (parameterString)
        {
            case "Torso":
                value = CycleValue(SyntyStatics.Male_Torso, value, i);
                ActivateTorso(value);
                break;
            case "UpperArm":
                value = CycleValue(SyntyStatics.Male_Arm_Upper_Left, value, i);
                ActivateUpperArm(value);
                break;
            case "LowerArm":
                value = CycleValue(SyntyStatics.Male_Arm_Lower_Left, value, i);
                ActivateLowerArm(value);
                break;
            case "Hand":
                value = CycleValue(SyntyStatics.Male_Hand_Left, value, i);
                ActivateHand(value);
                break;
            case "Hips":
                value = CycleValue(SyntyStatics.Male_Hips, value, i);
                ActivateHips(value);
                break;
            case "Leg":
                value = CycleValue(SyntyStatics.Male_Leg_Left, value, i);
                ActivateLeg(value);
                break;
            default:
                value = -999;
                break;
        }

        return value;
    }

    int CycleValue(string parameterString, int value, int i)
    {
        int available = CharacterGameObjects[parameterString].Count;
        value += i + available;
        value %= available;
        return value;
    }

    void SetColor(GameObject item, string parameterString, Color colorToSet)
    {
        //Color colorToSet = Color.white;
        //colorToSet = SyntyStatics.GetColor(parameterString, value);

        {
            Material mat;
            if (materialDict.ContainsKey(item) && materialDict[item] != null)
            {
                mat = materialDict[item];
            }
            else
            {
                mat = Instantiate(item.GetComponent<Renderer>().sharedMaterial);
                item.GetComponent<Renderer>().material = mat;
                materialDict[item] = mat;
            }

            mat.SetColor(parameterString, colorToSet);
            //item.GetComponent<Renderer>().material.SetColor(parameterString, colorToSet);
        }
    }

    #endregion
}
