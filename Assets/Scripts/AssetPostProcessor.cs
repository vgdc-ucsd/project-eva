#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

using UnityEditor;

public class MaterialsPostProcessor : AssetPostprocessor {
	Material OnAssignMaterialModel( Material material, Renderer renderer ) {
		//Debug.Log( "Looking for material for " + renderer.name );
		string standardMatPath = "Assets/Models/" + renderer.name + "/Materials/" + material.name + ".mat";
		string standardMatDir = "Assets/Models/" + renderer.name + "/Materials/";
		string standardDiffusePath = "Assets/Models/" + renderer.name + "/Textures/" + renderer.name + "_diffuse.png";
		string standardNormalPath = "Assets/Models/" + renderer.name + "/Textures/" + renderer.name + "_normal.png";

		if( AssetDatabase.LoadAssetAtPath( standardMatPath, typeof( Material ) ) ) {
			//Debug.Log( "Found standard material" );
			return ( Material )AssetDatabase.LoadAssetAtPath( standardMatPath, typeof( Material ) );
		} else {
			//Debug.Log( "Couldn't find standard material, creating new material in " + standardMatPath );
			material.shader = Shader.Find( "Diffuse" );

			if( AssetDatabase.LoadAssetAtPath( standardDiffusePath, typeof( Texture2D ) ) ) {
				//Debug.Log( "Found associated diffuse texture for " + material.name );
				Texture defaultDiffuse = ( Texture2D )AssetDatabase.LoadAssetAtPath( standardDiffusePath, typeof( Texture2D ) );
				material.SetTexture( "Base", defaultDiffuse );
			}

			if( AssetDatabase.LoadAssetAtPath( standardNormalPath, typeof( Texture ) ) ) {
				//Debug.Log( "Found associated normal texture for " + material.name );
				Texture2D defaultNormal = ( Texture2D )AssetDatabase.LoadAssetAtPath( standardNormalPath, typeof( Texture2D ) );
				material.shader = Shader.Find( "Bumped Diffuse" );
				material.SetTexture( "Normalmap", defaultNormal );
			}

			Directory.CreateDirectory( standardMatDir );
			AssetDatabase.CreateAsset( material, standardMatPath );
			return material;
		}
	}
}
#endif