﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using Gazeus.DesafioMatch3.Models;
using Gazeus.DesafioMatch3.ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Gazeus.DesafioMatch3.Views
{
    public class BoardView : MonoBehaviour
    {
        public event Action<int, int> TileClicked;

        [SerializeField] private GridLayoutGroup _boardContainer;
        [SerializeField] private TilePrefabRepository _tilePrefabRepository;
        [SerializeField] private TileSpotView _tileSpotPrefab;

        private GameObject[][] _tiles;
        private TileSpotView[][] _tileSpots;

        private ComboEffect _comboEffect;
        private PointsMatch _pointsMatch;

        private GameObject _selectedTile;
        private Tween _selectedTileTween;

        private void Start()
        {
            _comboEffect = FindObjectOfType<ComboEffect>();
            _pointsMatch = FindObjectOfType<PointsMatch>();
        }

        public void CreateBoard(List<List<Tile>> board)
        {
            _boardContainer.constraintCount = board[0].Count;
            _tiles = new GameObject[board.Count][];
            _tileSpots = new TileSpotView[board.Count][];

            for (int y = 0; y < board.Count; y++)
            {
                _tiles[y] = new GameObject[board[0].Count];
                _tileSpots[y] = new TileSpotView[board[0].Count];

                for (int x = 0; x < board[0].Count; x++)
                {
                    TileSpotView tileSpot = Instantiate(_tileSpotPrefab);
                    tileSpot.transform.SetParent(_boardContainer.transform, false);
                    tileSpot.SetPosition(x, y);
                    tileSpot.Clicked += TileSpot_Clicked;

                    _tileSpots[y][x] = tileSpot;

                    int tileTypeIndex = board[y][x].Type;
                    if (tileTypeIndex > -1)
                    {
                        GameObject tilePrefab = _tilePrefabRepository.TileTypePrefabList[tileTypeIndex];
                        GameObject tile = Instantiate(tilePrefab);
                        tileSpot.SetTile(tile);

                        _tiles[y][x] = tile;
                    }
                }
            }
        }

        public Tween CreateTile(List<AddedTileInfo> addedTiles)
        {
            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < addedTiles.Count; i++)
            {
                AddedTileInfo addedTileInfo = addedTiles[i];
                Vector2Int position = addedTileInfo.Position;

                TileSpotView tileSpot = _tileSpots[position.y][position.x];

                GameObject tilePrefab = _tilePrefabRepository.TileTypePrefabList[addedTileInfo.Type];
                GameObject tile = Instantiate(tilePrefab);
                tileSpot.SetTile(tile);

                _tiles[position.y][position.x] = tile;

                tile.transform.localScale = Vector2.zero;
                sequence.Join(tile.transform.DOScale(1.0f, 0.2f));
            }

            return sequence;
        }

        public Tween DestroyTiles(List<Vector2Int> matchedPosition)
        {
            Dictionary<int, List<Vector2Int>> matchType = new Dictionary<int, List<Vector2Int>>();
            for (int i = 0; i < matchedPosition.Count; i++)
            {
                Vector2Int position = matchedPosition[i];
                GameObject tile = _tiles[position.y][position.x];
                
                if (tile != null)
                {
                    int tileType = tile.GetComponent<TileType>().Type;

                    if (!matchType.ContainsKey(tileType))
                    {
                        matchType[tileType] = new List<Vector2Int>();
                    }
                    matchType[tileType].Add(position);

                    TintImageUI dissolveEffect = tile.GetComponent<TintImageUI>();
                    if (dissolveEffect != null)
                    {
                        dissolveEffect.DissolveImage();
                    }
                    else
                    {
                        Destroy(tile, 0.2f);
                    }

                    _tiles[position.y][position.x] = null;
                }
            }   
            
            foreach(var tileMatched in matchType)
            {
                int tileType = tileMatched.Key;
                List<Vector2Int> positions = tileMatched.Value;
                foreach (Vector2Int pos in positions)
                {
                    Vector3 worldPos = _tileSpots[pos.y][pos.x].transform.position;
                    if (positions.Count >= 5)
                    {
                        ParticlePool.Instance.SpawnFromPool("EffectMatch5", worldPos, Quaternion.identity);
                        _pointsMatch.AddPoints(100);
                    }
                    else if (positions.Count == 4)
                    {
                        ParticlePool.Instance.SpawnFromPool("EffectMatch4", worldPos, Quaternion.identity);
                        _pointsMatch.AddPoints(50);
                    }
                    else if (positions.Count == 3)
                    {
                        ParticlePool.Instance.SpawnFromPool("EffectMatch3", worldPos, Quaternion.identity);
                        _pointsMatch.AddPoints(50);
                    }

                }
            }

            _comboEffect.AddCombo();

            if (_selectedTile != null)
            {
                if (_selectedTileTween != null && _selectedTileTween.IsActive())
                {
                    _selectedTileTween.Kill();
                }
                _selectedTile = null;
            }

            return DOVirtual.DelayedCall(0.2f, () => { });
        }

        public Tween MoveTiles(List<MovedTileInfo> movedTiles)
        {
            GameObject[][] tiles = new GameObject[_tiles.Length][];
            for (int y = 0; y < _tiles.Length; y++)
            {
                tiles[y] = new GameObject[_tiles[y].Length];
                for (int x = 0; x < _tiles[y].Length; x++)
                {
                    tiles[y][x] = _tiles[y][x];
                }
            }

            Sequence sequence = DOTween.Sequence();
            for (int i = 0; i < movedTiles.Count; i++)
            {
                MovedTileInfo movedTileInfo = movedTiles[i];

                Vector2Int from = movedTileInfo.From;
                Vector2Int to = movedTileInfo.To;

                sequence.Join(_tileSpots[to.y][to.x].AnimatedSetTile(_tiles[from.y][from.x]));

                tiles[to.y][to.x] = _tiles[from.y][from.x];
            }

            _tiles = tiles;

            return sequence;
        }

        public Tween SwapTiles(int fromX, int fromY, int toX, int toY)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_tileSpots[fromY][fromX].AnimatedSetTile(_tiles[toY][toX]));
            sequence.Join(_tileSpots[toY][toX].AnimatedSetTile(_tiles[fromY][fromX]));

            (_tiles[toY][toX], _tiles[fromY][fromX]) = (_tiles[fromY][fromX], _tiles[toY][toX]);

            return sequence;
        }

        #region Events
        private void TileSpot_Clicked(int x, int y)
        {
            TileClicked(x, y);

            if (_selectedTile != null)
            {
                if (_selectedTileTween != null && _selectedTileTween.IsActive())
                {
                    _selectedTileTween.Kill();
                }

                if (_selectedTile != null && _selectedTile.transform != null)
                {
                    _selectedTile.transform.localScale = Vector3.one;
                }
            }

            _selectedTile = _tiles[y][x];

            if (_selectedTile != null && _selectedTile.transform != null)
            {
                _selectedTileTween = _selectedTile.transform
                    .DOScale(1.2f, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .OnKill(() =>
                    {
                        if (_selectedTile != null && _selectedTile.transform != null)
                        {
                            _selectedTile.transform.localScale = Vector3.one;
                        }
                    });
            }
        }
        #endregion
    }
}
