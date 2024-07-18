using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace InventoryDiablo
{
   
    [CustomEditor(typeof(InventoryUI))] /* [CanEditMultipleObjects] */
    public class SetCharacterEditor : Editor
    {
        const int defaultSpace = 8;
        public bool _showBckgrounds = false;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            InventoryUI Inventory = (InventoryUI)target;

            EditorGUILayout.BeginHorizontal("Box");
            
            _showBckgrounds = EditorGUILayout.Foldout(_showBckgrounds, "Одежда", true);

            EditorGUILayout.EndHorizontal();


            if(_showBckgrounds)
            {
                EditorGUILayout.BeginVertical("Box");
                
                if (GUILayout.Button("Пересоздать слоты"))
                {
                    if(Inventory.Clothes != null && Inventory.Clothes.characterRoot != null)
                    {
                        for(int n = 0; n < Inventory.Clothes.items.Count; n++)
                        {
                            List<GameObject> removedObjs = Inventory.Clothes.GetRemoveObjList(n);
                                        
                            for(int m = 0; m < removedObjs.Count; m ++)
                            {
                                if(removedObjs[m] != null)
                                {
                                    DestroyImmediate(removedObjs[m]);
                                }
                            }
                        }

                        Inventory.Clothes.items.Clear();
                    }
                    
                    CreateCloches(Inventory);
                }
                EditorGUILayout.EndVertical();
             
                DrawDetails(Inventory);
            }
        }

        private void DrawDetails(InventoryUI Inventory)
        {
            

            if (Inventory.Clothes != null && Inventory.Clothes.items != null && Inventory.Clothes.items.Count > 0)
            {
                // GUILayout.Space(defaultSpace);

                
                // EditorGUILayout.BeginVertical("Box");
                for(int n = 0; n < Inventory.Clothes.items.Count; n++)
                {
                    EditorGUILayout.BeginHorizontal("Box");

                    GUILayout.Label(Inventory.Clothes.items[n].ItemType.ToString(), GUILayout.Width(100));
                    
                    if (!Inventory.Clothes.HasItem(n))
                    {
                        if (GUILayout.Button("Надеть"))
                        {
                            Inventory.Clothes.AddItem(n);
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Снять"))
                        {
                            Inventory.Clothes.RemoveItem(n);
                            // List<GameObject> removedObjs = chest.Clothes.GetRemoveObjList(n);
                            
                            // for(int m = 0; m < removedObjs.Count; m ++)
                            // {
                            //     if(removedObjs[m] != null)
                            //     {
                            //         DestroyImmediate(removedObjs[m]);
                            //     }
                            // }
                        }
                    }

                    if(Inventory.Clothes.items[n] != null)
                    {
                        Inventory.Clothes.items[n].Prefab = 
                        EditorGUILayout.ObjectField(Inventory.Clothes.items[n].Prefab, typeof(ItemOnstreet), true) as ItemOnstreet;
                    }
                    
                    // if (GUILayout.Button("X"))
                    // {
                    //     chest.clothes.items.RemoveAt(n);

                    //     break;
                    // }
                    
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space(defaultSpace);
            }
        }

        private void CreateCloches(InventoryUI chest)
        {
            chest.Clothes = new SetCharacter(chest.transform, chest.transform.GetComponent<Animator>());

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Шлем, false, HumanBodyBones.Head));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Броня, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Ремень, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Штаны, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Сапоги, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Оружие, false, HumanBodyBones.RightHand));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Щит, false, HumanBodyBones.LeftHand));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Кольцо, false, HumanBodyBones.RightHand));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Кольцо, false, HumanBodyBones.LeftHand));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Наплечники, true));

            chest.Clothes.items.Add(new SetCharacter.Item(ItemData.ItemType.Ожерелье, false, HumanBodyBones.Neck));
        }
    }
}


