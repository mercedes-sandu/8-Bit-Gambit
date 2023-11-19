using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Editor.Level_Generator
{
    public class LevelGeneratorEditor : EditorWindow
    {
        #region SerializeFields
        // user-inputted level texture
        [SerializeField] private Texture2D levelTexture;
        
        // user-inputted tilemap
        [SerializeField] private Tilemap boardTilemap;
        
        // colors
        [SerializeField] private Color32 ignoreColor = new (255, 255, 255, 255);
        [SerializeField] private Color32 boardTileColor = new (0, 0, 0, 255);
        [SerializeField] private Color32 playerPawnColor = new (255, 0, 0, 255);
        [SerializeField] private Color32 playerRookColor = new (255, 255, 0, 255);
        [SerializeField] private Color32 playerKnightColor = new (0, 255, 0, 255);
        [SerializeField] private Color32 playerBishopColor = new (0, 255, 255, 255);
        [SerializeField] private Color32 playerQueenColor = new (0, 0, 255, 255);
        [SerializeField] private Color32 playerKingColor = new (255, 0, 255, 255);
        [SerializeField] private Color32 opponentPawnColor = new (255, 128, 0, 255);
        [SerializeField] private Color32 opponentRookColor = new (255, 255, 128, 255);
        [SerializeField] private Color32 opponentKnightColor = new (128, 255, 128, 255);
        [SerializeField] private Color32 opponentBishopColor = new (128, 255, 255, 255);
        [SerializeField] private Color32 opponentQueenColor = new (128, 128, 255, 255);
        [SerializeField] private Color32 opponentKingColor = new (255, 128, 255, 255);
        #endregion

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
        
        // color to prefab dictionary
        private Dictionary<Color32, GameObject> _colorToPrefab = new ();

        // color 2d array
        private Color32[,] _tilemapColors;
        
        // instantiated piece offset
        private static readonly Vector3 PieceOffset = new (0.5f, 0.5f, 0f);

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
        private void LoadAssets()
        {
            _lightBoardTile = Resources.Load<Sprite>("Sprites/Board/LightBoardTile");
            _darkBoardTile = Resources.Load<Sprite>("Sprites/Board/DarkBoardTile");
            _playerPawnPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/PlayerPawn");
            _playerRookPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/PlayerRook");
            _playerKnightPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/PlayerKnight");
            _playerBishopPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/PlayerBishop");
            _playerQueenPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/PlayerQueen");
            _playerKingPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Player/PlayerKing");
            _opponentPawnPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/OpponentPawn");
            _opponentRookPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/OpponentRook");
            _opponentKnightPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/OpponentKnight");
            _opponentBishopPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/OpponentBishop");
            _opponentQueenPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/OpponentQueen");
            _opponentKingPrefab = Resources.Load<GameObject>("Prefabs/Pieces/Opponent/OpponentKing");
        }

        /// <summary>
        /// Opens the level generator and begins loading assets.
        /// </summary>
        private void OnEnable()
        {
            Debug.Log("Opening Level Generator");
            LoadAssets();
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
        /// Maps each color to its corresponding prefab.
        /// </summary>
        private void BuildColorToPrefabDictionary()
        {
            var colors = new[]
            {
                playerPawnColor, playerRookColor, playerKnightColor, playerBishopColor,
                playerQueenColor, playerKingColor, opponentPawnColor, opponentRookColor,
                opponentKnightColor, opponentBishopColor, opponentQueenColor, opponentKingColor
            };

            var prefabs = new[]
            {
                _playerPawnPrefab, _playerRookPrefab, _playerKnightPrefab, _playerBishopPrefab,
                _playerQueenPrefab, _playerKingPrefab, _opponentPawnPrefab, _opponentRookPrefab,
                _opponentKnightPrefab, _opponentBishopPrefab, _opponentQueenPrefab, _opponentKingPrefab
            };

            for (var i = 0; i < colors.Length; i++)
            {
                _colorToPrefab.TryAdd(colors[i], prefabs[i]);
            }
        }

        /// <summary>
        /// Returns whether two Color32's are equal.
        /// </summary>
        /// <param name="color1">The first color.</param>
        /// <param name="color2">The second color.</param>
        /// <returns>True if the two colors are equal, false otherwise.</returns>
        private bool ColorsEqual(Color32 color1, Color32 color2) => color1.r == color2.r && color1.g == color2.g &&
                                                                    color1.b == color2.b && color1.a == color2.a;

        /// <summary>
        /// Places tiles corresponding to the level texture on the board tilemap. A board tile is placed at any pixel
        /// that is not IgnoreColor.
        /// TODO: figure out why tiles get instantiated off to the upper right of center
        /// </summary>
        private void BoardPass()
        {
            for (var x = 0; x < levelTexture.width; x++)
            {
                for (var y = 0; y < levelTexture.height; y++)
                {
                    var color = _tilemapColors[x, y];
                    if (ColorsEqual(color, ignoreColor)) continue;

                    var tile = CreateInstance<Tile>();
                    tile.sprite = (x + y) % 2 == 0 ? _lightBoardTile : _darkBoardTile;
                    boardTilemap.SetTile(new Vector3Int(x, y, 0), tile);
                }
            }
        }

        /// <summary>
        /// Creates a pair of the piece prefab and whether it is a player piece or not.
        /// </summary>
        /// <param name="color">The color corresponding to the prefab.</param>
        /// <returns>The pair (prefab, isPlayerPiece).</returns>
        private (GameObject, bool) GetPiecePrefab(Color32 color) =>
            (_colorToPrefab[color], _colorToPrefab[color].name.Contains("Player"));

        /// <summary>
        /// Places pieces corresponding to the level texture on the board tilemap.
        /// </summary>
        /// <param name="playerPiecesContainer">The container game object which will hold the player's pieces.</param>
        /// <param name="opponentPiecesContainer">The container game object which will hold the opponent's pieces.
        /// </param>
        private void PiecesPass(Transform playerPiecesContainer, Transform opponentPiecesContainer)
        {
            for (var x = 0; x < levelTexture.width; x++)
            {
                for (var y = 0; y < levelTexture.height; y++)
                {
                    var color = _tilemapColors[x, y];
                    if (ColorsEqual(color, ignoreColor) || ColorsEqual(color, boardTileColor)) continue;

                    var tilePosition = new Vector3Int(x, y, 0);
                    var (piece, isPlayerPiece) = GetPiecePrefab(color);
                    var instantiatedPiece = PrefabUtility.InstantiatePrefab(piece,
                        isPlayerPiece ? playerPiecesContainer : opponentPiecesContainer) as GameObject;

                    if (instantiatedPiece) instantiatedPiece.transform.position = tilePosition + PieceOffset;
                }
            }
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
        /// </summary>
        private void GenerateLevel()
        {
            // make sure all loaded assets are not null
            ValidateLoadedAssets();
            
            // build the color to prefab dictionary
            BuildColorToPrefabDictionary();
            
            // get all the colors from the texture
            _tilemapColors = ConvertTo2DArray(levelTexture.GetPixels32(), levelTexture.width, levelTexture.height);
            
            // run the board pass
            BoardPass();
            
            // create containers for the pieces
            var piecesContainer = new GameObject("Pieces");
            var playerPiecesContainer = new GameObject("Player Pieces");
            var opponentPiecesContainer = new GameObject("Opponent Pieces");
            playerPiecesContainer.transform.parent = piecesContainer.transform;
            opponentPiecesContainer.transform.parent = piecesContainer.transform;
            
            // run the pieces pass
            PiecesPass(playerPiecesContainer.transform, opponentPiecesContainer.transform);
            
            // make the scene dirty
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        /// <summary>
        /// Draws the menu UI for the level generator.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Level Generator", EditorStyles.boldLabel);
            GUILayout.Label("Make sure that Read/Write access is set to true for the Texture2D.");
            
            levelTexture =
                (Texture2D)EditorGUILayout.ObjectField("Level Texture", levelTexture, typeof(Texture2D), 
                    false);
            boardTilemap =
                (Tilemap)EditorGUILayout.ObjectField("Board Tilemap", boardTilemap, typeof(Tilemap), 
                    true);
            ignoreColor = EditorGUILayout.ColorField("Ignore Color", ignoreColor);
            boardTileColor = EditorGUILayout.ColorField("Board Tile Color", boardTileColor);
            playerPawnColor = EditorGUILayout.ColorField("Player Pawn Color", playerPawnColor);
            playerRookColor = EditorGUILayout.ColorField("Player Rook Color", playerRookColor);
            playerKnightColor = EditorGUILayout.ColorField("Player Knight Color", playerKnightColor);
            playerBishopColor = EditorGUILayout.ColorField("Player Bishop Color", playerBishopColor);
            playerQueenColor = EditorGUILayout.ColorField("Player Queen Color", playerQueenColor);
            playerKingColor = EditorGUILayout.ColorField("Player King Color", playerKingColor);
            opponentPawnColor = EditorGUILayout.ColorField("Opponent Pawn Color", opponentPawnColor);
            opponentRookColor = EditorGUILayout.ColorField("Opponent Rook Color", opponentRookColor);
            opponentKnightColor = EditorGUILayout.ColorField("Opponent Knight Color", opponentKnightColor);
            opponentBishopColor = EditorGUILayout.ColorField("Opponent Bishop Color", opponentBishopColor);
            opponentQueenColor = EditorGUILayout.ColorField("Opponent Queen Color", opponentQueenColor);
            opponentKingColor = EditorGUILayout.ColorField("Opponent King Color", opponentKingColor);
            
            if (GUILayout.Button("Generate Level")) GenerateLevel();
        }
    }
}