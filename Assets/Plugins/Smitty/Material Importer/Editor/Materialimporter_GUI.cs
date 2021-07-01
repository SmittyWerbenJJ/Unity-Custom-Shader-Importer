using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEngine.Rendering;
using UnityEditor.AnimatedValues;


namespace Smitty.MaterialImporter
{
    public class Materialimporter_GUI : EditorWindow
    {

        private MaterialImporter_Controller Controller;

        public Materialimporter material_Importer;

        //Conditions
        public bool canExportMaterials;
        bool doExportMaterials;
        bool isExportingMaterials;

        // Foldouts
        bool foldoutGUIParsing;
        bool foldoutGUI_Shaders;

        bool foldoutGUI_Materials;
        //UI Stuff
        Vector2 scrollposition = new Vector2();

        private void OnEnable()
        {
            Controller = new MaterialImporter_Controller();
            material_Importer = new Materialimporter();
        }

        [MenuItem("Smitty/Json Importer")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(Materialimporter_GUI));
        }



        void OnGUI()
        {




            scrollposition = EditorGUILayout.BeginScrollView(scrollposition);
            GUI_Conditions();

            // Title
            GUI_Title();
            // Display the GUI Elements - Each Element is in a Foldout 
            //Show the Parsing Panel - assign json file etc
            GUI_Parsing();
            //Show the Shader Assignment Panel
            GUI_Shaders();
            //Button to Decompile the Materials to UNity Materials  
            GUI_Materials();

            Controller.Update();

            EditorGUILayout.EndScrollView();



        }
        #region GUI Methods

        public void GUI_Conditions()
        {


            Controller.checkParsingState();
            // //Can Export Materials
            // //Detect if the materialImporter has null shaders in its list
            // shadersAssigned = true;
            // foreach (var shader in material_Importer.shaders)
            // {
            //     shadersAssigned = shader == null ? false : shadersAssigned;
            // }



            // //EXporting Materials
            // //Exports the Materials with the corresponding shaders

            // if (
            //     material_Importer.parsingState == ParsingStates.parsingSuccessful
            //     && material_Importer.textureFolder != null
            //     && material_Importer.materialFolder != null
            //     )
            // {
            //     canExportMaterials = true;

            //     //start exporting materials once the The export button is pressed.               
            //     if (doExportMaterials)
            //     {
            //         doExportMaterials = false; //!We need to set this back to false, otherwise it will run indefinetly
            //         List<UnityEngine.Material> materials = new List<UnityEngine.Material>();

            //         //Export all the materials
            //         foreach (var material in material_Importer.parsedJson.materials)
            //         {
            //             var shader = material_Importer.ShaderDict[material.type];
            //             materialDescription materialDescription = new materialDescription().init(
            //                 name: material.name,
            //                 shader: shader,
            //                 material.maps);

            //             materials.Add(material_Importer.createMaterial(materialDescription));

            //         }

            //         //Save Materials
            //         string targetPath = AssetDatabase.GetAssetPath(material_Importer.materialFolder) + "/";
            //         foreach (var newmaterial in materials)
            //         {
            //             material_Importer.saveMaterial(newmaterial, targetPath);
            //         }
            //     }
            // }
            // else
            // {
            //     canExportMaterials = false;
            // }
        }

        void GUI_Parsing()
        {
            string _parsingMessage = " error ";
            MessageType _parsingMessageType = MessageType.None;
            string _label = "Json File: ";
            string _labelReload = "Load File";

            bool _canParse = Controller.canParse();

            foldoutGUIParsing = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutGUIParsing, "Load File");
            //Foldout of Parsing Section        
            if (foldoutGUIParsing)
            {

                // The Parsing Field
                showParsingField(_label);

                //The Parsing messages, dependent on parsing state
                switch (Controller.GetParsingState())
                {
                    case ParsingStates.noFile:
                        _parsingMessage = "Select a File to begin ...";
                        _parsingMessageType = MessageType.Warning;
                        break;

                    case ParsingStates.waitForUser:
                        _parsingMessage = " Press the Button to Read the FileFile Can be Parsed ...";
                        _parsingMessageType = MessageType.Info;
                        break;

                    case ParsingStates.parsingFailed:
                        _parsingMessage = $"No Valid FIle";
                        _parsingMessageType = MessageType.Warning;
                        break;

                    case ParsingStates.parsingSuccessful:
                        //* prepares the Display of a Messagebox with: Elements of Parsed Asset                                                                    

                        var parsingInfo = Controller.getParsingInformation();
                        _parsingMessage =
                        $"Objects: {parsingInfo.Objects.Count}" + '\t' +
                        $"Materials: {parsingInfo.Materials.Count}" + '\n' +
                        $"Shaders: {parsingInfo.Shaders.Count}" + '\t' +
                        $"\nTextures: {parsingInfo.Textures.Count}";
                        _parsingMessageType = MessageType.Info;
                        break;
                    default:
                        _parsingMessage = "something went wrong";
                        _parsingMessageType = MessageType.Error;
                        break;
                }

                //Show the parsing Message Help Box                
                showParsingMessage(_parsingMessage, _parsingMessageType);

                //Disable the Button when not Ready to Parse
                showParsingButton(_labelReload, !_canParse);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            void showParsingMessage(string message, MessageType type)
            {
                EditorGUILayout.BeginHorizontal();
                GUI_HelpBox(message, type);
                EditorGUILayout.EndHorizontal();
            }

            void showParsingButton(string label, bool disabled)
            {
                EditorGUI.BeginDisabledGroup(disabled);
                if (GUILayout.Button(label)) // The Button to Kick off Parsing                
                    Controller.ScheduleParsing(); //tell the material importer to schedule the parsing                
                EditorGUI.EndDisabledGroup();
            }

            void showParsingField(string description)
            {
                EditorGUILayout.LabelField(description);
                EditorGUILayout.BeginHorizontal();

                TextAsset file = Controller.getJsonFile();
                file = (TextAsset)EditorGUILayout.ObjectField(file, typeof(TextAsset), false);
                Controller.setJsonFile(file);

                EditorGUILayout.EndHorizontal();
            }
        }


        void GUI_Shaders()
        {
            //THe Foldout for the Shader Section
            foldoutGUI_Shaders = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutGUI_Shaders, "Shaders");
            bool _displayReady = Controller.GetParsingState() == ParsingStates.parsingSuccessful;

            if (foldoutGUI_Shaders)
            {
                // Display ObjectFields for all Shaders

                if (_displayReady)
                    showObjectFields();

                else
                    showMessage("Parse The JSON File First!", MessageType.Warning);

            }


            void showObjectFields()
            {
                var _shaderDict = Controller.getParsingInformation().Shaders;

                //Show the Object Fields for Assinging Shaders
                foreach (var key in new List<string>(_shaderDict.Keys))
                {

                    _shaderDict[key] = (UnityEngine.Shader)EditorGUILayout.ObjectField(
                                                 label: key,
                                                 obj: _shaderDict[key],
                                                 objType: typeof(UnityEngine.Shader),
                                                 allowSceneObjects: false
                                                 );
                }
            }


            void showMessage(string message, MessageType messageType)
            {
                EditorGUILayout.BeginHorizontal();
                GUI_HelpBox(message, messageType);
                EditorGUILayout.EndHorizontal();
            }
            // // Kick off Listing of Shaders if there are any.
            // // Show warning when the Materialimporter has found no Shaders
            // var _shaderNames = material_Importer.getShaderNames();
            // if (_shaderNames.Count >= 1)
            // {
            //     //Display dem shaders - make a objectField to assign them and store den in a List               

            //     //  initialize a Shader list of length [AmountOfShaders] with 'Default NullShader'
            //     for (int i = 0; i < _shaderNames.Count; i++)
            //     {
            //         // If the slot is null - make space for a custom shader by adding a nullshader. this will prevent OOR exception

            //         //Create all Slots
            //         if (material_Importer.shaders.Count < _shaderNames.Count)
            //         {
            //             foreach (var item in _shaderNames)
            //             {
            //                 material_Importer.shaders.Add(material_Importer.nullShader);
            //             }
            //         }

            //         //Show the Object Fields 
            //         material_Importer.shaders[i] = (UnityEngine.Shader)EditorGUILayout.ObjectField(
            //             _shaderNames[i],
            //             material_Importer.shaders[i],
            //             typeof(UnityEngine.Shader),
            //             false);
            //     }
            // }
            // else
            // {
            //     GUI_HelpBox("Shaders not Ready!\n Complete the Steps above, first!", MessageType.Error);
            // }




            EditorGUILayout.EndFoldoutHeaderGroup();
        }
        bool GUI_Materials()
        {
            //Show Material Creation and Texture Folder Assignment
            foldoutGUI_Materials = EditorGUILayout.BeginFoldoutHeaderGroup(foldoutGUI_Materials, "Materials");
            if (foldoutGUI_Materials)
            {
                /*
             Ready to export:
             show a message box with the amount of materials that will be created

             Not Ready (else):
             show a messageBox with a warning
             */
                var _parsingSuccessful = Controller.GetParsingState() == ParsingStates.parsingSuccessful;
                var _ShadersAssigned = Controller.isShadersAssigned();
                var _foldersAssigned = false;

                //Texture folder and Output Folder
                _foldersAssigned = showTextureAndMaterialAssignment(); //The Object Fields for Textures Folder and output Folder


                var isReadyToExport = _parsingSuccessful && _ShadersAssigned && _foldersAssigned;
                //Not Ready message
                if (!isReadyToExport)
                {
                    //Display a Help Box with the Error Message                                                
                    GUI_HelpBox("Finish Shader & Texture Assignment!", MessageType.Warning);
                }

                //Show the export Button - is greyed out if not ready
                showMaterialCreationButton(isReadyToExport);

            }
            EditorGUILayout.EndFoldoutHeaderGroup();


            /*
            *local methods*
            */
            bool showTextureAndMaterialAssignment()
            {
                //The Object Fields for Textures Folder and output Folder
                EditorGUILayout.BeginVertical();
                material_Importer.textureFolder = (DefaultAsset)EditorGUILayout.ObjectField("Select Textures Folder", material_Importer.textureFolder, typeof(DefaultAsset), false);
                material_Importer.materialFolder = (DefaultAsset)EditorGUILayout.ObjectField("Select Material-Output Folder", material_Importer.materialFolder, typeof(DefaultAsset), false);

                EditorGUILayout.EndVertical();
                return material_Importer.textureFolder != null && material_Importer.materialFolder != null;

            }

            void showMaterialCreationButton(bool enabled)
            {
                EditorGUILayout.BeginHorizontal();

                //Disable the Button inf not yet Ready
                EditorGUI.BeginDisabledGroup(!enabled);

                //The BUtton that will kick off material exporting
                doExportMaterials = GUILayout.Button("Export Materials!");
                if (doExportMaterials)
                    Controller.exportMaterials();

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }

            return true;
        }


        void GUI_HelpBox(string message, MessageType messageType)
        {
            /// Displays a Generic Message Box 
            EditorGUILayout.HelpBox(message, messageType);
        }


        void GUI_Title()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("Material Importer v0.1", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        #endregion
    }
}
