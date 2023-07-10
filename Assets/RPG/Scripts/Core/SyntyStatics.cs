using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class SyntyStatics : MonoBehaviour
    {
        public enum Gender { Male, Female }

        public const string HairColor = "_Color_Hair";
        public const string SkinColor = "_Color_Skin";
        public const string BodyArtColor = "_Color_BodyArt";

        //Armor Color Cache
        public const string PrimaryColor = "_Color_Primary";
        public const string SecondaryColor = "_Color_Secondary";
        public const string LeatherPrimaryColor = "_Color_Leather_Primary";
        public const string LeatherSecondaryColor = "_Color_Leather_Secondary";
        public const string MetalPrimaryColor = "_Color_Metal_Primary";
        public const string MetalSecondaryColor = "_Color_Metal_Secondary";
        public const string MetalDarkColor = "_Color_Metal_Dark";

        //Armor Cache
        public static readonly string HeadCoverings_Base_Hair = "HeadCoverings_Base_Hair";
        public static readonly string HeadCoverings_No_FacialHair = "HeadCoverings_No_FacialHair";
        public static readonly string HeadCoverings_No_Hair = "HeadCoverings_No_Hair";
        public static readonly string All_01_Hair = "All_01_Hair";
        public static readonly string Helmet = "Helmet";
        public static readonly string Back_Attachment = "All_04_Back_Attachment";
        public static readonly string Shoulder_Attachment_Right = "All_05_Shoulder_Attachment_Right";
        public static readonly string Shoulder_Attachment_Left = "All_06_Shoulder_Attachment_Left";
        public static readonly string Elbow_Attachment_Right = "All_07_Elbow_Attachment_Right";
        public static readonly string Elbow_Attachment_Left = "All_08_Elbow_Attachment_Left";
        public static readonly string Hips_Attachment = "All_09_HipsAttachment";
        public static readonly string Knee_Attachment_Right = "All_10_Knee_Attachement_Right";
        public static readonly string Knee_Attachment_Left = "All_11_Knee_Attachement_Left";
        public static readonly string Elf_Ear = "Elf_Ear";

        public static readonly string[] AllGenderBodyParts = new string[]
        {
        "HeadCoverings_Base_Hair",
        "HeadCoverings_No_FacialHair",
        "HeadCoverings_No_Hair",
        "All_01_Hair",
        "Helmet",
        "All_04_Back_Attachment",
        "All_05_Shoulder_Attachment_Right",
        "All_06_Shoulder_Attachment_Left",
        "All_07_Elbow_Attachment_Right",
        "All_08_Elbow_Attachment_Left",
        "All_09_Hips_Attachment",
        "All_10_Knee_Attachement_Right",
        "All_11_Knee_Attachement_Left",
        "Elf_Ear"
        };

        public static readonly string Female_Head_All_Elements = "Female_Head_All_Elements";
        public static readonly string Female_Head_NoElements = "Female_Head_No_Elements";
        public static readonly string Female_Eyebrows = "Female_01_Eyebrows";
        public static readonly string Female_Torso = "Female_03_Torso";
        public static readonly string Female_Arm_Upper_Right = "Female_04_Arm_Upper_Right";
        public static readonly string Female_Arm_Upper_Left = "Female_05_Arm_Upper_Left";
        public static readonly string Female_Arm_Lower_Right = "Female_06_Arm_Lower_Right";
        public static readonly string Female_Arm_Lower_Left = "Female_07_Arm_Lower_Left";
        public static readonly string Female_Hand_Right = "Female_08_Hand_Right";
        public static readonly string Female_Hand_Left = "Female_09_Hand_Left";
        public static readonly string Female_Hips = "Female_10_Hips";
        public static readonly string Female_Leg_Right = "Female_11_Leg_Right";
        public static readonly string Female_Leg_Left = "Female_12_Leg_Left";

        public static readonly string[] FemaleBodyCategories = new string[]
        {
        "Female_Head_All_Elements",
        "Female_Head_No_Elements",
        "Female_01_Eyebrows",
        "Female_03_Torso",
        "Female_04_Arm_Upper_Right",
        "Female_05_Arm_Upper_Left",
        "Female_06_Arm_Lower_Right",
        "Female_07_Arm_Lower_Left",
        "Female_08_Hand_Right",
        "Female_09_Hand_Left",
        "Female_10_Hips",
        "Female_11_Leg_Right",
        "Female_12_Leg_Left",
        };

        public static readonly string Male_Head_All_Elements = "Male_Head_All_Elements";
        public static readonly string Male_Head_No_Elements = "Male_Head_No_Elements";
        public static readonly string Male_Eyebrows = "Male_01_Eyebrows";
        public static readonly string Male_FacialHair = "Male_02_FacialHair";
        public static readonly string Male_Torso = "Male_03_Torso";
        public static readonly string Male_Arm_Upper_Right = "Male_04_Arm_Upper_Right";
        public static readonly string Male_Arm_Upper_Left = "Male_05_Arm_Upper_Left";
        public static readonly string Male_Arm_Lower_Right = "Male_06_Arm_Lower_Right";
        public static readonly string Male_Arm_Lower_Left = "Male_07_Arm_Lower_Left";
        public static readonly string Male_Hand_Right = "Male_08_Hand_Right";
        public static readonly string Male_Hand_Left = "Male_09_Hand_Left";
        public static readonly string Male_Hips = "Male_10_Hips";
        public static readonly string Male_Leg_Right = "Male_11_Leg_Right";
        public static readonly string Male_Leg_Left = "Male_12_Leg_Left";

        public static readonly string[] MaleBodyCategories = new string[]
        {
        "Male_Head_All_Elements",
        "Male_Head_No_Elements",
        "Male_01_Eyebrows",
        "Male_02_FacialHair",
        "Male_03_Torso",
        "Male_04_Arm_Upper_Right",
        "Male_05_Arm_Upper_Left",
        "Male_06_Arm_Lower_Right",
        "Male_07_Arm_Lower_Left",
        "Male_08_Hand_Right",
        "Male_09_Hand_Left",
        "Male_10_Hips",
        "Male_11_Leg_Right",
        "Male_12_Leg_Left",
        };

        //Hair Color Cache
        public static Color blackColor = new Color(0.3098039f, 0.254902f, 0.1764706f);
        public static Color brownColor = new Color(0.333f, 0.162f, 0.014f);
        public static Color dirtyBlondeColor = new Color(0.468f, 0.383f, 0.071f);
        public static Color whiteColor = new Color(1f, 1f, 1f);
        public static Color blondeColor = new Color(0.9056604f, 0.8016192f, 0.06322531f);

        //Skin Color Cache
        public static Color whiteSkin = new Color(1f, 0.8000001f, 0.682353f);
        public static Color brownSkin = new Color(0.8196079f, 0.6352941f, 0.4588236f);
        public static Color blackSkin = new Color(0.5647059f, 0.4078432f, 0.3137255f);
        public static Color elfSkin = new Color(0.9607844f, 0.7843138f, 0.7294118f);

        //Stubble Colors Cache
        public static Color whiteStubble = new Color(0.8039216f, 0.7019608f, 0.6313726f);
        public static Color brownStubble = new Color(0.6588235f, 0.572549f, 0.4627451f);
        public static Color blackStubble = new Color(0.3882353f, 0.2901961f, 0.2470588f);
        public static Color elfStubble = new Color(0.8627452f, 0.7294118f, 0.6862745f);

        //Scar Colors Cache
        public static Color whiteScar = new Color(0.9294118f, 0.6862745f, 0.5921569f);
        public static Color brownScar = new Color(0.6980392f, 0.5450981f, 0.4f);
        public static Color blackScar = new Color(0.4235294f, 0.3176471f, 0.282353f);
        public static Color elfScar = new Color(0.8745099f, 0.6588235f, 0.6313726f);

        //Tattoo Color Cache
        public static Color redColor = new Color(0.823f, 0.064f, 0.035f);
        public static Color blueColor = new Color(0.0509804f, 0.6745098f, 0.9843138f);
        public static Color greenColor = new Color(0.3098039f, 0.7058824f, 0.3137255f);
        public static Color orangeColor = new Color(0.8862745f, 0.39301f, 0.0862745f);
        public static Color yellowColor = new Color(0.8867924f, 0.8002203f, 0.08784264f);
        public static Color purpleColor = new Color(0.490172f, 0.0862745f, 0.8862745f);
        public static Color pitchBlackColor = new Color(0f, 0f, 0f);

        public static Color[] hairColors = new Color[]
        {
            blackColor,
            brownColor,
            dirtyBlondeColor,
            orangeColor,
            redColor,
            blondeColor,
            whiteColor,
            blueColor,
            greenColor,
            purpleColor,
            pitchBlackColor
        };

        public static Color[] skinColors = new Color[]
        {
            whiteSkin,
            brownSkin,
            blackSkin,
            elfSkin
        };
        public static Color[] stubbleColors = new Color[]
        {
            whiteStubble,
            brownStubble,
            blackStubble,
            elfStubble
        };
        public static Color[] scarColors = new Color[]
        {
            whiteScar,
            brownScar,
            blackScar,
            elfScar
        };

        public static Color[] eyeColors = new Color[]
        {
            pitchBlackColor,
            greenColor,
            blueColor,
            yellowColor,
            orangeColor,
            redColor,
            whiteColor
        };

        public static Color[] bodyArtColors = new Color[]
        {
            greenColor,
            redColor,
            blueColor,
            orangeColor,
            yellowColor,
            purpleColor,
            whiteColor,
            pitchBlackColor
        };
        public static string[] GearColors = new string[]
        {
        PrimaryColor,
        SecondaryColor,
        LeatherPrimaryColor,
        LeatherSecondaryColor,
        MetalPrimaryColor,
        MetalSecondaryColor,
        MetalDarkColor
        };
    }

}
