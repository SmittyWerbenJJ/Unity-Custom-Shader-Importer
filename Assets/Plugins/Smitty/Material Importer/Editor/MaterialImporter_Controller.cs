using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Smitty.MaterialImporter
{
    public class MaterialImporter_Controller : IMaterialImporter
    {
        Materialimporter material_importer;

        bool isParsingScheduled;


        public MaterialImporter_Controller()
        {
            this.material_importer = new Materialimporter();

            this.isParsingScheduled = false;


        }
        public void Update()
        {
            if (isParsingScheduled)
            {
                ParseJson();
                isParsingScheduled = false;
            }
        }

        public void ScheduleParsing()
        {
            //Todo write Parsing schedule logic
            isParsingScheduled = true;
        }

        public bool canParse()
        {

            bool canParse = material_importer.jsonFile != null;
            //Controller.getParsingState() == ParsingStates.parsingSuccessful) material_Importer.parsingState == ParsingStates.parsingSuccessful)//Parsing Message inside of Parsing Foldout
            return canParse;
        }

        public TextAsset getJsonFile()
        {
            return material_importer.jsonFile;
        }

        public void setJsonFile(TextAsset newJsonFile)
        {
            //Compare old File with new file and kick off resetting of the importer
            // https://stackoverflow.com/questions/5401527/how-to-detect-if-a-variable-has-changed //If the current file is the old file,cancel
            if (newJsonFile != getJsonFile())
            {
                resetParsing();
                material_importer.jsonFile = newJsonFile;
            }
        }
        void ParseJson()
        {
            material_importer.parseJson(getJsonFile());
        }

        public ParsingInformation getParsingInformation()
        {
            return material_importer.parsingInformation;
        }

        public void resetParsing()
        {
            material_importer.isParsed = false;
            material_importer.parsedJson = null;
        }

        public bool isShadersAssigned()
        {
            //return if all shaders has been assigned
            bool bReturn = true;
            var _shaderList = new List<Shader>(getParsingInformation().Shaders.Values);
            foreach (var shader in _shaderList)
            {
                if (shader == null)
                {
                    bReturn = false;
                    break;
                }
            }
            return bReturn;
        }

        public void checkParsingState()
        {
            var currentState = material_importer.parsingState;
            var _isFilepresent = getJsonFile() != null;
            var _isParsed = material_importer.isParsed;
            var _isFileValid = material_importer.parsedJson != null;

            // state= no File
            if (!_isFilepresent)
            {
                currentState = ParsingStates.noFile;
            }
            else
            // states: wait for user / successful / failed
            {
                if (!_isParsed)
                    //state - wait for user
                    currentState = ParsingStates.waitForUser;

                //state - successful
                else if (_isFileValid)
                    currentState = ParsingStates.parsingSuccessful;

                //state - Failed
                else
                    currentState = ParsingStates.parsingFailed;
            }
            material_importer.parsingState = currentState;

        }



        public ParsingStates GetParsingState()
        {
            //Calculate the Parsing State
            checkParsingState();
            return material_importer.parsingState;
        }

        public void SetParsingStates(ParsingStates target)
        {
            material_importer.parsingState = target;
        }

        public void exportMaterials()
        {
            material_importer.exportMaterials();
        }
    }

}








public class oldCOde
{

    #region oldCode

    // public enum ImporterStages
    // {
    //     parsing,
    //     assigningShaders,
    //     assigingTextures,
    //     creatingMaterials,
    //     exporting
    // }

    //     //stages
    //     public ImporterStages importerStage;
    //     // File Stuff
    //     public TextAsset jsonFile;

    //     //Parsing Stuff
    //     public bool jsonReadytoParse;
    //     public bool isjsonParsed;
    //     public ParsingStates parsingState;
    //     public bool btnParseJsonPressed;

    //     public MessageType parsingMessageType;

    //     // Material Stuff
    //     public int activeObjIndex;
    //     public bool completedParsing;
    //     public bool isReady_MaterialsConverted;
    //     public bool isReady_MaterialCreationReady;
    //     public bool isReady_stepTextureFolderSelected;
    //     public bool isGUIReady_Shaders;
    //     public bool foldoutGUI_Shaders;
    //     public DefaultAsset textureFolder;
    //     public List<string> shaderNames;
    //     public List<Shader> shaders;

    //     // strings
    //     public string parsingMessage;
    //     public List<string> randomOptions;// = new List<string> { "--- None ---" };
    //     public int iselectedObject;
    //     internal bool bTextureFolderSelected;
    //     internal bool stepShadersCreated;

    //     internal bool stepShadersAssigned;
    //     internal bool foldoutShaders;
    //     internal bool foldoutGUIParsing;
    //     internal bool foldoutGUI_Materials;

    // #region variables

    // public materialIMporterGUI importerGUI = new materialIMporterGUI();

    // #endregion

    // #region GUI



    #endregion

    #region old Code
    //     //Stages - Do the actiual Logic
    //     switch (importerGUI.importerStage)
    //     {
    //         case ImporterStages.parsing:
    //             if (importerGUI.jsonFile != null && !importerGUI.isjsonParsed)
    //             {
    //                 material_Importer.runParsing();
    //             }
    //             break;
    //         case ImporterStages.assigningShaders:

    //             material_Importer.runShaderAssignment();
    //             break;
    //         case ImporterStages.creatingMaterials:
    //             material_Importer.runMaterialCreation();
    //             break;
    //         case ImporterStages.assigingTextures:
    //             break;
    //         case ImporterStages.exporting:

    //             break;
    //     }


    //     //Update the Stage
    //     if (importerGUI.isjsonParsed)
    //     {
    //         importerGUI.importerStage = ImporterStages.assigningShaders;
    //     }
    //     else if (importerGUI.stepShadersCreated) { }
    //     else if (false)
    //     {

    //     }

    //     /*
    //     load the file
    //     parse the file
    //     show errors / materials
    //     show materials
    //     show textures
    //     convert materials
    //     */



    //     // //Show Button to Create UNity Materials
    //     // stepMaterialCreationReady = true;
    //     // foreach (var shader in shaders)
    //     // {
    //     //     if (shader == null)
    //     //     {
    //     //         stepMaterialCreationReady = false;
    //     //     }
    //     // }

    //     // EditorGUILayout.BeginHorizontal();
    //     // EditorGUI.BeginDisabledGroup(!stepMaterialCreationReady);
    //     // if (GUILayout.Button("[3] Create Materials"))
    //     // {
    //     //     //         //make Material Folder path string
    //     //     //         var textureFolderPath = AssetDatabase.GetAssetPath(textureFolder);
    //     //     //         string basepath = textureFolderPath.Substring(0, textureFolderPath.LastIndexOf("/"));
    //     //     //         string materialFolderPath = $"{basepath}/Materials";

    //     //     //         //create Material Folder, if not already existing
    //     //     //         if (!AssetDatabase.IsValidFolder(materialFolderPath))
    //     //     //         {
    //     //     //             AssetDatabase.CreateFolder(basepath, "Materials");
    //     //     //         }

    //     //     //         material_Importer.createMaterials_unity(materialFolderPath, shaders);
    //     //     //     }
    //     // }
    //     // EditorGUI.EndDisabledGroup();
    //     // EditorGUILayout.EndHorizontal();


    #endregion

    #region  OLD CODE

    // /// <summary>
    // /// Displays a INfobox Showing the current Status of the Parsing
    // /// </summary>
    // /// <param name="jsonFile"></param>
    // void GUI_HelpBox_JsonReady()
    // {
    //     // EditorGUILayout.BeginHorizontal();
    //     // string message = "";
    //     // MessageType messagetype = MessageType.None;
    //     // string parsingMessage = "";
    //     // if (jsonParsed == true)
    //     // {


    //     //     message = "Json successfully parsed!";
    //     //     messagetype = MessageType.Info;

    //     //     parsingMessage = $"\nObjects: {material_Importer.parsedJson.objects.Count}";
    //     //     objectMaterialCount_text += $"\nMaterials: {material_Importer.parsedJson.materials.Count}";
    //     // }
    //     // else
    //     // {
    //     //     message = "Invalid Json File!";
    //     //     messagetype = MessageType.Warning;
    //     // }

    //     // EditorGUILayout.HelpBox(message, messagetype);
    //     // EditorGUILayout.HelpBox(objectMaterialCount_text, MessageType.None);
    //     // EditorGUILayout.EndHorizontal();
    // }
    // /// <summary>
    // /// Displays a Seperator
    // /// </summary>
    // static void GUI_Seperator()
    // {
    //     EditorGUILayout.BeginHorizontal();
    //     EditorGUILayout.Separator();
    //     EditorGUILayout.EndHorizontal();
    // }




    // /// <summary>
    // /// Displays a Dropdown Button with 10 random Object names of the Json File
    // /// </summary>
    // /// <returns></returns>
    // string GUI_DropDown_10RandomObjects(bool Reload)
    // {
    //     //First Init of the List 
    //     if (importerGUI.randomOptions == null)
    //     {
    //         importerGUI.randomOptions = new List<string>() { " --- None --- " };
    //         Reload = true;
    //     }

    //     if (Reload)
    //     {

    //         importerGUI.randomOptions = new List<string>() { " --- None --- " };

    //         List<string> objects = material_Importer.parsedJson.objects;

    //         int objectCount = material_Importer.parsedJson.objects.Count;

    //         int newRange;
    //         if (objectCount >= 10)
    //         {
    //             newRange = 10;
    //         }
    //         else
    //         {
    //             newRange = objectCount;
    //         }

    //         while (newRange > 0)
    //         {
    //             int randomNumber = Random.Range(0, objectCount);
    //             string randomObjectName = material_Importer.parsedJson.objects[randomNumber];
    //             importerGUI.randomOptions.Add(randomObjectName);
    //             newRange--;
    //         }
    //     }

    //     EditorGUILayout.BeginHorizontal();
    //     importerGUI.activeObjIndex = EditorGUILayout.Popup(
    //         label: "Objects",
    //         selectedIndex: importerGUI.activeObjIndex,
    //         displayedOptions: importerGUI.randomOptions.ToArray());
    //     EditorGUILayout.EndHorizontal();

    //     return importerGUI.randomOptions[importerGUI.activeObjIndex];
    // }


    // #endregion




    // bool GUI_Button(string text)
    // {
    //     return GUILayout.Button(text);
    // }
    // void GUI_Reset()
    // {
    //     EditorGUILayout.Space(50f);
    //     EditorGUILayout.BeginHorizontal();
    //     if (GUILayout.Button("Reset Importer"))
    //     {
    //         importerGUI.parsingState = ParsingStates.noFile;
    //         importerGUI.jsonFile = null;
    //         importerGUI.stepShadersCreated = false;
    //         importerGUI.textureFolder = null;

    //     }
    //     EditorGUILayout.EndHorizontal();

    // }


    // void GUI_Preview()
    // {
    //     if (importerGUI.parsingState == ParsingStates.parsingSuccessful)
    //     {
    //         //Show 10 Random materials
    //         EditorGUILayout.BeginHorizontal();
    //         var reset = GUI_Button("Reload 10 Random Materials");
    //         EditorGUILayout.EndHorizontal();

    //         EditorGUILayout.BeginHorizontal();
    //         GUI_DropDown_10RandomObjects(reset);
    //         EditorGUILayout.EndHorizontal();

    //     }
    // }
    #endregion
}