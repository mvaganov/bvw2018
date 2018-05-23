#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class MassExportSubstance : EditorWindow
{

	private int size;
	private SubstanceArchive[] substances;

	[MenuItem("Window/Mass Export Bitmaps")]
	static void Init()
	{
		EditorWindow.GetWindow(typeof(MassExportSubstance));
	}

	void OnGUI()
	{
		if (size == 0)
		{
			size = 1;
		}

		size = EditorGUILayout.IntField("Substances", size);

		if (substances == null)
		{
			substances = new SubstanceArchive[size];
		}

		if (substances.Length != size)
		{
			SubstanceArchive[] cache = substances;

			substances = new SubstanceArchive[size];

			for (int i = 0; i < (cache.Length > size ? size : cache.Length); i++)
			{
				substances[i] = cache[i];
			}
		}

		for (int i = 0; i < size; i++)
		{
			substances[i] = EditorGUILayout.ObjectField("Substance " + (i + 1), substances[i], typeof(SubstanceArchive), false) as SubstanceArchive;
		}

		if (GUILayout.Button("Extract"))
		{
			for (int subs = 0; subs < size; subs++)
			{
				SubstanceArchive substance = substances[subs];

				if (substance == null)
				{
					continue;
				}

				string substancePath = AssetDatabase.GetAssetPath(substance.GetInstanceID());
				SubstanceImporter substanceImporter = AssetImporter.GetAtPath(substancePath) as SubstanceImporter;
				int substanceMaterialCount = substanceImporter.GetMaterialCount();
				#pragma warning disable 0618
				// this script explicitly exists for migrating substance materials
				ProceduralMaterial[] substanceMaterials = substanceImporter.GetMaterials();
				#pragma warning restore 0618
				if (substanceMaterialCount <= 0)
				{
					continue;
				}

				string basePath = substancePath.Replace("/" + substance.name + ".sbsar", "");

				if (!Directory.Exists(basePath + "/" + substance.name))
				{
					AssetDatabase.CreateFolder(basePath, substance.name);

					AssetDatabase.ImportAsset(basePath + "/" + substance.name);
				}

				if (!Directory.Exists("EXPORT_HERE"))
				{
					Directory.CreateDirectory("EXPORT_HERE");
				}

				System.Type substanceImporterType = typeof(SubstanceImporter);
				MethodInfo exportBitmaps = substanceImporterType.GetMethod("ExportBitmaps", BindingFlags.Instance | BindingFlags.Public);

				#pragma warning disable 0618
				// this script explicitly exists for migrating substance materials
				foreach (ProceduralMaterial substanceMaterial in substanceMaterials)
				#pragma warning restore 0618
				{
					bool generateAllOutputs = substanceImporter.GetGenerateAllOutputs(substanceMaterial);

					if (!Directory.Exists(basePath + "/" + substance.name + "/" + substanceMaterial.name))
					{
						AssetDatabase.CreateFolder(basePath + "/" + substance.name, substanceMaterial.name);

						AssetDatabase.ImportAsset(basePath + "/" + substance.name + "/" + substanceMaterial.name);
					}

					string materialPath = basePath + "/" + substance.name + "/" + substanceMaterial.name + "/";
					Material newMaterial = new Material(substanceMaterial.shader);

					newMaterial.CopyPropertiesFromMaterial(substanceMaterial);

					AssetDatabase.CreateAsset(newMaterial, materialPath + substanceMaterial.name + ".mat");

					AssetDatabase.ImportAsset(materialPath + substanceMaterial.name + ".mat");

					substanceImporter.SetGenerateAllOutputs(substanceMaterial, true);

					exportBitmaps.Invoke(substanceImporter, new object[] { substanceMaterial, materialPath,true });

					if (!generateAllOutputs)
					{
						substanceImporter.SetGenerateAllOutputs(substanceMaterial, false);
					}

					string[] exportedTextures = Directory.GetFiles("EXPORT_HERE");

					if (exportedTextures.Length > 0) foreach (string exportedTexture in exportedTextures)
					{
						File.Move(exportedTexture, materialPath + exportedTexture.Replace("EXPORT_HERE", ""));
					}

					AssetDatabase.Refresh();

					int propertyCount = ShaderUtil.GetPropertyCount(newMaterial.shader);
					Texture[] materialTextures = substanceMaterial.GetGeneratedTextures();

					if ((materialTextures.Length <= 0) || (propertyCount <= 0))
					{
						continue;
					}

					#pragma warning disable 0618
					// this script explicitly exists for migrating substance materials
					foreach (ProceduralTexture materialTexture in materialTextures)
					#pragma warning restore 0618
					{
						string newTexturePath = materialPath + materialTexture.name + ".tga";

						Texture newTextureAsset = AssetDatabase.LoadAssetAtPath(newTexturePath, typeof(Texture)) as Texture;

						for (int i = 0; i < propertyCount; i++)
						{
							if (ShaderUtil.GetPropertyType(newMaterial.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
							{
								string propertyName = ShaderUtil.GetPropertyName(newMaterial.shader, i);

								Texture oldTex = newMaterial.GetTexture(propertyName);

								if (oldTex!=null)
								{
									if (newMaterial.GetTexture(propertyName).name == newTextureAsset.name)
									{
										newMaterial.SetTexture(propertyName, newTextureAsset);
									}
								}
								else
								{
									Debug.LogFormat("Property {0} has null Texture", propertyName);
								}

							}
						}

						#pragma warning disable 0618
						// this script explicitly exists for migrating substance materials
						if (materialTexture.GetProceduralOutputType() == ProceduralOutputType.Normal)
						#pragma warning restore 0618
						{
							TextureImporter textureImporter = AssetImporter.GetAtPath(newTexturePath) as TextureImporter;

							textureImporter.textureType = TextureImporterType.NormalMap;

							AssetDatabase.ImportAsset(newTexturePath);
						}
					}
				}

				if (Directory.Exists("EXPORT_HERE"))
				{
					Directory.Delete("EXPORT_HERE");
				}
			}
		}
	}
}
#endif