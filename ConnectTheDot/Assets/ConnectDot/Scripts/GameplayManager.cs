using Connect.Common;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Connect.Core;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Connect.Core
{
    public class GameplayManager : MonoBehaviour
    {
        #region START_METHODS


        #region START_VARIABLES
        public static GameplayManager Instance;

        [HideInInspector] public bool hasGameFinished;
        //[HideInInspector] public bool isPause = false;

        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private GameObject _winPopUP;
        [SerializeField] private SpriteRenderer _clickHighlight;
        [SerializeField] private GameObject _effectTouch;

        private void Awake()
        {
            Instance = this;
            //isPause = false;
            hasGameFinished = false;
            _winPopUP.SetActive(false);
            _titleText.gameObject.SetActive(true);
            _titleText.text = "LEVEL " + GameManager.Instance.CurrentLevel.ToString();
            AdManager.instance.ShowBanner();
            CurrentLevelData = GameManager.Instance.GetLevel();

            SpawnBoard();

            SpawnNodes();

        }

        private void Start()
        {
            Observe.OnWin += OnWin;
            interAdsTime = PlayerPrefs.GetFloat(TimerKeyWin, 120f);
            StartCoroutine(UpdateTimer());
        }

        private void OnDestroy()
        {
            Observe.OnWin -= OnWin;

        }

        public void OnWin()
        {


            StartCoroutine(UIManager.Instance.ShowWinPopup());

        }

        #endregion

        #region BOARD_SPAWN

        [SerializeField] private SpriteRenderer _boardPrefab, _bgCellPrefab;

        private void SpawnBoard()
        {
            int currentLevelSize = GameManager.Instance.CurrentStage + 4;

            var board = Instantiate(_boardPrefab,
                new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, 0f),
                Quaternion.identity);

            board.size = new Vector2(currentLevelSize + 0.08f, currentLevelSize + 0.08f);

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    Instantiate(_bgCellPrefab, new Vector3(i + 0.5f, j + 0.5f, 0f), Quaternion.identity);
                }
            }

            Camera.main.orthographicSize = currentLevelSize + 2f;
            Camera.main.transform.position = new Vector3(currentLevelSize / 2f, currentLevelSize / 2f, -10f);

            _clickHighlight.size = new Vector2(currentLevelSize / 4f, currentLevelSize / 4f);
            _clickHighlight.transform.position = Vector3.zero;
            _clickHighlight.gameObject.SetActive(false);
        }

        #endregion

        #region NODE_SPAWN

        private LevelData CurrentLevelData;
        [SerializeField] private Node _nodePrefab;
        private List<Node> _nodes;

        public Dictionary<Vector2Int, Node> _nodeGrid;

        private void SpawnNodes()
        {
            _nodes = new List<Node>();
            _nodeGrid = new Dictionary<Vector2Int, Node>();

            int currentLevelSize = GameManager.Instance.CurrentStage + 4;
            Node spawnedNode;
            Vector3 spawnPos;

            for (int i = 0; i < currentLevelSize; i++)
            {
                for (int j = 0; j < currentLevelSize; j++)
                {
                    spawnPos = new Vector3(i + 0.5f, j + 0.5f, 0f);
                    spawnedNode = Instantiate(_nodePrefab, spawnPos, Quaternion.identity);
                    spawnedNode.Init();

                    int colorIdForSpawnedNode = GetColorId(i, j);

                    if (colorIdForSpawnedNode != -1)
                    {
                        spawnedNode.SetColorForPoint(colorIdForSpawnedNode);
                    }

                    _nodes.Add(spawnedNode);
                    _nodeGrid.Add(new Vector2Int(i, j), spawnedNode);
                    spawnedNode.gameObject.name = i.ToString() + j.ToString();
                    spawnedNode.Pos2D = new Vector2Int(i, j);

                }
            }

            List<Vector2Int> offsetPos = new List<Vector2Int>()
            {Vector2Int.up,Vector2Int.down,Vector2Int.left,Vector2Int.right };

            foreach (var item in _nodeGrid)
            {
                foreach (var offset in offsetPos)
                {
                    var checkPos = item.Key + offset;
                    if (_nodeGrid.ContainsKey(checkPos))
                    {
                        item.Value.SetEdge(offset, _nodeGrid[checkPos]);
                    }
                }
            }


        }

        public List<Color> NodeColors;

        public int GetColorId(int i, int j)
        {
            List<Common.Node> edges = CurrentLevelData.Edges;
            Vector2Int point = new Vector2Int(i, j);

            for (int colorId = 0; colorId < edges.Count; colorId++)
            {
                if (edges[colorId].StartPoint == point ||
                    edges[colorId].EndPoint == point)
                {
                    return colorId;
                }
            }

            return -1;
        }

        public Color GetHighLightColor(int colorID)
        {
            Color result = NodeColors[colorID % NodeColors.Count];
            result.a = 0.4f;
            return result;
        }


        #endregion

        #endregion

        #region UPDATE_METHODS

        private Node startNode;

        private void Update()
        {
            if (hasGameFinished) return;
            /*if (isPause) return;*/
            if (Input.GetMouseButtonDown(0))
            {
                AudioController.Instance.PlaySound(AudioController.Instance.tapDot);
                startNode = null;
                return;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (startNode == null)
                {

                    if (hit && hit.collider.gameObject.TryGetComponent(out Node tNode)
                        && tNode.IsClickable)
                    {

                        SpawnEffect(mousePos);

                        startNode = tNode;
                        _clickHighlight.gameObject.SetActive(true);
                        _clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;
                        _clickHighlight.color = GetHighLightColor(tNode.colorId);
                    }

                    return;
                }

                _clickHighlight.gameObject.transform.position = (Vector3)mousePos2D;

                if (hit && hit.collider.gameObject.TryGetComponent(out Node tempNode)
                    && startNode != tempNode)
                {


                    if (startNode.colorId != tempNode.colorId && tempNode.IsEndNode)
                    {
                        return;
                    }


                    startNode.UpdateInput(tempNode);
                    CheckWin();
                    startNode = null;


                }

                return;
            }

            if (Input.GetMouseButtonUp(0))
            {


                startNode = null;
                _clickHighlight.gameObject.SetActive(false);
            }

        }

        private void SpawnEffect(Vector3 pos)
        {
            /*Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);*/
            Instantiate(_effectTouch, pos, Quaternion.identity);
        }
        #endregion

        #region WIN_CONDITION

        private void CheckWin()
        {
            bool IsWinning = true;

            foreach (var item in _nodes)
            {
                item.SolveHighlight();
            }

            foreach (var item in _nodes)
            {
                IsWinning &= item.IsWin;
                if (!IsWinning)
                {
                    return;
                }
            }



            GameManager.Instance.UnlockLevel();
            Observe.OnWin?.Invoke();
            _clickHighlight.gameObject.SetActive(false);
            hasGameFinished = true;
        }

        #endregion

        #region BUTTON_FUNCTIONS

        public void ClickedBack()
        {
            AudioController.Instance.PlaySound(AudioController.Instance.click);


            //AudioController.Instance.PlayBackgroundMusic();
            GameManager.Instance.GoToMainMenu();
        }

        public void NextLevelClick()
        {
            GameManager.Instance.UnlockLevel();
            // Load the next level
            string levelName = "Level" + GameManager.Instance.CurrentStage.ToString() + GameManager.Instance.CurrentLevel.ToString();
            GameManager.Instance.GoToGameplay();

        }
        public void NextLevelReward()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                UIManager.Instance.ShowNoInternetPopUp();
                return;
            }
            AdManager.instance.ShowReward(() =>
            {
                NextLevelClick();

            }, () =>
            {

            }, "YourPlacementID");
        }




        public void ClickedReload()
        {
            AudioController.Instance.PlaySound(AudioController.Instance.click);



            GameManager.Instance.GoToGameplay();
        }
        public float interAdsTime = 120f;
        private const string TimerKeyWin = "InterAdsTimer";

        private IEnumerator UpdateTimer()
        {
            while (true)
            {
                interAdsTime -= Time.deltaTime;
                PlayerPrefs.SetFloat(TimerKeyWin, interAdsTime);
                yield return null;
            }
        }
        private void ResetTimer()
        {
            interAdsTime = 120f;
            PlayerPrefs.SetFloat(TimerKeyWin, interAdsTime);
        }
        public void ClickedNextLevel()
        {
            if (!hasGameFinished) return;
            AudioController.Instance.PlaySound(AudioController.Instance.click);
            GameManager.Instance.GoToGameplay();
        }

        public void ClickedNextLevelInter()
        {
            if (interAdsTime <= 0)
            {
                AdManager.instance.ShowInter(() =>
                {
                    ResetTimer();
                    ClickedNextLevel();
                },
                () =>
                {
                    ResetTimer();
                    ClickedNextLevel();
                }, "Null");
            }
            else
            {
                ClickedNextLevel();

            }
        }

        public void ClickedBackMenuInter()
        {
            if (interAdsTime <= 0)
            {
                AdManager.instance.ShowInter(() =>
                {
                    ResetTimer();
                    ClickedBack();
                },
                () =>
                {
                    ResetTimer();
                    ClickedBack();
                }, "Null");
            }
            else
            {
                ClickedBack();

            }
        }

        #endregion
    }
}
