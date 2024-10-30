using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
namespace StateMachine
{
    public class SFMWindow : EditorWindow
    {
        private enum ToolMode { CreateStateMachine, AddStates }
        private ToolMode currentMode = ToolMode.CreateStateMachine;

        // For Create State Machine
        private string stateMachineName = "";
        private List<string> stateNames = new List<string>();
        private Vector2 scrollPos;

        // For Add States
        private MonoScript existingBaseStateScript;
        private string existingBaseStateName = "";
        private List<string> newStateNames = new List<string>();
        private Vector2 addStatesScrollPos;

        [MenuItem("SFM/State Machine Tools")]
        public static void ShowWindow()
        {
            GetWindow<SFMWindow>("SFM Tools");
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            currentMode = (ToolMode)GUILayout.Toolbar((int)currentMode, new string[] { "Create State Machine", "Add States" });

            GUILayout.Space(10);

            switch (currentMode)
            {
                case ToolMode.CreateStateMachine:
                    DrawCreateStateMachineGUI();
                    break;
                case ToolMode.AddStates:
                    DrawAddStatesGUI();
                    break;
            }
        }

        private void DrawCreateStateMachineGUI()
        {
            GUILayout.Label("State Machine Generator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("State Machine Name:", GUILayout.Width(150));
            stateMachineName = GUILayout.TextField(stateMachineName);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.Label("States:", EditorStyles.boldLabel);

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
            for (int i = 0; i < stateNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                stateNames[i] = GUILayout.TextField(stateNames[i]);

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    stateNames.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            if (GUILayout.Button("Add State"))
            {
                stateNames.Add("");
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Generate State Machine"))
            {
                if (string.IsNullOrEmpty(stateMachineName))
                {
                    EditorUtility.DisplayDialog("Error", "Please enter a state machine name.", "OK");
                }
                else
                {
                    GenerateStateMachineScripts();
                }
            }
        }

        private void DrawAddStatesGUI()
        {
            GUILayout.Label("Add States to Existing SFM", EditorStyles.boldLabel);
            GUILayout.Space(10);

            // Existing Base State Script
            GUILayout.BeginHorizontal();
            GUILayout.Label("Select Base State Script:", GUILayout.Width(150));
            existingBaseStateScript = (MonoScript)EditorGUILayout.ObjectField(existingBaseStateScript, typeof(MonoScript), false);
            GUILayout.EndHorizontal();

            if (existingBaseStateScript != null)
            {
                existingBaseStateName = existingBaseStateScript.name;
            }
            else
            {
                existingBaseStateName = "";
            }

            GUILayout.Space(10);

            GUILayout.Label("New States:", EditorStyles.boldLabel);

            addStatesScrollPos = GUILayout.BeginScrollView(addStatesScrollPos, GUILayout.Height(150));
            for (int i = 0; i < newStateNames.Count; i++)
            {
                GUILayout.BeginHorizontal();
                newStateNames[i] = GUILayout.TextField(newStateNames[i]);

                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    newStateNames.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            if (GUILayout.Button("Add State"))
            {
                newStateNames.Add("");
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Add States"))
            {
                if (existingBaseStateScript == null)
                {
                    EditorUtility.DisplayDialog("Error", "Please select a base state script.", "OK");
                }
                else
                {
                    AddStatesToExistingSFM();
                }
            }
        }

        private void GenerateStateMachineScripts()
        {
            string path = EditorUtility.SaveFolderPanel("Select Folder to Save Scripts", "Assets", "");
            if (string.IsNullOrEmpty(path))
                return;

            path = FileUtil.GetProjectRelativePath(path);
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Error", "Please select a folder within the Assets directory.", "OK");
                return;
            }

            stateMachineName = char.ToUpper(stateMachineName[0]) + stateMachineName.Substring(1);

            string baseStateName = $"{stateMachineName}BaseState";

            string baseStateScript = $@"using UnityEngine;
using StateMachine;

public abstract class {baseStateName} : BaseState
{{
}}";
            File.WriteAllText(Path.Combine(path, $"{baseStateName}.cs"), baseStateScript);

            string stateMachineScript = $@"using UnityEngine;
using StateMachine;

public class {stateMachineName}StateMachine : BaseStateMachine
{{
    public void SwitchState<T>(T state) where T : {baseStateName}
    {{
        if (state is {baseStateName})
        {{
            base.SwitchState(state);
        }}
        else
        {{
            Debug.LogError(""State must be of type {baseStateName}"");
        }}
    }}

}}";
            File.WriteAllText(Path.Combine(path, $"{stateMachineName}StateMachine.cs"), stateMachineScript);

            // Generate State Scripts
            foreach (string stateName in stateNames)
            {
                if (!string.IsNullOrEmpty(stateName))
                {
                    // Combine state name and state machine name for the class name
                    string fullStateName = $"{stateName}{stateMachineName}" + "State";

                    string stateScript = $@"using UnityEngine;
using StateMachine;

public class {fullStateName} : {baseStateName}
{{
    public override void OnEnter()
    {{
        base.OnEnter();
    }}

    public override void OnUpdate()
    {{
        base.OnUpdate();
    }}

    public override void OnExit()
    {{
        base.OnExit();
    }}

    protected override void OnDestroy()
    {{
        base.OnDestroy();
    }}
}}";
                    File.WriteAllText(Path.Combine(path, $"{fullStateName}.cs"), stateScript);
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "State Machine scripts generated successfully.", "OK");
        }

        private void AddStatesToExistingSFM()
        {
            string path = EditorUtility.SaveFolderPanel("Select Folder to Save Scripts", "Assets", "");
            if (string.IsNullOrEmpty(path))
                return;

            path = FileUtil.GetProjectRelativePath(path);
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Error", "Please select a folder within the Assets directory.", "OK");
                return;
            }

            string baseStateName = existingBaseStateName;

            string stateMachineNameFromBase = baseStateName.Replace("BaseState", "");

            foreach (string stateName in newStateNames)
            {
                if (!string.IsNullOrEmpty(stateName))
                {
                    string fullStateName = $"{stateName}{stateMachineNameFromBase}" + "State";

                    string stateScript = $@"using UnityEngine;
using StateMachine;

public class {fullStateName} : {baseStateName}
{{
    public override void OnEnter()
    {{
        base.OnEnter();
    }}

    public override void OnUpdate()
    {{
        base.OnUpdate();
    }}

    public override void OnExit()
    {{
        base.OnExit();
    }}

    protected override void OnDestroy()
    {{
        base.OnDestroy();
    }}
}}";
                    File.WriteAllText(Path.Combine(path, $"{fullStateName}.cs"), stateScript);
                }
            }

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "States added successfully.", "OK");
        }
    }
}
#endif