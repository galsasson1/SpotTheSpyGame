using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The individual data for every person in the scene, simplified into enums to make data management easier
/// </summary>
public class PersonData : MonoBehaviour
{
    [SerializeField] private SpriteRenderer faceAccessoriesRenderer;
    [SerializeField] private List<Sprite> faceAccessoriesOptions;
    private FaceAccessories[] faceAccessoriesMale = { FaceAccessories.NONE, FaceAccessories.GLASSES, FaceAccessories.SCAR };
    private FaceAccessories[] faceAccessoriesFemale = { FaceAccessories.NONE, FaceAccessories.GLASSES, FaceAccessories.CROWN, FaceAccessories.BUNNY_EARS, FaceAccessories.HEADPHONES, FaceAccessories.HAT };

    [SerializeField] private SpriteRenderer frontHairRenderer;
    [SerializeField] private SpriteRenderer backHairRenderer;

    [System.Serializable] private struct HairSprites { public Sprite backHair; public Sprite frontHair; }

    [SerializeField] private List<HairSprites> hairOptions;
    private HairType[] hairMale = { HairType.BALD, HairType.BALDING, HairType.BUZZCUT };
    private HairType[] hairFemale = { HairType.CURLY, HairType.STRAIGHT };

    [SerializeField] private SpriteRenderer facialHairRenderer;
    [SerializeField] private List<Sprite> facialHairOptions;
    private FacialHairType[] facialHairMale = {FacialHairType.NONE, FacialHairType.BEARD };
    private FacialHairType[] facialHairFemale = { FacialHairType.NONE, FacialHairType.EYEBROWS, FacialHairType.FRECKLES};

    [SerializeField] private SpriteRenderer clothesRenderer;
    [SerializeField] private List<Sprite> clothesOptions;
    private ClothesType[] clothesMale = { ClothesType.TSHIRT, ClothesType.SUIT ,ClothesType.OVERALLS };
    private ClothesType[] clothesFemale = { ClothesType.TSHIRT, ClothesType.DRESS, ClothesType.SKIRT, ClothesType.SUIT, ClothesType.OVERALLS };


    [SerializeField] private List<Color> colorOptions;
    [SerializeField] private PropertyColor[] hairColors = {PropertyColor.BLACK, PropertyColor.GREY, PropertyColor.WHITE, PropertyColor.BROWN, PropertyColor.BLONDE, PropertyColor.RED };


    [Header("Properties")]
    public int personID;
    public Gender _gender;
    [Header("Face")]
    public FaceAccessories _faceAccessoriesType;
    public PropertyColor _faceAccessoriesColor;
    [Header("Hair")]
    public HairLength _hairLength;
    public HairType _hairType;
    public PropertyColor _hairColor;
    [Header("Facial Hair")]
    public FacialHairType _facialHairType;
    public PropertyColor _facialHairColor;
    [Header("Clothes")]
    public ClothesType _clothesType;
    public PropertyColor _clothesColor;

    public void Awake()
    {
        RandomaizePerson();
    }

    public void RandomaizePerson()
    {
        _gender = (Gender)Random.Range(0, 2);

        _facialHairType = _gender switch
        {
            Gender.MASCULINE => facialHairMale[Random.Range(0, facialHairMale.Length)],
            Gender.FEMININE => facialHairFemale[Random.Range(0, facialHairFemale.Length)],
            _ => throw new System.NotImplementedException()
        };
        _facialHairColor = _facialHairType switch
        {
            FacialHairType.NONE => PropertyColor.NONE,
            FacialHairType.FRECKLES => PropertyColor.BROWN,
            _ => hairColors[Random.Range(0, hairColors.Length)]
        };
        facialHairRenderer.sprite = facialHairOptions[(int)_facialHairType];
        facialHairRenderer.color = colorOptions[(int)_facialHairColor];

        _faceAccessoriesType = _gender switch
        {
            Gender.MASCULINE => faceAccessoriesMale[Random.Range(0, faceAccessoriesMale.Length)],
            Gender.FEMININE => faceAccessoriesFemale[Random.Range(0, faceAccessoriesFemale.Length)],
            _ => throw new System.NotImplementedException()
        };
        _faceAccessoriesColor = _faceAccessoriesType switch
        {
            FaceAccessories.NONE => PropertyColor.NONE,
            FaceAccessories.GLASSES => PropertyColor.BLACK,
            FaceAccessories.SCAR => PropertyColor.BROWN,
            _ => (PropertyColor)Random.Range(1, colorOptions.Count)
        };
        faceAccessoriesRenderer.sprite = faceAccessoriesOptions[(int)_faceAccessoriesType];
        faceAccessoriesRenderer.color = colorOptions[(int)_faceAccessoriesColor];


        _hairType = _gender switch
        {
            Gender.MASCULINE => hairMale[Random.Range(0, hairMale.Length)],
            Gender.FEMININE => hairFemale[Random.Range(0, hairFemale.Length)],
            _ => throw new System.NotImplementedException()
        };
        _hairLength = _hairType switch
        {
            HairType.BALD => HairLength.BALD,
            HairType.BALDING => HairLength.BALDING,
            HairType.BUZZCUT => HairLength.SHORT,
            HairType.CURLY => HairLength.MEDIUM,
            HairType.STRAIGHT => HairLength.LONG,
            _ => throw new System.NotImplementedException()
        };
        _hairColor = _hairType switch
        {
            HairType.BALD => PropertyColor.NONE,
            _ => hairColors[Random.Range(0, hairColors.Length)]
        };
        frontHairRenderer.sprite = hairOptions[(int)_hairType].frontHair;
        backHairRenderer.sprite = hairOptions[(int)_hairType].backHair;
        frontHairRenderer.color = colorOptions[(int)_hairColor];
        backHairRenderer.color = colorOptions[(int)_hairColor];

        _clothesType = _gender switch
            {
                Gender.MASCULINE => clothesMale[Random.Range(0, clothesMale.Length)],
                Gender.FEMININE => clothesFemale[Random.Range(0, clothesFemale.Length)],
                _ => throw new System.NotImplementedException()

            };
        _clothesColor = _clothesType switch
        {
            _ => (PropertyColor)Random.Range(1, colorOptions.Count)
        };
        clothesRenderer.sprite = clothesOptions[(int)_clothesType];
        clothesRenderer.color = colorOptions[(int)_clothesColor];
    }
}
