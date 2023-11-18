using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Editor.Level_Generator
{
    public class LevelGeneratorEditor : EditorWindow
    {
        // user-inputted level texture
        [SerializeField] private Texture2D levelTexture;
        
        // user-inputted tilemap
        [SerializeField] private Tilemap boardTilemap;

        // sprites to load
        private Sprite _lightBoardTile;
        private Sprite _darkBoardTile;

        // game objects to load
        private GameObject _playerPawnPrefab;
        private GameObject _playerRookPrefab;
        private GameObject _playerKnightPrefab;
        private GameObject _playerBishopPrefab;
        private GameObject _playerQueenPrefab;
        private GameObject _playerKingPrefab;
        private GameObject _opponentPawnPrefab;
        private GameObject _opponentRookPrefab;
        private GameObject _opponentKnightPrefab;
        private GameObject _opponentBishopPrefab;
        private GameObject _opponentQueenPrefab;
        private GameObject _opponentKingPrefab;
        
        // colors
        private static readonly Color32 IgnoreColor = new (255, 255, 255, 255);
        private static readonly Color32 BoardTileColor = new (0, 0, 0, 255);
        private static readonly Color32 PlayerPawnColor = new (255, 0, 0, 255);
        private static readonly Color32 PlayerRookColor = new (255, 255, 0, 255);
        private static readonly Color32 PlayerKnightColor = new (0, 255, 0, 255);
        private static readonly Color32 PlayerBishopColor = new (0, 255, 255, 255);
        private static readonly Color32 PlayerQueenColor = new (0, 0, 255, 255);
        private static readonly Color32 PlayerKingColor = new (255, 0, 255, 255);
        private static readonly Color32 OpponentPawnColor = new (255, 128, 0, 255);
        private static readonly Color32 OpponentRookColor = new (255, 255, 128, 255);
        private static readonly Color32 OpponentKnightColor = new (128, 255, 128, 255);
        private static readonly Color32 OpponentBishopColor = new (128, 255, 255, 255);
        private static readonly Color32 OpponentQueenColor = new (128, 128, 255, 255);
        private static readonly Color32 OpponentKingColor = new (255, 128, 255, 255);

        // color 2d array
        private Color32[,] _tilemapColors;

        /// <summary>
        /// Creates a window for the level generator.
        /// </summary>
        [MenuItem("Level Generator/Generate Level")]
        private static void CreateLevelGenerator()
        {
            GetWindow<LevelGeneratorEditor>();
        }

        /// <summary>
        /// Loads all the assets needed for the level generator from the Resources folder.
        /// </summary>
        private void OnEnable()
        {
            Debug.Log("Opening Level Generator");
            _lightBoardTile = Resources.Load<Sprite>("Sprites/Board/LightBoardTile");
            _darkBoardTile = Resources.Load<Sprite>("Sprites/Board/DarkBoardTile");
            _playerPawnPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/Pawn");
            _playerRookPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/Rook");
            _playerKnightPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/Knight");
            _playerBishopPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/Bishop");
            _playerQueenPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/Queen");
            _playerKingPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/King");
            _opponentPawnPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/Pawn");
            _opponentRookPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/Rook");
            _opponentKnightPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/Knight");
            _opponentBishopPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/Bishop");
            _opponentQueenPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/Queen");
            _opponentKingPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/King");
        }
        
        /// <summary>
        /// Throws an exception if the specified object is null and was not found/loaded.
        /// </summary>
        /// <param name="obj">The object to perform the null check on.</param>
        /// <param name="objectName">The object's name (used for the exception).</param>
        /// <exception cref="Exception"></exception>
        private void ThrowExceptionIfNull(object obj, string objectName)
        {
            if (obj == null) throw new NullReferenceException($"Failed to load {objectName}.");
        }

        /// <summary>
        /// Checks to make sure all the assets needed for the level generator were loaded.
        /// </summary>
        private void ValidateLoadedAssets()
        {
            ThrowExceptionIfNull(_lightBoardTile, "Light Board Tile");
            ThrowExceptionIfNull(_darkBoardTile, "Dark Board Tile");
            ThrowExceptionIfNull(_playerPawnPrefab, "Player Pawn Prefab");
            ThrowExceptionIfNull(_playerRookPrefab, "Player Rook Prefab");
            ThrowExceptionIfNull(_playerKnightPrefab, "Player Knight Prefab");
            ThrowExceptionIfNull(_playerBishopPrefab, "Player Bishop Prefab");
            ThrowExceptionIfNull(_playerQueenPrefab, "Player Queen Prefab");
            ThrowExceptionIfNull(_playerKingPrefab, "Player King Prefab");
            ThrowExceptionIfNull(_opponentPawnPrefab, "Opponent Pawn Prefab");
            ThrowExceptionIfNull(_opponentRookPrefab, "Opponent Rook Prefab");
            ThrowExceptionIfNull(_opponentKnightPrefab, "Opponent Knight Prefab");
            ThrowExceptionIfNull(_opponentBishopPrefab, "Opponent Bishop Prefab");
            ThrowExceptionIfNull(_opponentQueenPrefab, "Opponent Queen Prefab");
            ThrowExceptionIfNull(_opponentKingPrefab, "Opponent King Prefab");
        }

        /// <summary>
        /// Places tiles corresponding to the level texture on the board tilemap.
        /// TODO: implement
        /// </summary>
        private void BoardPass()
        {
            
        }

        /// <summary>
        /// Places pieces corresponding to the level texture on the board tilemap.
        /// TODO: implement
        /// </summary>
        private void PiecesPass()
        {
            
        }

        /// <summary>
        /// Converts the 1D array of colors in the texture to a 2D array of colors.
        /// </summary>
        /// <param name="colors">The 1D array of colors in the texture.</param>
        /// <param name="width">The texture's width.</param>
        /// <param name="height">The texture's height.</param>
        /// <returns>The 2D array of colors in the texture.</returns>
        private Color32[,] ConvertTo2DArray(IReadOnlyList<Color32> colors, int width, int height)
        {
            var colors2D = new Color32[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var index = y * width + x;
                    colors2D[x, y] = colors[index];
                }
            }

            return colors2D;
        }
        
        /// <summary>
        /// Generates a level with the specified level texture.
        /// TODO: finish implementing
        /// </summary>
        private void GenerateLevel()
        {
            // make sure all loaded assets are not null
            // ValidateLoadedAssets();
            
            // get all the colors from the texture
            _tilemapColors = ConvertTo2DArray(levelTexture.GetPixels32(), levelTexture.width, levelTexture.height);
        }

        /// <summary>
        /// Draws the menu UI for the level generator.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Level Generator", EditorStyles.boldLabel);
            GUILayout.Label("Make sure that Texture.isReadable is set to true.");
            levelTexture =
                (Texture2D)EditorGUILayout.ObjectField("Level Texture", levelTexture, typeof(Texture2D), false);
            boardTilemap =
                (Tilemap)EditorGUILayout.ObjectField("Board Tilemap", boardTilemap, typeof(Tilemap), true);
            if (GUILayout.Button("Generate Level")) GenerateLevel();
        }
    }
}