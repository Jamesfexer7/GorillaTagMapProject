﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEngine.UI;
using VmodMonkeMapLoader.Behaviours;

public class CompileMapWindow : EditorWindow
{

    private MapDescriptor[] notes;
    [MenuItem("Window/Map Exporter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CompileMapWindow), false, "Map Exporter", false);
    }
    public Vector2 scrollPosition = Vector2.zero;

    private void OnFocus()
    {
        notes = GameObject.FindObjectsOfType<MapDescriptor>();
    }
    void OnGUI()
    {
        var window = EditorWindow.GetWindow(typeof(CompileMapWindow), false, "Map Exporter", false);

        int ScrollSpace = (16 + 20) + (16 + 17 + 17 + 20 + 20);
        foreach (MapDescriptor note in notes)
        {
            if (note != null)
            {

                ScrollSpace += (16 + 17 + 17 + 20 + 20);

            }
        }
        float currentWindowWidth = EditorGUIUtility.currentViewWidth;
        float windowWidthIncludingScrollbar = currentWindowWidth;
        if (window.position.size.y >= ScrollSpace)
        {
            windowWidthIncludingScrollbar += 30;
        }
        scrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, window.position.size.y), scrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - 20, ScrollSpace), false, false);

        //GUILayout.ScrollViewScope
        GUILayout.Label("Notes", EditorStyles.boldLabel, GUILayout.Height(16));
        GUILayout.Space(20);

        foreach (MapDescriptor note in notes)
        {
            if (note != null)
            {
                GUILayout.Label("GameObject : " + note.gameObject.name, EditorStyles.boldLabel, GUILayout.Height(16));
                note.AuthorName = EditorGUILayout.TextField("Author name", note.AuthorName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));
                note.MapName = EditorGUILayout.TextField("Map name", note.MapName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));

                if (GUILayout.Button("Export " + note.MapName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(20)))
                {
                    GameObject noteObject = note.gameObject;
                    if (noteObject != null && note != null)
                    {
                        string path = EditorUtility.SaveFilePanel("Save map file", "", note.MapName + ".gtmap", "gtmap");
                        Debug.Log(path == "");

                        if (path != "")
                        {
                            EditorUtility.SetDirty(note);

                            if(noteObject.transform.Find("ThumbnailCamera") != null)
                            {
                                noteObject = Instantiate(noteObject);
                                // do stuff 
                                try
                                {
                                    ExporterUtils.ExportPackage(noteObject, path, "Map", ExporterUtils.MapDescriptorToJSON(note));
                                    EditorUtility.DisplayDialog("Exportation Successful!", "Exportation Successful!", "OK");
                                }
                                catch(System.Exception e)
                                {
                                    EditorUtility.DisplayDialog("Error!", e.Message, "OK");
                                    DestroyImmediate(noteObject);
                                }
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("Exportation Failed!", "No thumbnail camera.", "OK");
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Exportation Failed!", "Path is invalid.", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Exportation Failed!", "Map GameObject is missing.", "OK");
                    }
                }
                GUILayout.Space(20);
            }
        }
        GUI.EndScrollView();
    }

}