//  SaveData.cs
//  http://kan-kikuchi.hatenablog.com/entry/Json_SaveData
//
//  Created by kan.kikuchi on 2016.11.21.

using UnityEngine;

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// �N���X���ۂ���Json�ŕۑ�����f�[�^�N���X
/// </summary>
[Serializable]
public class SaveData : ISerializationCallbackReceiver
{

    //�V���O���g�����������邽�߂̎��́A���A�N�Z�X����Load����B
    private static SaveData _instance = null;
    public static SaveData Instance
    {
        get
        {
            if (_instance == null)
            {
                Load();
            }
            return _instance;
        }
    }

    //SaveData��Json�ɕϊ������e�L�X�g(�����[�h���ɉ��x���ǂݍ��܂Ȃ��Ă����悤�ɕێ�)
    [SerializeField]
    private static string _jsonText = "";

    //=================================================================================
    //�ۑ������f�[�^(public or SerializeField��t����)
    //=================================================================================

    /*
    public int SampleInt = 10;
    public string SampleString = "Sample";
    public bool SampleBool = false;

    public List<int> SampleIntList = new List<int>() { 2, 3, 5, 7, 11, 13, 17, 19 };

    [SerializeField]
    private string _sampleDictJson = "";
    public Dictionary<string, int> SampleDict = new Dictionary<string, int>(){
    {"Key1", 50},
    {"Key2", 150},
    {"Key3", 550}
    };
    */
    public string playerName;
    public float sensitivity = 1f;
    public DuelerData customizeData = new DuelerData();
    public InputDuelerKM keyConfig = new InputDuelerKM();

    public SaveData()
    {
        customizeData = new DuelerData();
        keyConfig = new InputDuelerKM();
    }

    //=================================================================================
    //�V���A���C�Y,�f�V���A���C�Y���̃R�[���o�b�N
    //=================================================================================

    /// <summary>
    /// SaveData��Json�ɕϊ������O�Ɏ��s�����B
    /// </summary>
    public void OnBeforeSerialize()
    {
        //Dictionary�͂��̂܂܂ŕۑ�����Ȃ��̂ŁA�V���A���C�Y���ăe�L�X�g�ŕۑ��B
        //_sampleDictJson = Serialize(SampleDict);
    }

    /// <summary>
    /// Json��SaveData�ɕϊ����ꂽ��Ɏ��s�����B
    /// </summary>
    public void OnAfterDeserialize()
    {
        //�ۑ�����Ă���e�L�X�g������΁ADictionary�Ƀf�V���A���C�Y����B
        /*
        if (!string.IsNullOrEmpty(_sampleDictJson))
        {
            SampleDict = Deserialize<Dictionary<string, int>>(_sampleDictJson);
        }
        */
    }

    //�����̃I�u�W�F�N�g���V���A���C�Y���ĕԂ�
    private static string Serialize<T>(T obj)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream();
        binaryFormatter.Serialize(memoryStream, obj);
        return Convert.ToBase64String(memoryStream.GetBuffer());
    }

    //�����̃e�L�X�g���w�肳�ꂽ�N���X�Ƀf�V���A���C�Y���ĕԂ�
    private static T Deserialize<T>(string str)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(str));
        return (T)binaryFormatter.Deserialize(memoryStream);
    }

    //=================================================================================
    //�擾
    //=================================================================================

    /// <summary>
    /// �f�[�^���ēǂݍ��݂���B
    /// </summary>
    public void Reload()
    {
        JsonUtility.FromJsonOverwrite(GetJson(), this);
    }

    //�f�[�^��ǂݍ��ށB
    private static void Load()
    {
        _instance = JsonUtility.FromJson<SaveData>(GetJson());
    }

    //�ۑ����Ă���Json���擾����
    private static string GetJson()
    {
        //����Json���擾���Ă���ꍇ�͂����Ԃ��B
        if (!string.IsNullOrEmpty(_jsonText))
        {
            return _jsonText;
        }

        //Json��ۑ����Ă���ꏊ�̃p�X���擾�B
        string filePath = GetSaveFilePath();

        //Json�����݂��邩���ׂĂ���擾���ϊ�����B���݂��Ȃ���ΐV���ȃN���X���쐬���A�����Json�ɕϊ�����B
        if (File.Exists(filePath))
        {
            _jsonText = File.ReadAllText(filePath);
        }
        else
        {
            _jsonText = JsonUtility.ToJson(new SaveData());
        }

        return _jsonText;
    }

    //=================================================================================
    //�ۑ�
    //=================================================================================

    /// <summary>
    /// �f�[�^��Json�ɂ��ĕۑ�����B
    /// </summary>
    public void Save()
    {
        _jsonText = JsonUtility.ToJson(this);
        File.WriteAllText(GetSaveFilePath(), _jsonText);
    }

    //=================================================================================
    //�폜
    //=================================================================================

    /// <summary>
    /// �f�[�^��S�č폜���A����������B
    /// </summary>
    public void Delete()
    {
        _jsonText = JsonUtility.ToJson(new SaveData());
        Reload();
    }

    //=================================================================================
    //�ۑ���̃p�X
    //=================================================================================

    //�ۑ�����ꏊ�̃p�X���擾�B
    private static string GetSaveFilePath()
    {

        string filePath = "SaveData";

        //�m�F���₷���悤�ɃG�f�B�^�ł�Assets�Ɠ����K�w�ɕۑ����A����ȊO�ł�Application.persistentDataPath�ȉ��ɕۑ�����悤�ɁB
#if UNITY_EDITOR
        filePath += ".json";
#else
    filePath = Application.persistentDataPath + "/" + filePath;
#endif

        return filePath;
    }

}