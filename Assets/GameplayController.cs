using NSTools;
using UnityEngine;
using UnityEngine.UI;

namespace zigzag
{

    /// <summary>
    /// Tiny FSM implementation
    /// </summary>
    public enum GameState { Ready, Play, Win, Lose }
    
    public class GameplayController : MonoBehaviour
    {
        public GameState state;
        
        public GameObject tilePrefab, gemPrefab, ballPrefab;
        public Text hint, title;
        public Button touchZone;

        private int difficulty = 1;

        private BallController ballController;
        private Vector2 tileSize = new Vector2(5.11f,2.55f);
        private int level = 1;
        private int gems = 1;

        private LevelData currentLevel;
        
        
        void Start()
        {
            level = PlayerPrefs.GetInt("level", 1);

            touchZone.onClick.AddListener(() =>
            {
                switch (state)
                {
                    case GameState.Ready: 
                        StatePlay(); 
                        break;
                    
                    case GameState.Play: 
                        ballController.Turn(); 
                        break;
                }
            });
            StateReady();
        }
        
        
        /// <summary>
        /// Build scene from level data
        /// </summary>
        /// <param name="levelData"></param>
        private void BuildScene(LevelData levelData)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);

            var tile = Instantiate(tilePrefab, gameObject.transform);
            tile.transform.localPosition = levelData.tiles[0] * tileSize;
            tile.transform.localScale = Vector2.one * 3; 
            tile.name = "path";
            var start_rend = tile.GetComponent<SpriteRenderer>();
            start_rend.color = Color.red;
            start_rend.sortingOrder = 1;

            for (int i = 1; i < levelData.levelLength-1; i++)
            {
               
                tile = Instantiate(tilePrefab, gameObject.transform);
                var tilePos = (levelData.tiles[i]+Vector2.up*2) * tileSize;
                tile.transform.localPosition = tilePos;
                
                tile.transform.localScale = Vector2.one * difficulty;
                tile.name = "path";
                
                if (levelData.gems.Contains(i))
                {
                    var gem = Instantiate(gemPrefab, gameObject.transform);
                    gem.transform.localPosition = tilePos+Vector2.up*tileSize.y;
                    gem.name = "gem";
                    gem.GetComponent<GemController>().gameplayController = this;
                }
            }
            
            tile = Instantiate(tilePrefab, gameObject.transform);
            tile.transform.localPosition = (levelData.tiles[levelData.levelLength-1]+Vector2.up*2) * tileSize;
            tile.transform.localScale = Vector2.one * difficulty; 
            var finish_rend = tile.GetComponent<SpriteRenderer>();
            finish_rend.color = Color.green;
            finish_rend.sortingOrder = 1;
            finish_rend.name = "finish";
        }
        
        
        public void PickGem()
        {
            gems++;
            hint.text = $"Gems: {gems}";
        }
        
         
        /// <summary>
        /// ----------- Game States. Ugly, but without reactive code, huh -----------
        /// </summary>
        
        public void StateReady()
        {
            state = GameState.Ready;
            transform.localPosition = Vector2.zero;
            gems = 0;
            hint.gameObject.SetActive(true);
            hint.text = "Tap to start";
            title.gameObject.SetActive(true);
            title.text = $"Level {level}";
            currentLevel = LevelBuilder.BuildLevelData(level,difficulty);
            BuildScene(currentLevel);
            
            var ballObject = Instantiate(ballPrefab, gameObject.transform);
            ballController = ballObject.GetComponent<BallController>();
            ballController.gameplayController = this;
        }

        public void StatePlay()
        {
            Debug.Log("StatePlay");
            state = GameState.Play;
            hint.gameObject.SetActive(true);
            hint.text = $"Gems: {gems}";
            title.gameObject.SetActive(false);
        }

        public void StateWin()
        {
            Debug.Log("StateWin");
            state = GameState.Win;
            title.gameObject.SetActive(true);
            title.text = "You Win!";
            level++;
            PlayerPrefs.SetInt("level", level);
            EZ.Spawn().Wait(1).Add(StateReady);
        }
        
        public void StateLose()
        {
            Debug.Log("StateLose");
            state = GameState.Lose;
            ballController.Lose();
            title.gameObject.SetActive(true);
            title.text = "You Lose";
            EZ.Spawn().Wait(1.1f).Add(StateReady);
        }
    }
}
